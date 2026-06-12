using System;
using System.Collections.Generic;
using UnityEngine;
using APLab.Backend;

namespace APLab.Core
{
    public enum ModulePhase { Idle, Learn, Practice, Assess, Results }

    /// <summary>
    /// Drives a module through Learn -> Practice -> Assess -> Results, collects
    /// practical step scores and quiz outcomes, computes mastery, and syncs the
    /// result to Supabase (falling back to a local queue on failure).
    /// UI and interaction layers call into this; it owns no scene references.
    /// </summary>
    public class ModuleRunner : MonoBehaviour
    {
        public ModuleDefinition Module { get; private set; }
        public ModulePhase Phase { get; private set; } = ModulePhase.Idle;

        public event Action<ModulePhase> OnPhaseChanged;
        public event Action<RubricResult> OnResults;

        private readonly List<float> _practicalScores = new List<float>();
        private int _quizCorrect, _quizTotal;
        private bool _learnCompleted;
        private SessionTracker _session;

        public void Begin(ModuleDefinition module, SessionTracker session)
        {
            Module = module;
            _session = session;
            _practicalScores.Clear();
            _quizCorrect = 0; _quizTotal = 0;
            _learnCompleted = false;

            _session?.StartSession(module.slug,
                string.IsNullOrEmpty(module.courseId) ? null : module.courseId);
            SetPhase(ModulePhase.Learn);
        }

        public void CompleteLearn()
        {
            _learnCompleted = true;
            SetPhase(ModulePhase.Practice);
        }

        /// <summary>Record the awarded fraction (0..1) for a practical step.</summary>
        public void RecordPracticalStep(int index, float awarded01)
        {
            while (_practicalScores.Count <= index) _practicalScores.Add(0f);
            _practicalScores[index] = Mathf.Clamp01(awarded01);
        }

        public void BeginAssess() => SetPhase(ModulePhase.Assess);

        public void RecordQuiz(int correct, int total)
        {
            _quizCorrect = correct;
            _quizTotal = total;
        }

        /// <summary>Compute the rubric, move to Results, end the session, and sync.</summary>
        public RubricResult Finish()
        {
            float quizFraction = _quizTotal > 0 ? (float)_quizCorrect / _quizTotal : 0f;
            var rubric = ScoreModel.Compute(Module, _practicalScores, quizFraction);

            SetPhase(ModulePhase.Results);
            OnResults?.Invoke(rubric);
            _session?.EndSession();
            SyncResult(rubric);
            return rubric;
        }

        private void SyncResult(RubricResult rubric)
        {
            var sb = SupabaseClient.Instance;
            var payload = new LabResultPayload
            {
                ModuleSlug = Module.slug,
                CourseId = string.IsNullOrEmpty(Module.courseId) ? null : Module.courseId,
                ChapterId = Module.HasChapter ? Module.chapterId : (int?)null,
                SectionId = string.IsNullOrEmpty(Module.sectionId) ? null : Module.sectionId,
                LearnCompleted = _learnCompleted,
                PracticalScore = rubric.PracticalScore,
                QuizScore = rubric.QuizScore,
                MasteryScore = rubric.MasteryScore,
                Passed = rubric.Passed,
                Rubric = rubric,
            };

            if (sb == null || !sb.IsSignedIn)
            {
                LocalCache.SavePendingResult(payload);
                return;
            }

            sb.InvokeFunction("vr-sync-result", payload, true,
                body => Debug.Log("[APLab] result synced: " + body),
                err =>
                {
                    Debug.LogWarning("[APLab] result sync failed (queued locally): " + err);
                    LocalCache.SavePendingResult(payload);
                });
        }

        private void SetPhase(ModulePhase p)
        {
            Phase = p;
            OnPhaseChanged?.Invoke(p);
        }
    }
}

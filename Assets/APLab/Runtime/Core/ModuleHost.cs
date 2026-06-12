// A&P Lab — scene orchestrator for a module.
// Loads the content JSON into a ModuleDefinition, owns a ModuleRunner, and drives
// the learner through the phases, binding the Practice steps to pokeable BoneTargets
// on the labeled skull.
//
// Slice 1 (now): full Practice loop — arm the bone targets, score each IdentifyPart
// step via ModuleRunner.RecordPracticalStep, then compute results. Learn is auto-
// completed and the quiz is recorded empty until slices 2–3 add their UI.
//
// autoSelfTest drives perfect play (auto-pokes the correct bone each step) so the
// whole flow can be exercised in Play mode and checked from the console.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using APLab.Core;
using APLab.Content;

namespace APLab.View
{
    public class ModuleHost : MonoBehaviour
    {
        [Header("Content")]
        [Tooltip("The module content JSON (e.g. skeletal-system.json) as a TextAsset.")]
        public TextAsset contentJson;
        [Tooltip("Root of the generated labels (defaults to a 'Skeletal Labels' object).")]
        public Transform labelsRoot;

        [Header("Flow")]
        public bool autoStartOnPlay = true;
        [Tooltip("Auto-poke the correct bone each Practice step (for editor verification).")]
        public bool autoSelfTest = false;
        public int maxAttemptsPerStep = 3;

        ModuleRunner _runner;
        ModuleDefinition _def;
        readonly Dictionary<string, BoneTarget> _targets = new Dictionary<string, BoneTarget>();
        int _stepIndex;
        int _attempts;

        void Start()
        {
            if (autoStartOnPlay) StartModule();
        }

        public void StartModule()
        {
            if (contentJson == null) { Debug.LogError("[APLab] ModuleHost: contentJson not assigned."); return; }

            try { _def = ModuleContentLoader.Build(contentJson.text); }
            catch (System.Exception e) { Debug.LogError("[APLab] content load failed: " + e.Message); return; }

            _runner = GetComponent<ModuleRunner>() ?? gameObject.AddComponent<ModuleRunner>();
            _runner.OnPhaseChanged += OnPhase;
            _runner.OnResults += OnResults;

            GatherTargets();

            var session = APLabManager.Instance != null ? APLabManager.Instance.Session : null;
            Debug.Log($"[APLab] Starting module '{_def.title}' — {_targets.Count} bone targets, " +
                      $"{_def.practical.steps.Count} practical steps.");
            _runner.Begin(_def, session);
        }

        void GatherTargets()
        {
            _targets.Clear();
            if (labelsRoot == null)
            {
                var go = GameObject.Find("Skeletal Labels");
                if (go != null) labelsRoot = go.transform;
            }
            if (labelsRoot == null) { Debug.LogWarning("[APLab] ModuleHost: no labelsRoot / 'Skeletal Labels'."); return; }

            foreach (var t in labelsRoot.GetComponentsInChildren<BoneTarget>(true))
            {
                t.SetArmed(false);
                t.Selected -= OnTargetSelected;
                t.Selected += OnTargetSelected;
                if (!string.IsNullOrEmpty(t.anchorName)) _targets[t.anchorName] = t;
            }
        }

        void OnPhase(ModulePhase phase)
        {
            Debug.Log("[APLab] Phase -> " + phase);
            switch (phase)
            {
                case ModulePhase.Learn:
                    _runner.CompleteLearn();          // slice 1: skip Learn UI (slice 2 adds it)
                    break;
                case ModulePhase.Practice:
                    BeginPracticeStep(0);
                    break;
                case ModulePhase.Assess:
                    _runner.RecordQuiz(0, 0);         // slice 1: no quiz yet (slice 3 adds it)
                    _runner.Finish();
                    break;
            }
        }

        void BeginPracticeStep(int i)
        {
            DisarmAll();
            var steps = _def.practical.steps;

            // Slice 1 scores IdentifyPart steps; record others as 0 and skip past them.
            while (i < steps.Count && steps[i].kind != PracticalStepKind.IdentifyPart)
            {
                Debug.Log($"[APLab] (slice 1 skips {steps[i].kind} step {i})");
                _runner.RecordPracticalStep(i, 0f);
                i++;
            }
            if (i >= steps.Count) { _runner.BeginAssess(); return; }

            _stepIndex = i;
            _attempts = 0;
            var step = steps[i];
            Debug.Log($"[APLab] Practice {i + 1}/{steps.Count}: \"{step.instruction}\"  (answer: {step.correctKey})");

            // Whole skull is the field — arm every bone; only the right one scores.
            foreach (var t in _targets.Values) t.SetArmed(true);

            if (autoSelfTest) StartCoroutine(AutoPick(step.correctKey));
        }

        void OnTargetSelected(BoneTarget t)
        {
            if (_def == null || _runner.Phase != ModulePhase.Practice) return;
            var step = _def.practical.steps[_stepIndex];
            _attempts++;

            if (t.anchorName == step.correctKey)
            {
                t.FlashCorrect();
                float award = 1f / _attempts;       // 1.0 first try, 0.5 second, ...
                _runner.RecordPracticalStep(_stepIndex, award);
                Debug.Log($"[APLab] ✓ {t.anchorName}  award={award:0.00}");
                Advance();
            }
            else
            {
                t.FlashWrong();
                Debug.Log($"[APLab] ✗ {t.anchorName} (want {step.correctKey}) attempt {_attempts}/{maxAttemptsPerStep}");
                if (_attempts >= maxAttemptsPerStep)
                {
                    _runner.RecordPracticalStep(_stepIndex, 0f);
                    Advance();
                }
            }
        }

        void Advance() => StartCoroutine(NextAfter(0.7f));

        IEnumerator NextAfter(float delay)
        {
            DisarmAll();
            yield return new WaitForSeconds(delay);
            BeginPracticeStep(_stepIndex + 1);
        }

        IEnumerator AutoPick(string correctKey)
        {
            yield return new WaitForSeconds(0.5f);
            if (_targets.TryGetValue(correctKey, out var t)) t.Select();
            else Debug.LogWarning("[APLab] autoSelfTest: no target for " + correctKey);
        }

        void DisarmAll()
        {
            foreach (var t in _targets.Values) t.SetArmed(false);
        }

        void OnResults(RubricResult r)
        {
            Debug.Log($"[APLab] RESULTS — practical {r.PracticalScore:0}%  quiz {r.QuizScore:0}%  " +
                      $"mastery {(r.MasteryScore * 100f):0}%  passed={r.Passed}");
        }
    }
}

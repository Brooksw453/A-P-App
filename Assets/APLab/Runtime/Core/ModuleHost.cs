// A&P Lab — scene orchestrator for a module.
// Loads the content JSON into a ModuleDefinition, owns a ModuleRunner, and drives
// the learner through the phases, binding the Practice steps to pokeable BoneTargets
// on the labeled skull.
//
// Practice: show the step instruction on the banner, arm the bone targets, score each
// IdentifyPart step via ModuleRunner.RecordPracticalStep. Assess: hide the skull +
// labels, move the QuizPanel to the skull's spot, run it, and feed the score to
// ModuleRunner.RecordQuiz. Learn is still auto-completed (slice 2 adds the Learn UI).
//
// autoSelfTest drives perfect play (auto-pokes the correct bone each step and auto-
// answers the quiz) so the whole flow can be exercised in Play mode from the console.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using APLab.Core;
using APLab.Content;
using TMPro;

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
        [Tooltip("Auto-poke the correct bone each Practice step, and auto-answer the quiz " +
                 "(for editor verification of the whole flow).")]
        public bool autoSelfTest = false;
        public int maxAttemptsPerStep = 3;

        [Header("Assess")]
        [Tooltip("World-space quiz UI for the Assess phase (build via menu A&P Lab/Build Quiz " +
                 "Panel). If empty, the Assess phase records 0/0 and mastery is practical-only.")]
        public QuizPanel quizPanel;

        [Header("Scene refs (auto-found by name if empty)")]
        [Tooltip("The skull model root ('Skull'). Hidden during the quiz; the panel moves to its spot.")]
        public Transform modelRoot;
        [Tooltip("Practice-phase instruction banner (built by A&P Lab/Build Quiz Panel).")]
        public TextMeshPro instructionText;
        [Tooltip("Banner offset from the skull root: +Y is above, +Z is behind (away from the user). " +
                 "Sits above-and-behind so it clears the exploded skull — tune live in the inspector. " +
                 "Pulled ~0.5 m further back now that the big back panel carries the questions.")]
        public Vector3 bannerOffset = new Vector3(0f, 0.65f, 0.70f);

        ModuleRunner _runner;
        ModuleDefinition _def;
        readonly Dictionary<string, BoneTarget> _targets = new Dictionary<string, BoneTarget>();
        readonly List<BoneTarget> _all = new List<BoneTarget>();   // every target (both L/R) — used for arming
        int _stepIndex;
        int _attempts;
        bool _loaded;

        void Start()
        {
            if (autoStartOnPlay) StartModule();
        }

        // Legacy linear flow (Learn->Practice->Assess->Results). Kept for autoSelfTest and any scene
        // that still wants the auto-run rubric. The new LabModeController boots into Explore instead
        // (calls EnsureLoaded() directly), so with autoStartOnPlay=false this never runs.
        public void StartModule()
        {
            if (!EnsureLoaded()) return;
            var session = APLabManager.Instance != null ? APLabManager.Instance.Session : null;
            Debug.Log($"[APLab] Starting module '{_def.title}' — {_targets.Count} bone targets, " +
                      $"{_def.practical.steps.Count} practical steps.");
            _runner.Begin(_def, session);
        }

        /// <summary>Parse the content, build the runner, gather bone targets, and find scene refs —
        /// once (idempotent). Returns false if content is missing. LabModeController calls this before
        /// driving the three modes; StartModule() calls it before the legacy auto-run.</summary>
        public bool EnsureLoaded()
        {
            if (_loaded) return true;
            if (contentJson == null) { Debug.LogError("[APLab] ModuleHost: contentJson not assigned."); return false; }

            try { _def = ModuleContentLoader.Build(contentJson.text); }
            catch (System.Exception e) { Debug.LogError("[APLab] content load failed: " + e.Message); return false; }

            _runner = GetComponent<ModuleRunner>() ?? gameObject.AddComponent<ModuleRunner>();
            _runner.OnPhaseChanged -= OnPhase; _runner.OnPhaseChanged += OnPhase;
            _runner.OnResults -= OnResults;   _runner.OnResults += OnResults;

            GatherTargets();
            if (quizPanel == null)
                quizPanel = FindFirstObjectByType<QuizPanel>(FindObjectsInactive.Include);
            if (modelRoot == null)
            {
                var s = GameObject.Find("Skull") ?? GameObject.Find("Skull (Exploding)");
                if (s != null) modelRoot = s.transform;
            }
            if (instructionText == null)
            {
                var b = GameObject.Find("Instruction Banner");
                if (b != null) instructionText = b.GetComponent<TextMeshPro>();
            }

            _loaded = true;
            return true;
        }

        void GatherTargets()
        {
            _targets.Clear();
            _all.Clear();
            if (labelsRoot == null)
            {
                var go = GameObject.Find("Skeletal Labels");
                if (go != null) labelsRoot = go.transform;
            }

            BoneTarget[] found = labelsRoot != null
                ? labelsRoot.GetComponentsInChildren<BoneTarget>(true)
                : System.Array.Empty<BoneTarget>();
            if (found.Length == 0)   // exploding-skull bones carry the BoneTargets under 'Skull (Exploding)', not labelsRoot
                found = FindObjectsByType<BoneTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (found.Length == 0) { Debug.LogWarning("[APLab] ModuleHost: no BoneTargets found in scene."); return; }

            foreach (var t in found)
            {
                t.SetArmed(false);
                t.Selected -= OnTargetSelected;
                t.Selected += OnTargetSelected;
                _all.Add(t);                                       // arm every bone, both sides
                if (!string.IsNullOrEmpty(t.anchorName)) _targets[t.anchorName] = t;  // one-per-anchor for auto-test lookup
            }
        }

        // ---- Public accessors for LabModeController (the 3-mode UI brain) ----
        public ModuleDefinition Def => _def;
        public QuizData Quiz => _def != null ? _def.quiz : null;
        public IReadOnlyList<BoneTarget> AllTargets => _all;

        /// <summary>Arm/disarm every bone (Explore + Bone Quiz need them clickable). Mesh-mode arming
        /// is visually inert, so this only flips the gate that lets BoneTarget.Select() fire.</summary>
        public void ArmAll(bool on) { foreach (var t in _all) if (t != null) t.SetArmed(on); }

        /// <summary>Look up a bone's authored label (term/definition/pronunciation/landmarks/related)
        /// by anchorName, from the already-parsed content — for filling the Explore info panels.</summary>
        public AnatomyLabel GetLabelByAnchor(string anchor)
        {
            if (_def == null || _def.labels == null || _def.labels.labels == null || string.IsNullOrEmpty(anchor)) return null;
            foreach (var l in _def.labels.labels)
                if (l != null && string.Equals(l.anchorName, anchor, System.StringComparison.OrdinalIgnoreCase)) return l;
            return null;
        }

        void OnPhase(ModulePhase phase)
        {
            Debug.Log("[APLab] Phase -> " + phase);
            switch (phase)
            {
                case ModulePhase.Learn:
                    HideInstruction();
                    _runner.CompleteLearn();          // slice 1: skip Learn UI (slice 2 adds it)
                    break;
                case ModulePhase.Practice:
                    BeginPracticeStep(0);
                    break;
                case ModulePhase.Assess:
                    BeginAssess();
                    break;
            }
        }

        // Run the in-headset quiz, then record the score and finish. Falls back to an
        // empty quiz (0/0, practical-only mastery) when no panel is present.
        void BeginAssess()
        {
            bool hasQuiz = quizPanel != null && _def.quiz != null &&
                           _def.quiz.questions != null && _def.quiz.questions.Count > 0;
            if (!hasQuiz)
            {
                if (quizPanel == null) Debug.Log("[APLab] Assess: no quiz panel — recording 0/0.");
                _runner.RecordQuiz(0, 0);
                _runner.Finish();
                return;
            }

            // Clear the stage: hide the practice banner, the labels, and the skull, then
            // move the quiz panel back to where the skull was so it reads at a comfortable
            // distance instead of in the learner's face.
            HideInstruction();
            if (labelsRoot != null) labelsRoot.gameObject.SetActive(false);
            if (modelRoot != null) modelRoot.gameObject.SetActive(false);
            if (modelRoot != null && quizPanel != null)
            {
                var pp = quizPanel.transform.position;
                quizPanel.transform.position = new Vector3(modelRoot.position.x, pp.y, modelRoot.position.z);
            }

            quizPanel.Begin(_def.quiz, autoSelfTest, (correct, total) =>
            {
                _runner.RecordQuiz(correct, total);
                _runner.Finish();
            });
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
            ShowInstruction($"Practice  {i + 1} / {steps.Count}\n{step.instruction}");

            // Whole skull is the field — arm every bone (both L/R); only the right one scores.
            foreach (var t in _all) t.SetArmed(true);

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
            foreach (var t in _all) t.SetArmed(false);
        }

        void ShowInstruction(string text)
        {
            if (instructionText == null) return;
            if (modelRoot != null)
                instructionText.transform.position = modelRoot.position + bannerOffset;
            instructionText.gameObject.SetActive(true);
            instructionText.text = text;
            instructionText.ForceMeshUpdate();
        }

        void HideInstruction()
        {
            if (instructionText != null) instructionText.gameObject.SetActive(false);
        }

        void OnResults(RubricResult r)
        {
            // Restore the skull + labels for the idle end state.
            if (modelRoot != null) modelRoot.gameObject.SetActive(true);
            if (labelsRoot != null) labelsRoot.gameObject.SetActive(true);
            Debug.Log($"[APLab] RESULTS — practical {r.PracticalScore:0}%  quiz {r.QuizScore:0}%  " +
                      $"mastery {(r.MasteryScore * 100f):0}%  passed={r.Passed}");
        }
    }
}

// A&P Lab — the skull lab's 3-mode brain.
//
// Turns the skull lab into an EXPLORER first, with two opt-in activities. The user
// toggles between three modes from a row of pokeable ModeButtons on the back panel:
//
//   • EXPLORE  (default): bones are freely clickable. Hover = cyan (HandRaySelector),
//     click = persistent GREEN (BoneTarget.SetSelected, single-selection). The two side
//     info panels fill: LEFT = term + description, RIGHT = pronunciation + landmarks +
//     related ("see also") bones, looked up from the parsed content via ModuleHost.
//   • INFO QUIZ: hides the skull/labels/sliders/panels and runs the existing QuizPanel
//     (a round of randomized questions from the bank), then returns to Explore.
//   • BONE QUIZ: paints a "select the bone that…" prompt on BOTH side panels, arms the
//     bones, scores the pick (reusing BoneTarget flash + anchorName compare), advances,
//     then returns to Explore.
//
// Architecture: this is the UI/mode layer; ModuleHost stays the data/scoring backbone
// (it owns the parsed content + the bone targets + the QuizPanel). We never auto-run the
// linear rubric — autoStartOnPlay is false and we EnterMode(Explore) on Start().
//
// Visibility per mode = SetActive on ALREADY-BUILT objects (drift-safe; the XR-rig rule
// forbids CREATING colliders at runtime, not toggling existing ones). All pokeable UI is
// authored as static scene objects by LabUIBuilder; this only enables/disables + fills.
//
// Scores stay LOCAL this pass (auth/sync deferred) — shown on the back-panel status line,
// not pushed to ModuleRunner/Supabase.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using APLab.Core;

namespace APLab.View
{
    public class LabModeController : MonoBehaviour
    {
        public enum Mode { Explore, InfoQuiz, BoneQuiz }

        [Header("Backbone (auto-found if empty)")]
        public ModuleHost host;

        [Header("Mode buttons (wired by A&P Lab/Build Lab UI)")]
        public ModeButton exploreButton;
        public ModeButton infoQuizButton;
        public ModeButton boneQuizButton;

        [Header("Info panels — text (wired by A&P Lab/Build Lab UI)")]
        public TextMeshPro leftTitle;       // term
        public TextMeshPro leftBody;        // description (Explore) / question (Bone Quiz)
        public TextMeshPro rightPron;       // pronunciation
        public TextMeshPro rightLandmarks;  // key landmarks list
        public TextMeshPro rightRelated;    // "see also" list / hint
        [Tooltip("Status line on the back panel (mode hint, quiz score).")]
        public TextMeshPro statusText;

        [Header("Toggled scene objects (wired by A&P Lab/Build Lab UI; found by name)")]
        public Transform skullRoot;         // "Skull (Exploding)"
        public Transform labelsRoot;        // "Bone Labels"
        public Transform explodeSlider;     // "Explode Slider"
        public Transform rotateControl;     // "Rotate Control"
        public QuizPanel quizPanel;         // "Quiz Panel"
        public Transform infoPanelLeft;     // "Info Panel Left"
        public Transform infoPanelRight;    // "Info Panel Right"
        [Tooltip("The old Practice instruction banner — forced hidden in every mode now.")]
        public Transform instructionBanner; // "Instruction Banner"
        [Tooltip("The Lab Back Panel root — the Info Quiz panel is moved to this depth so it isn't in your face.")]
        public Transform backPanelRoot;     // "Lab Back Panel"

        [Header("Timing")]
        public float revealDelay = 1.0f;    // pause so a Bone Quiz flash reads before advancing
        public float returnDelay = 2.0f;    // pause on a result before returning to Explore
        public int maxAttemptsPerBone = 3;

        Mode _mode = Mode.Explore;
        BoneTarget _selected;               // current Explore green selection
        bool _subscribed;

        // Bone Quiz round state
        List<PracticalStep> _bqRound;
        int _bqIndex, _bqCorrect, _bqAttempts;
        bool _bqLocked;

        void Start()
        {
            if (host == null) host = GetComponent<ModuleHost>();
            if (host == null) host = FindFirstObjectByType<ModuleHost>(FindObjectsInactive.Include);
            if (host == null) { Debug.LogError("[APLab] LabModeController: no ModuleHost in scene."); return; }

            host.EnsureLoaded();
            SubscribeButtons();
            SubscribeBones();
            EnterMode(Mode.Explore);
            Debug.Log("[APLab] LabModeController ready — booted into Explore.");
        }

        // ---- wiring ----
        void SubscribeButtons()
        {
            Hook(exploreButton); Hook(infoQuizButton); Hook(boneQuizButton);
        }
        void Hook(ModeButton b)
        {
            if (b == null) return;
            b.Picked -= OnModePicked; b.Picked += OnModePicked;
        }
        void OnModePicked(ModeButton b)
        {
            if (b == exploreButton) EnterMode(Mode.Explore);
            else if (b == infoQuizButton) EnterMode(Mode.InfoQuiz);
            else if (b == boneQuizButton) EnterMode(Mode.BoneQuiz);
        }

        void SubscribeBones()
        {
            if (_subscribed || host.AllTargets == null) return;
            foreach (var t in host.AllTargets)
            {
                if (t == null) continue;
                t.Selected -= OnBoneSelected; t.Selected += OnBoneSelected;
            }
            _subscribed = true;
        }

        // ---- mode entry ----
        public void EnterMode(Mode m)
        {
            StopAllCoroutines();        // cancel any pending advance / return
            ClearSelection();
            _bqRound = null; _bqLocked = false;

            _mode = m;
            ApplyVisibility(m);
            HighlightModeButton(m);

            switch (m)
            {
                case Mode.Explore:  EnterExplore();  break;
                case Mode.InfoQuiz: EnterInfoQuiz(); break;
                case Mode.BoneQuiz: EnterBoneQuiz(); break;
            }
        }

        void ApplyVisibility(Mode m)
        {
            bool explore = m == Mode.Explore;
            bool bone    = m == Mode.BoneQuiz;
            bool info    = m == Mode.InfoQuiz;

            SetActive(skullRoot,     explore || bone);
            SetActive(labelsRoot,    explore);              // bone names hidden in Bone Quiz (don't reveal the answer)
            SetActive(explodeSlider, explore || bone);
            SetActive(rotateControl, explore || bone);
            SetActive(infoPanelLeft,  explore || bone);      // shows description (Explore) or prompt (Bone Quiz)
            SetActive(infoPanelRight, explore || bone);
            SetActive(instructionBanner, false);             // the old banner is retired — never shown
            if (quizPanel != null && !info) quizPanel.gameObject.SetActive(false);  // Begin() activates it for Info Quiz
            // The middle backdrop + the mode bar live on the 'Lab Back Panel' root and stay ON in
            // every mode (the mode bar must always be reachable), so they're never toggled here.
        }

        // ===== EXPLORE =====
        void EnterExplore()
        {
            host.ArmAll(true);                    // bones clickable (mesh-mode arming is visually inert)
            SetInfoIdle();
            SetText(statusText, "EXPLORE  —  tap any bone to learn about it");
        }

        void SetInfoIdle()
        {
            SetText(leftTitle, "Select a bone");
            SetText(leftBody, "Point at any bone on the skull and pinch to see its name, " +
                              "pronunciation, and description here.");
            SetText(rightPron, "");
            SetText(rightLandmarks, "");
            SetText(rightRelated, "");
        }

        void ExploreSelect(BoneTarget t)
        {
            if (_selected == t) return;                   // re-click same bone: no-op
            if (_selected != null) _selected.SetSelected(false);
            _selected = t;
            t.SetSelected(true);                          // persistent green (single-selection)
            FillInfo(t.anchorName);
        }

        void FillInfo(string anchor)
        {
            var label = host.GetLabelByAnchor(anchor);
            if (label == null)
            {
                SetText(leftTitle, string.IsNullOrEmpty(anchor) ? "Bone" : anchor);
                SetText(leftBody, "");
                SetText(rightPron, ""); SetText(rightLandmarks, ""); SetText(rightRelated, "");
                return;
            }
            SetText(leftTitle, label.term);
            SetText(leftBody, label.definition);
            SetText(rightPron, string.IsNullOrEmpty(label.pronunciation) ? "" : "Say it:  " + label.pronunciation);
            SetText(rightLandmarks, FormatList("Key landmarks", label.landmarks));
            SetText(rightRelated, FormatList("See also", label.related));
        }

        // ===== INFO QUIZ =====
        void EnterInfoQuiz()
        {
            host.ArmAll(false);
            var quiz = host.Quiz;
            if (quizPanel == null || quiz == null || quiz.questions == null || quiz.questions.Count == 0)
            {
                SetText(statusText, "No quiz available.");
                StartCoroutine(ReturnToExploreAfter(returnDelay));
                return;
            }
            // Move the quiz panel back to the BACK-PANEL depth so it isn't in the user's face
            // (the skull is hidden during the quiz). Falls back to the skull's x/z.
            if (backPanelRoot != null)
                quizPanel.transform.position = backPanelRoot.position + new Vector3(0f, 0f, -0.10f);
            else if (skullRoot != null)
            {
                var pp = quizPanel.transform.position;
                quizPanel.transform.position = new Vector3(skullRoot.position.x, pp.y, skullRoot.position.z);
            }

            SetText(statusText, "INFO QUIZ");
            quizPanel.Begin(quiz, false, (correct, total) =>
            {
                if (_mode != Mode.InfoQuiz) return;       // user bailed mid-round
                SetText(statusText, $"Info Quiz:  {correct} / {total} correct");
                StartCoroutine(ReturnToExploreAfter(returnDelay));
            });
        }

        // ===== BONE QUIZ =====
        void EnterBoneQuiz()
        {
            _bqRound = new List<PracticalStep>();
            var steps = host.Def != null && host.Def.practical != null ? host.Def.practical.steps : null;
            if (steps != null)
                foreach (var s in steps)
                    if (s.kind == PracticalStepKind.IdentifyPart && !string.IsNullOrEmpty(s.correctKey))
                        _bqRound.Add(s);
            Shuffle(_bqRound);
            _bqIndex = 0; _bqCorrect = 0;

            if (_bqRound.Count == 0)
            {
                SetText(leftTitle, "Bone Quiz");
                SetText(leftBody, "No questions available.");
                SetText(rightPron, ""); SetText(rightLandmarks, ""); SetText(rightRelated, "");
                StartCoroutine(ReturnToExploreAfter(returnDelay));
                return;
            }

            host.ArmAll(true);
            ShowBoneQuestion(0);
        }

        void ShowBoneQuestion(int i)
        {
            _bqLocked = false; _bqAttempts = 0;
            var step = _bqRound[i];
            // The prompt shows on BOTH side panels so it's readable from either side of the skull.
            SetText(leftTitle, $"Bone Quiz   {i + 1} / {_bqRound.Count}");
            SetText(leftBody, step.instruction);
            SetText(rightPron, "");
            SetText(rightLandmarks, $"Question {i + 1} / {_bqRound.Count}");
            SetText(rightRelated, string.IsNullOrEmpty(step.hint) ? "Pinch the correct bone on the skull." : "Hint:  " + step.hint);
            SetText(statusText, "BONE QUIZ  —  select the correct bone");
        }

        void BoneQuizAnswer(BoneTarget t)
        {
            if (_bqLocked || _bqRound == null || _bqIndex >= _bqRound.Count) return;
            var step = _bqRound[_bqIndex];
            _bqAttempts++;

            if (t.anchorName == step.correctKey)
            {
                _bqLocked = true;
                t.FlashCorrect();
                _bqCorrect++;
                StartCoroutine(NextBoneAfter(revealDelay));
            }
            else
            {
                t.FlashWrong();
                if (_bqAttempts >= maxAttemptsPerBone)
                {
                    _bqLocked = true;
                    var correct = FindTarget(step.correctKey);
                    if (correct != null) correct.FlashCorrect();   // reveal the answer
                    StartCoroutine(NextBoneAfter(revealDelay));
                }
            }
        }

        IEnumerator NextBoneAfter(float d)
        {
            yield return new WaitForSeconds(d);
            _bqIndex++;
            if (_bqRound != null && _bqIndex < _bqRound.Count) ShowBoneQuestion(_bqIndex);
            else FinishBoneQuiz();
        }

        void FinishBoneQuiz()
        {
            int total = _bqRound != null ? _bqRound.Count : 0;
            SetText(leftTitle, "Bone Quiz complete");
            SetText(leftBody, $"You identified {_bqCorrect} of {total} bones correctly.");
            SetText(rightPron, ""); SetText(rightLandmarks, ""); SetText(rightRelated, "");
            SetText(statusText, $"Bone Quiz:  {_bqCorrect} / {total}");
            StartCoroutine(ReturnToExploreAfter(returnDelay));
        }

        // ---- shared ----
        IEnumerator ReturnToExploreAfter(float d)
        {
            yield return new WaitForSeconds(d);
            EnterMode(Mode.Explore);
        }

        void OnBoneSelected(BoneTarget t)
        {
            if (t == null) return;
            if (_mode == Mode.Explore) ExploreSelect(t);
            else if (_mode == Mode.BoneQuiz) BoneQuizAnswer(t);
            // InfoQuiz: bones are hidden + disarmed, so this never fires there.
        }

        void ClearSelection()
        {
            if (_selected != null) _selected.SetSelected(false);
            _selected = null;
        }

        void HighlightModeButton(Mode m)
        {
            if (exploreButton  != null) exploreButton.SetCurrent(m == Mode.Explore);
            if (infoQuizButton != null) infoQuizButton.SetCurrent(m == Mode.InfoQuiz);
            if (boneQuizButton != null) boneQuizButton.SetCurrent(m == Mode.BoneQuiz);
        }

        BoneTarget FindTarget(string anchor)
        {
            if (host.AllTargets == null) return null;
            foreach (var t in host.AllTargets)
                if (t != null && t.anchorName == anchor) return t;
            return null;
        }

        static string FormatList(string header, List<string> items)
        {
            if (items == null || items.Count == 0) return "";
            var sb = new System.Text.StringBuilder();
            sb.Append("<b>").Append(header).Append("</b>");
            foreach (var it in items)
                if (!string.IsNullOrEmpty(it)) sb.Append("\n• ").Append(it);
            return sb.ToString();
        }

        static void SetActive(Transform t, bool on)
        {
            if (t != null && t.gameObject.activeSelf != on) t.gameObject.SetActive(on);
        }

        static void SetText(TextMeshPro t, string s)
        {
            if (t == null) return;
            t.text = s ?? "";
            t.ForceMeshUpdate();
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}

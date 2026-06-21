// A&P Lab — world-space multiple-choice quiz panel (the Assess phase UI).
//
// Presents a round of questions from the module's QuizData and reports the score
// back to the ModuleHost, which feeds ModuleRunner.RecordQuiz -> ScoreModel. The
// buttons themselves are built ONCE as static scene objects by QuizPanelBuilder
// (menu: A&P Lab/Build Quiz Panel) so we never create colliders at runtime under
// the XR rig — that was the tracking-origin drift ("skull flies up") cause. This
// component only fills text, arms/disarms options, flashes results, and advances.
//
// autoAnswer mode auto-picks the correct option each question (mirrors ModuleHost
// autoSelfTest) so the full flow can be exercised in Play mode from the console.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace APLab.View
{
    public class QuizPanel : MonoBehaviour
    {
        [Header("Wiring (set by QuizPanelBuilder)")]
        [Tooltip("The question prompt text.")]
        public TextMeshPro questionText;
        [Tooltip("Progress readout, e.g. 'Question 2 / 5'.")]
        public TextMeshPro progressText;
        [Tooltip("Answer buttons in order; a round uses as many as each question needs.")]
        public List<QuizOption> options = new List<QuizOption>();

        [Header("Timing")]
        [Tooltip("Pause after an answer so the correct/wrong flash reads before advancing.")]
        public float revealDelay = 1.0f;

        List<QuestionData> _round;
        int _qIndex, _correct;
        bool _locked, _auto;
        Action<int, int> _onComplete;

        // Visibility is managed externally now: the UI builders leave this panel INACTIVE in the
        // scene, and LabModeController (Info Quiz) / ModuleHost (Assess) activate it via Begin().
        // No self-disable in Awake — that would fight an external SetActive(true) when the panel
        // starts inactive (Awake fires during the first activation and would immediately re-hide it).

        /// <summary>
        /// Run a quiz round. <paramref name="onComplete"/>(correct, total) fires when
        /// the round ends. Safe to call with a null/empty quiz (reports 0/0).
        /// </summary>
        public void Begin(QuizData quiz, bool autoAnswer, Action<int, int> onComplete)
        {
            _auto = autoAnswer;
            _onComplete = onComplete;

            if (quiz == null || quiz.questions == null || quiz.questions.Count == 0)
            {
                onComplete?.Invoke(0, 0);
                return;
            }

            _round = PickRound(quiz);
            _qIndex = 0;
            _correct = 0;

            gameObject.SetActive(true);
            foreach (var o in options)
            {
                if (o == null) continue;
                o.Selected -= OnOptionSelected;
                o.Selected += OnOptionSelected;
            }
            ShowQuestion(0);
        }

        static List<QuestionData> PickRound(QuizData quiz)
        {
            var pool = new List<QuestionData>(quiz.questions);
            // Fisher-Yates shuffle so each attempt draws a fresh round from the bank.
            for (int i = pool.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (pool[i], pool[j]) = (pool[j], pool[i]);
            }
            int n = quiz.questionsPerRound > 0
                ? Mathf.Min(quiz.questionsPerRound, pool.Count)
                : pool.Count;
            return pool.GetRange(0, n);
        }

        void ShowQuestion(int i)
        {
            _locked = false;
            var q = _round[i];

            if (questionText != null) { questionText.text = q.questionText; questionText.ForceMeshUpdate(); }
            if (progressText != null) { progressText.text = $"Question {i + 1} / {_round.Count}"; progressText.ForceMeshUpdate(); }

            for (int o = 0; o < options.Count; o++)
            {
                if (options[o] == null) continue;
                bool used = q.answerOptions != null && o < q.answerOptions.Count;
                options[o].SetVisible(used);
                if (!used) continue;
                options[o].index = o;
                if (options[o].label != null)
                {
                    options[o].label.text = q.answerOptions[o];
                    options[o].label.ForceMeshUpdate();
                }
                options[o].SetArmed(true);
            }

            if (_auto) StartCoroutine(AutoAnswer(q.correctAnswerIndex));
        }

        IEnumerator AutoAnswer(int correctIndex)
        {
            yield return new WaitForSeconds(0.5f);
            if (correctIndex >= 0 && correctIndex < options.Count && options[correctIndex] != null)
                options[correctIndex].Select();
        }

        void OnOptionSelected(QuizOption opt)
        {
            if (_locked) return;
            _locked = true;
            foreach (var o in options) if (o != null) o.SetArmed(false);

            var q = _round[_qIndex];
            bool right = opt.index == q.correctAnswerIndex;
            if (right)
            {
                _correct++;
                opt.FlashCorrect();
            }
            else
            {
                opt.FlashWrong();
                if (q.correctAnswerIndex >= 0 && q.correctAnswerIndex < options.Count && options[q.correctAnswerIndex] != null)
                    options[q.correctAnswerIndex].FlashCorrect();   // reveal the right answer
            }
            Debug.Log($"[APLab] Quiz {_qIndex + 1}/{_round.Count}: {(right ? "✓" : "✗")} " +
                      $"chose {opt.index}, correct {q.correctAnswerIndex}");

            StartCoroutine(NextAfter(revealDelay));
        }

        IEnumerator NextAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            _qIndex++;
            if (_qIndex < _round.Count) ShowQuestion(_qIndex);
            else Finish();
        }

        void Finish()
        {
            int total = _round.Count;
            Debug.Log($"[APLab] Quiz round complete — {_correct}/{total} correct.");
            gameObject.SetActive(false);
            _onComplete?.Invoke(_correct, total);
        }
    }
}

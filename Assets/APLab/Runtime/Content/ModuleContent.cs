// A&P Lab — runtime content loader.
// Parses a module content JSON (e.g. skeletal-system.json) into a ModuleDefinition
// (+ LabelSet, LearnSequence, PracticalTask, QuizData) built in memory, so the
// framework runs straight off the authored JSON with no hand-built ScriptableObject
// assets. The same JSON is the single source of truth shared with the web course.
//
// Pass in the TextAsset .text (works in Editor and in player builds — unlike
// File.ReadAllText of an Assets/ path, which is not available at runtime).

using System;
using UnityEngine;
using Newtonsoft.Json;
using APLab.Core;

namespace APLab.Content
{
    public static class ModuleContentLoader
    {
        // ---- JSON shapes (Newtonsoft matches member names case-insensitively) ----
        [Serializable] class JModule
        {
            public string slug, title, unit, courseId, sectionId, primaryModel, attribution, systemColor;
            public int chapterId = -1;
            public int estimatedMinutes = 15;
            public JLabel[] labels;
            public JLearn learn;
            public JPractical practical;
            public JQuiz quiz;
        }
        [Serializable] class JLabel { public string anchorName, term, pronunciation, definition; public string[] meshNameHints, landmarks, related; }
        [Serializable] class JLearn { public JKeyTerm[] keyTerms; public JLearnStep[] steps; }
        [Serializable] class JKeyTerm { public string term, definition; }
        [Serializable] class JLearnStep { public string kind, prompt, targetPartName; public string[] keyTerms; }
        [Serializable] class JPractical { public string title; public JPracticalStep[] steps; }
        [Serializable] class JPracticalStep { public string kind, instruction, correctKey, hint; public float weight = 1f; }
        [Serializable] class JQuiz { public string quizName; public int questionsPerRound = 5; public int pointsPerCorrectAnswer = 20; public JQuestion[] questions; }
        [Serializable] class JQuestion { public string questionText; public string[] answerOptions; public int correctAnswerIndex; }

        /// <summary>Build an in-memory ModuleDefinition from a content JSON string.</summary>
        public static ModuleDefinition Build(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Module content JSON is empty.");

            var m = JsonConvert.DeserializeObject<JModule>(json);
            if (m == null) throw new ArgumentException("Module content JSON did not parse.");

            var def = ScriptableObject.CreateInstance<ModuleDefinition>();
            def.slug = m.slug ?? "";
            def.title = m.title ?? def.slug;
            def.unit = ParseEnum(m.unit, APUnit.AP1);
            def.courseId = m.courseId ?? "";
            def.chapterId = m.chapterId;
            def.sectionId = m.sectionId ?? "";
            def.estimatedMinutes = m.estimatedMinutes;
            if (!string.IsNullOrEmpty(m.systemColor) && ColorUtility.TryParseHtmlString(m.systemColor, out var c))
                def.systemColor = c;

            // Labels
            var labelSet = ScriptableObject.CreateInstance<LabelSet>();
            labelSet.forModelName = m.primaryModel ?? "";
            if (m.labels != null)
                foreach (var l in m.labels)
                    labelSet.labels.Add(new AnatomyLabel
                    {
                        anchorName = l.anchorName,
                        term = l.term,
                        definition = l.definition,
                        pronunciation = l.pronunciation,
                        landmarks = l.landmarks != null
                            ? new System.Collections.Generic.List<string>(l.landmarks)
                            : new System.Collections.Generic.List<string>(),
                        related = l.related != null
                            ? new System.Collections.Generic.List<string>(l.related)
                            : new System.Collections.Generic.List<string>(),
                    });
            def.labels = labelSet;

            // Learn
            var learn = new LearnSequence();
            if (m.learn?.steps != null)
                foreach (var s in m.learn.steps)
                    learn.steps.Add(new LearnStep
                    {
                        kind = ParseEnum(s.kind, LearnStepKind.FocusPart),
                        prompt = s.prompt,
                        targetPartName = s.targetPartName,
                        keyTerms = s.keyTerms,
                    });
            def.learn = learn;

            // Practical
            var practical = new PracticalTask { title = m.practical?.title ?? "Practical" };
            if (m.practical?.steps != null)
                foreach (var s in m.practical.steps)
                    practical.steps.Add(new PracticalStep
                    {
                        kind = ParseEnum(s.kind, PracticalStepKind.IdentifyPart),
                        instruction = s.instruction,
                        correctKey = s.correctKey,
                        weight = s.weight,
                        hint = s.hint,
                    });
            def.practical = practical;

            // Quiz (reuses the existing QuizData ScriptableObject type)
            if (m.quiz != null)
            {
                var quiz = ScriptableObject.CreateInstance<QuizData>();
                quiz.quizName = m.quiz.quizName ?? "Quiz";
                quiz.questionsPerRound = m.quiz.questionsPerRound;
                quiz.pointsPerCorrectAnswer = m.quiz.pointsPerCorrectAnswer;
                if (m.quiz.questions != null)
                    foreach (var q in m.quiz.questions)
                        quiz.questions.Add(new QuestionData
                        {
                            questionText = q.questionText,
                            answerOptions = q.answerOptions != null
                                ? new System.Collections.Generic.List<string>(q.answerOptions)
                                : new System.Collections.Generic.List<string>(),
                            correctAnswerIndex = q.correctAnswerIndex,
                        });
                def.quiz = quiz;
            }

            return def;
        }

        static T ParseEnum<T>(string s, T fallback) where T : struct
            => !string.IsNullOrEmpty(s) && Enum.TryParse<T>(s, true, out var v) ? v : fallback;
    }
}

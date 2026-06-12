using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace APLab.Core
{
    public class RubricStepResult
    {
        [JsonProperty("step")] public int Step;
        [JsonProperty("kind")] public string Kind;
        [JsonProperty("instruction")] public string Instruction;
        [JsonProperty("awarded")] public float Awarded;   // 0..1
        [JsonProperty("weight")] public float Weight;
        [JsonProperty("attempts")] public int Attempts;
    }

    public class RubricResult
    {
        [JsonProperty("practical_score")] public float PracticalScore;  // 0..100
        [JsonProperty("quiz_score")] public float QuizScore;            // 0..100
        [JsonProperty("mastery_score")] public float MasteryScore;      // 0..1
        [JsonProperty("passed")] public bool Passed;
        [JsonProperty("steps")] public List<RubricStepResult> Steps = new List<RubricStepResult>();
    }

    /// <summary>
    /// Turns a practical rubric and a quiz fraction into a normalized mastery
    /// score using the weights declared on the ModuleDefinition.
    /// </summary>
    public static class ScoreModel
    {
        /// <param name="practicalStepScores">awarded 0..1 per practical step, in order</param>
        /// <param name="quizFraction">0..1 fraction of quiz correct</param>
        public static RubricResult Compute(ModuleDefinition def, IList<float> practicalStepScores, float quizFraction)
        {
            var result = new RubricResult();

            float practicalFraction = 0f;
            if (def.practical != null && def.practical.steps != null && def.practical.steps.Count > 0 &&
                practicalStepScores != null && practicalStepScores.Count > 0)
            {
                float totalWeight = 0f, awardedWeight = 0f;
                int n = Mathf.Min(def.practical.steps.Count, practicalStepScores.Count);
                for (int i = 0; i < n; i++)
                {
                    var step = def.practical.steps[i];
                    float w = Mathf.Max(0f, step.weight);
                    float a = Mathf.Clamp01(practicalStepScores[i]);
                    totalWeight += w;
                    awardedWeight += w * a;
                    result.Steps.Add(new RubricStepResult
                    {
                        Step = i, Kind = step.kind.ToString(), Instruction = step.instruction,
                        Awarded = a, Weight = w
                    });
                }
                practicalFraction = totalWeight > 0f ? awardedWeight / totalWeight : 0f;
            }

            quizFraction = Mathf.Clamp01(quizFraction);

            float pw = Mathf.Max(0f, def.practicalWeight);
            float qw = Mathf.Max(0f, def.quizWeight);
            float wsum = pw + qw;
            if (wsum <= 0f) { pw = qw = 0.5f; wsum = 1f; }

            float mastery = (practicalFraction * pw + quizFraction * qw) / wsum;

            result.PracticalScore = practicalFraction * 100f;
            result.QuizScore = quizFraction * 100f;
            result.MasteryScore = mastery;
            result.Passed = mastery >= def.passThreshold;
            return result;
        }
    }
}

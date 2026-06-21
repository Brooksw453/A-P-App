using System;
using System.Collections.Generic;
using UnityEngine;

namespace APLab.Core
{
    public enum APUnit { AP1, AP2 }

    /// <summary>
    /// One simulation. Data-only definition that the editor generator turns into
    /// a playable scene, and that ModuleRunner drives at runtime. Adding a sim =
    /// authoring one of these + dropping in a model, not hand-building a scene.
    /// </summary>
    [CreateAssetMenu(fileName = "Module", menuName = "A&P Lab/Module Definition")]
    public class ModuleDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string slug = "skeletal-system";
        public string title = "Skeletal System";
        public APUnit unit = APUnit.AP1;

        [Header("Supabase mapping (must match the online course for dashboards)")]
        [Tooltip("Course slug as used in section_progress.course_id / quiz_attempts.course_id")]
        public string courseId = "";
        [Tooltip("Leave at -1 if this module is not bound to a specific textbook chapter")]
        public int chapterId = -1;
        public string sectionId = "";

        [Header("Presentation")]
        [TextArea(2, 4)] public string summary;
        public int estimatedMinutes = 15;
        public Color systemColor = new Color(0.85f, 0.85f, 0.9f);

        [Header("Content")]
        public AnatomyModelDef[] models;
        public LabelSet labels;
        public LearnSequence learn;
        public PracticalTask practical;
        [Tooltip("Reuses the existing QuizData ScriptableObject for the Assess phase")]
        public QuizData quiz;

        [Header("Scoring weights (sum need not equal 1; normalized at runtime)")]
        [Range(0f, 1f)] public float practicalWeight = 0.6f;
        [Range(0f, 1f)] public float quizWeight = 0.4f;
        [Tooltip("Mastery (0..1) at or above this counts as passed")]
        [Range(0f, 1f)] public float passThreshold = 0.7f;

        public bool HasChapter => chapterId >= 0;
        public bool IsSectionMapped => !string.IsNullOrEmpty(courseId) && HasChapter && !string.IsNullOrEmpty(sectionId);
    }

    [Serializable]
    public class AnatomyModelDef
    {
        public string displayName;
        public GameObject prefab;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public float scale = 1f;
        [Tooltip("Child object names that can be exploded outward in the Learn phase")]
        public string[] explodeParts;
        public float explodeDistance = 0.15f;
    }

    /// <summary>A list of labelled anatomy terms anchored to named parts of a model.</summary>
    [CreateAssetMenu(fileName = "LabelSet", menuName = "A&P Lab/Label Set")]
    public class LabelSet : ScriptableObject
    {
        public string forModelName;
        public List<AnatomyLabel> labels = new List<AnatomyLabel>();
    }

    [Serializable]
    public class AnatomyLabel
    {
        [Tooltip("Name of the child transform (e.g. bone) this label points to")]
        public string anchorName;
        public string term;
        [TextArea(1, 3)] public string definition;
        public string pronunciation;
        [Tooltip("Key bony landmarks / features to call out on the Explore info panel")]
        public List<string> landmarks = new List<string>();
        [Tooltip("Related / adjacent bones to cross-reference ('see also') on the info panel")]
        public List<string> related = new List<string>();
        public AudioClip narration;
        [Tooltip("Optional offset for the label leader-line endpoint")]
        public Vector3 anchorOffset;
    }

    [Serializable]
    public class LearnSequence
    {
        public List<LearnStep> steps = new List<LearnStep>();
    }

    public enum LearnStepKind { FocusPart, RevealLabels, PlayNarration, PlayVideo, KeyTermCheckpoint }

    [Serializable]
    public class LearnStep
    {
        public LearnStepKind kind;
        [TextArea(1, 3)] public string prompt;
        public string targetPartName;
        public AudioClip narration;
        public UnityEngine.Video.VideoClip video;
        [Tooltip("For KeyTermCheckpoint: the terms the learner must recall/identify")]
        public string[] keyTerms;
    }

    /// <summary>A scored, ordered set of hands-on steps making up the practical.</summary>
    [Serializable]
    public class PracticalTask
    {
        public string title = "Practical";
        public List<PracticalStep> steps = new List<PracticalStep>();
    }

    public enum PracticalStepKind { IdentifyPart, PlaceInSocket, OrderSequence, SetValue, TracePath }

    [Serializable]
    public class PracticalStep
    {
        public PracticalStepKind kind;
        [TextArea(1, 3)] public string instruction;
        [Tooltip("Correct answer key: a part name, a socket id, an ordering, or a numeric target depending on kind")]
        public string correctKey;
        [Tooltip("Acceptable +/- tolerance for SetValue steps")]
        public float tolerance = 0.05f;
        [Tooltip("Relative weight of this step within the practical")]
        public float weight = 1f;
        [TextArea(1, 2)] public string hint;
    }
}

// A&P Lab — edit-mode sanity check for the content loader.
// Menu: "A&P Lab/Validate Skeletal Content" — parses skeletal-system.json through
// ModuleContentLoader and logs the resulting ModuleDefinition structure, so the
// loader can be verified without entering Play mode (which would domain-reload).

using System.IO;
using UnityEngine;
using UnityEditor;
using APLab.Content;

public static class ContentValidator
{
    const string ContentPath = "APLab/Content/skeletal-system.json"; // under Assets/

    [MenuItem("A&P Lab/Validate Skeletal Content")]
    public static void Validate()
    {
        var path = Path.Combine(Application.dataPath, ContentPath);
        if (!File.Exists(path)) { Debug.LogError("[A&P Lab] not found: Assets/" + ContentPath); return; }

        try
        {
            var def = ModuleContentLoader.Build(File.ReadAllText(path));
            int labels = def.labels != null ? def.labels.labels.Count : 0;
            int learn = def.learn != null ? def.learn.steps.Count : 0;
            int practical = def.practical != null ? def.practical.steps.Count : 0;
            int quiz = def.quiz != null ? def.quiz.questions.Count : 0;

            Debug.Log($"[A&P Lab] Content OK — '{def.title}' ({def.slug}) unit={def.unit} " +
                      $"course={def.courseId} ch={def.chapterId} §{def.sectionId} | " +
                      $"labels={labels} learn={learn} practical={practical} quiz={quiz}");

            if (def.practical != null)
                foreach (var s in def.practical.steps)
                    Debug.Log($"    practical [{s.kind}] correctKey='{s.correctKey}' w={s.weight}");

            Object.DestroyImmediate(def.labels);
            if (def.quiz != null) Object.DestroyImmediate(def.quiz);
            Object.DestroyImmediate(def);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[A&P Lab] content validation FAILED: " + e);
        }
    }
}

// A&P Lab — Quiz panel builder.
// Builds the world-space Assess-phase quiz UI as STATIC scene objects in the open
// Module_Skeletal scene, and wires it into the ModuleHost. Run via menu:
// "A&P Lab/Build Quiz Panel".  Re-runnable: rebuilds the "Quiz Panel" root.
//
// Why an editor command (not runtime): creating colliders at runtime under the XR
// rig corrupted the tracking origin ("skull flies up"). Everything pokeable is
// therefore authored here as scene geometry; QuizPanel only fills text + arms it.
//
// TMP is configured in code (font + autosize + ForceMeshUpdate) so it renders
// reliably — ad-hoc TMP created through the MCP bridge does not.
//
// Placement is tuned for the seated MR layout (skull ~[0,1.1,0.4] facing the user
// on the -Z side). The panel sits in front of the user, a bit nearer than the
// skull. Numbers are first-pass — nudge the "Quiz Panel" transform in the Editor
// and re-run, or just move it live. (ModuleHost hides the bone labels during the
// Assess phase so this panel is the focus.)

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using APLab.View;

public static class QuizPanelBuilder
{
    const string PanelName  = "Quiz Panel";
    const string BannerName = "Instruction Banner";
    // Fallback placement; ModuleHost moves the panel to the skull's spot at runtime.
    static readonly Vector3 PanelPos = new Vector3(0f, 1.15f, 0.45f);

    const float PanelWidth = 0.72f;
    const int   OptionSlots = 4;     // max answer options supported per question

    [MenuItem("A&P Lab/Build Quiz Panel")]
    public static void Build()
    {
        var old = GameObject.Find(PanelName);
        if (old != null) Object.DestroyImmediate(old);
        var oldBanner = GameObject.Find(BannerName);
        if (oldBanner != null) Object.DestroyImmediate(oldBanner);

        var unlit = Shader.Find("Universal Render Pipeline/Unlit");
        var font  = TMP_Settings.defaultFontAsset;

        var root = new GameObject(PanelName);
        Undo.RegisterCreatedObjectUndo(root, "Build Quiz Panel");
        root.transform.position = PanelPos;
        root.transform.rotation = Quaternion.identity;   // readable face toward the -Z user

        // Backboard (non-pokeable). Sits behind everything (+Z, away from the user).
        MakeQuad("Board", root.transform, new Vector3(0f, -0.05f, 0.02f),
            new Vector2(PanelWidth + 0.02f, 0.74f), new Color(0.06f, 0.07f, 0.10f, 1f), unlit);

        // Progress readout (top).
        var ptmp = MakeText("Progress", root.transform, new Vector3(0f, 0.255f, 0f),
            new Vector2(PanelWidth - 0.06f, 0.06f), 0.055f, TextAlignmentOptions.Center, font);
        ptmp.color = new Color(0.70f, 0.78f, 0.92f);
        ptmp.text = "Question 1 / 5";
        ptmp.ForceMeshUpdate();

        // Question prompt.
        var qtmp = MakeText("Question", root.transform, new Vector3(0f, 0.145f, 0f),
            new Vector2(PanelWidth - 0.05f, 0.20f), 0.11f, TextAlignmentOptions.Center, font);
        qtmp.text = "The question prompt appears here.";
        qtmp.ForceMeshUpdate();

        var panel = root.AddComponent<QuizPanel>();
        panel.questionText = qtmp;
        panel.progressText = ptmp;
        panel.options = new List<QuizOption>();

        // Answer buttons, stacked.
        const float btnW = 0.66f, btnH = 0.092f, gap = 0.018f, top = 0.0f;
        for (int i = 0; i < OptionSlots; i++)
        {
            float y = top - i * (btnH + gap);

            var btn = new GameObject($"Option {i}");
            btn.transform.SetParent(root.transform, false);
            btn.transform.localPosition = new Vector3(0f, y, 0f);

            var box = btn.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(btnW, btnH, 0.05f);

            var bg = MakeQuad("BG", btn.transform, new Vector3(0f, 0f, 0.012f),
                new Vector2(btnW, btnH), new Color(0.16f, 0.18f, 0.24f, 1f), unlit);

            var otmp = MakeText("Label", btn.transform, new Vector3(0f, 0f, 0f),
                new Vector2(btnW - 0.03f, btnH), 0.12f, TextAlignmentOptions.Center, font);
            otmp.text = $"Answer option {i + 1}";
            otmp.ForceMeshUpdate();

            var opt = btn.AddComponent<QuizOption>();
            opt.index = i;
            opt.label = otmp;
            opt.background = bg.GetComponent<MeshRenderer>();

            panel.options.Add(opt);
        }

        // Practice-phase instruction banner (a separate root object). ModuleHost shows it
        // during Practice, repositions it above the skull, and hides it for the quiz.
        var banner = MakeText(BannerName, null, new Vector3(0f, 1.5f, 0.45f),
            new Vector2(0.92f, 0.18f), 0.07f, TextAlignmentOptions.Center, font);
        banner.text = "Instruction appears here.";
        banner.ForceMeshUpdate();
        MakeQuad("Board", banner.transform, new Vector3(0f, 0f, 0.012f),
            new Vector2(0.96f, 0.21f), new Color(0.06f, 0.07f, 0.10f, 1f), unlit);

        // Wire the panel + banner into the ModuleHost so the phases find them.
        var host = Object.FindFirstObjectByType<ModuleHost>(FindObjectsInactive.Include);
        if (host != null)
        {
            host.quizPanel = panel;
            host.instructionText = banner;
            EditorUtility.SetDirty(host);
            Debug.Log("[A&P Lab] Quiz Panel + Instruction Banner wired into ModuleHost.");
        }
        else
        {
            Debug.LogWarning("[A&P Lab] No ModuleHost found — assign 'Quiz Panel'/'Instruction Banner' to ModuleHost manually.");
        }

        Selection.activeGameObject = root;
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[A&P Lab] Built '" + PanelName + "' with " + OptionSlots + " answer buttons.");
    }

    static GameObject MakeQuad(string name, Transform parent, Vector3 localPos, Vector2 size, Color color, Shader unlit)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.localPosition = localPos;
        quad.transform.localScale = new Vector3(size.x, size.y, 1f);
        var col = quad.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);   // visual only; not pokeable
        if (unlit != null)
        {
            var mat = new Material(unlit) { color = color };
            mat.SetFloat("_Cull", 0f);   // two-sided so it shows regardless of facing
            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        return quad;
    }

    static TextMeshPro MakeText(string name, Transform parent, Vector3 localPos, Vector2 size,
                                float fontSizeMax, TextAlignmentOptions align, TMP_FontAsset font)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;   // readable face toward the -Z user
        var tmp = go.AddComponent<TextMeshPro>();
        if (font != null) tmp.font = font;
        tmp.color = new Color(1f, 0.97f, 0.90f);
        tmp.alignment = align;
        tmp.rectTransform.sizeDelta = size;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = Mathf.Max(0.01f, fontSizeMax * 0.45f);
        tmp.fontSizeMax = fontSizeMax;
        tmp.fontSize = fontSizeMax;
        return tmp;
    }
}

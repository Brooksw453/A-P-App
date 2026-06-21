// A&P Lab — Lab UI builder.
// Builds the large back panel behind the skull as STATIC scene objects and wires them
// into a LabModeController. Run via menu "A&P Lab/Build Lab UI". Re-runnable: rebuilds
// the "Lab Back Panel" root and re-wires the controller.
//
// Three zones (the user's "3-zone back panel"):
//   • light-grey transparent MIDDLE backdrop (improves skull contrast in passthrough) +
//     a status line + the 3-button mode bar along the bottom
//   • darker transparent LEFT info panel  : term + description
//   • darker transparent RIGHT info panel : pronunciation + landmarks + related
//
// Why an editor command (not runtime): creating colliders at runtime under the XR rig
// corrupts the tracking origin ("skull flies up"). Everything pokeable (the mode buttons)
// is authored here as scene geometry; LabModeController only enables/disables + fills it.
//
// Transparency: URP/Unlit ignores alpha, so the glass panels use APLab/PanelGlass.
// Text orientation follows QuizPanelBuilder's convention exactly (identity rotation reads
// toward the -Z user; +Z is "behind", away from the user), so it matches the verified quiz UI.

using UnityEngine;
using UnityEditor;
using TMPro;
using APLab.View;

public static class LabUIBuilder
{
    const string RootName = "Lab Back Panel";

    // Placement (world): behind the skull (skull is at ~z 1.15; the old banner sat ~z 1.35).
    // ~0.5 m further back per the request. Tune live by moving the 'Lab Back Panel' root.
    static readonly Vector3 RootPos = new Vector3(0f, 1.30f, 1.85f);   // lowered so the +0.5 m grows downward

    const float MidW = 1.45f, MidH = 2.30f;     // middle backdrop (+0.5 m taller → buttons sit lower)
    const float SideW = 1.28f, SideH = 2.30f;   // each side info panel
    const float SideX = 1.42f;                  // side-panel centre offset from middle

    [MenuItem("A&P Lab/Build Lab UI")]
    public static void Build()
    {
        var old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        var glass = Shader.Find("APLab/PanelGlass") ?? Shader.Find("APLab/RayOverlay");
        var unlit = Shader.Find("Universal Render Pipeline/Unlit");
        var font  = TMP_Settings.defaultFontAsset;

        // Shared glass materials (never tinted at runtime, so sharing is fine).
        var middleMat = new Material(glass);
        middleMat.SetColor("_Color", new Color(0.80f, 0.80f, 0.86f, 0.16f));   // light-grey backdrop
        var sideMat = new Material(glass);
        sideMat.SetColor("_Color", new Color(0.04f, 0.05f, 0.08f, 0.55f));     // darker info panels

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Lab UI");
        root.transform.position = RootPos;
        root.transform.rotation = Quaternion.identity;

        // ---- middle backdrop + status line ----
        // NOTE: world-space metres at the panel's ~1.85 m distance. Autosize is clamped to [0.8*max, max]
        // (see MakeText), so text stays big and never shrinks to unreadable. All sizes + the panel
        // dimensions are live-tuning knobs. (The editor Scene camera sits CLOSE to the skull with the
        // panel BEHIND it, so text looks much smaller in-editor than it does on-device.)
        MakeGlassQuad("Backdrop (Middle)", root.transform, new Vector3(0f, 0f, 0.05f), new Vector2(MidW, MidH), middleMat);
        var status = MakeText("Status", root.transform, new Vector3(0f, 1.00f, 0f),
            new Vector2(MidW - 0.14f, 0.26f), 0.5f, TextAlignmentOptions.Center, font);
        status.color = new Color(0.72f, 0.80f, 0.95f);
        status.text = "EXPLORE";
        status.ForceMeshUpdate();

        // ---- left info panel: term + description ----
        var left = MakeSubRoot("Info Panel Left", root.transform, new Vector3(-SideX, 0f, 0f));
        MakeGlassQuad("BG", left, new Vector3(0f, 0f, 0.04f), new Vector2(SideW, SideH), sideMat);
        var leftTitle = MakeText("Term", left, new Vector3(0f, 0.86f, 0f),
            new Vector2(SideW - 0.12f, 0.55f), 0.5f, TextAlignmentOptions.Top, font);
        var leftBody = MakeText("Body", left, new Vector3(0f, -0.12f, 0f),
            new Vector2(SideW - 0.12f, 1.42f), 0.5f, TextAlignmentOptions.TopLeft, font);

        // ---- right info panel: pronunciation + landmarks + related ----
        var right = MakeSubRoot("Info Panel Right", root.transform, new Vector3(SideX, 0f, 0f));
        MakeGlassQuad("BG", right, new Vector3(0f, 0f, 0.04f), new Vector2(SideW, SideH), sideMat);
        var rightPron = MakeText("Pronunciation", right, new Vector3(0f, 0.90f, 0f),
            new Vector2(SideW - 0.12f, 0.34f), 0.5f, TextAlignmentOptions.Top, font);
        rightPron.color = new Color(0.75f, 0.92f, 1f);
        var rightLandmarks = MakeText("Landmarks", right, new Vector3(0f, 0.26f, 0f),
            new Vector2(SideW - 0.12f, 0.95f), 0.5f, TextAlignmentOptions.TopLeft, font);
        var rightRelated = MakeText("Related", right, new Vector3(0f, -0.66f, 0f),
            new Vector2(SideW - 0.12f, 0.80f), 0.5f, TextAlignmentOptions.TopLeft, font);

        // ---- mode bar (bottom of the panel, lower + wider so it's easy to point at) ----
        var bar = MakeSubRoot("Mode Bar", root.transform, new Vector3(0f, -0.92f, 0f));
        var bExplore = MakeButton("Mode Button Explore",  bar, new Vector3(-0.86f, 0f, 0f), new Vector2(0.80f, 0.26f), "EXPLORE",   "Explore",  unlit, font);
        var bInfo    = MakeButton("Mode Button Info Quiz", bar, new Vector3( 0.00f, 0f, 0f), new Vector2(0.80f, 0.26f), "INFO QUIZ", "InfoQuiz", unlit, font);
        var bBone    = MakeButton("Mode Button Bone Quiz", bar, new Vector3( 0.86f, 0f, 0f), new Vector2(0.80f, 0.26f), "BONE QUIZ", "BoneQuiz", unlit, font);

        // ---- wire the controller (co-located on the Module Host object) ----
        var host = Object.FindFirstObjectByType<ModuleHost>(FindObjectsInactive.Include);
        if (host == null)
        {
            Debug.LogWarning("[A&P Lab] Build Lab UI: no ModuleHost found — open Module_Skeletal and ensure 'Module Host' exists.");
        }
        else
        {
            var ctrl = host.GetComponent<LabModeController>() ?? Undo.AddComponent<LabModeController>(host.gameObject);
            ctrl.host = host;
            ctrl.exploreButton = bExplore;
            ctrl.infoQuizButton = bInfo;
            ctrl.boneQuizButton = bBone;
            ctrl.leftTitle = leftTitle;
            ctrl.leftBody = leftBody;
            ctrl.rightPron = rightPron;
            ctrl.rightLandmarks = rightLandmarks;
            ctrl.rightRelated = rightRelated;
            ctrl.statusText = status;
            ctrl.infoPanelLeft = left;
            ctrl.infoPanelRight = right;

            ctrl.skullRoot     = FindXform("Skull (Exploding)");
            ctrl.labelsRoot    = FindXform("Bone Labels");
            ctrl.explodeSlider = FindXform("Explode Slider");
            ctrl.rotateControl = FindXform("Rotate Control");
            ctrl.quizPanel     = Object.FindFirstObjectByType<QuizPanel>(FindObjectsInactive.Include);

            WarnIfNull(ctrl.skullRoot, "Skull (Exploding)");
            WarnIfNull(ctrl.labelsRoot, "Bone Labels");
            WarnIfNull(ctrl.explodeSlider, "Explode Slider");
            WarnIfNull(ctrl.rotateControl, "Rotate Control");

            ctrl.backPanelRoot = root.transform;   // Info Quiz panel is moved to this depth

            // Robust refs (work even when the objects are inactive on a re-run): prefer the host's
            // serialized refs. Disable the quiz panel + the old instruction banner BY DEFAULT in the
            // scene — the controller shows the quiz only on INFO QUIZ, and the banner is retired.
            if (ctrl.quizPanel == null && host.quizPanel != null) ctrl.quizPanel = host.quizPanel;
            if (ctrl.quizPanel == null)
                Debug.LogWarning("[A&P Lab] Build Lab UI: no QuizPanel found (Info Quiz won't run). Run 'A&P Lab/Build Quiz Panel' first.");
            else
                ctrl.quizPanel.gameObject.SetActive(false);

            var bannerTmp = host.instructionText;
            if (bannerTmp == null) { var bgo = GameObject.Find("Instruction Banner"); if (bgo != null) bannerTmp = bgo.GetComponent<TextMeshPro>(); }
            if (bannerTmp != null) { ctrl.instructionBanner = bannerTmp.transform; bannerTmp.gameObject.SetActive(false); }

            // Boot into Explore (not the linear rubric); pull the now-vestigial banner ~0.5 m back.
            var so = new SerializedObject(host);
            var auto = so.FindProperty("autoStartOnPlay");
            if (auto != null) auto.boolValue = false;
            var bo = so.FindProperty("bannerOffset");
            if (bo != null) bo.vector3Value = new Vector3(0f, 0.65f, 0.70f);
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(ctrl);
            EditorUtility.SetDirty(host);
            Debug.Log("[A&P Lab] Lab UI wired into LabModeController on '" + host.gameObject.name + "' (autoStartOnPlay = false).");
        }

        Selection.activeGameObject = root;
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[A&P Lab] Built '" + RootName + "' (back panel + split info panels + 3-button mode bar).");
    }

    // ---- helpers ----
    static Transform MakeSubRoot(string name, Transform parent, Vector3 localPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        return go.transform;
    }

    static GameObject MakeGlassQuad(string name, Transform parent, Vector3 localPos, Vector2 size, Material mat)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.localPosition = localPos;
        quad.transform.localScale = new Vector3(size.x, size.y, 1f);
        var col = quad.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);   // visual only; not pokeable
        quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
        return quad;
    }

    static GameObject MakeOpaqueQuad(string name, Transform parent, Vector3 localPos, Vector2 size, Color color, Shader unlit)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.localPosition = localPos;
        quad.transform.localScale = new Vector3(size.x, size.y, 1f);
        var col = quad.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);
        if (unlit != null)
        {
            var mat = new Material(unlit) { color = color };   // own instance per button (ModeButton tints it)
            mat.SetFloat("_Cull", 0f);                          // two-sided
            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        return quad;
    }

    static ModeButton MakeButton(string name, Transform parent, Vector3 localPos, Vector2 size,
                                 string labelText, string modeId, Shader unlit, TMP_FontAsset font)
    {
        var btn = new GameObject(name);
        btn.transform.SetParent(parent, false);
        btn.transform.localPosition = localPos;
        btn.transform.localRotation = Quaternion.identity;

        var box = btn.AddComponent<BoxCollider>();   // the ONLY collider (built in-editor; never at runtime)
        box.isTrigger = true;
        box.size = new Vector3(size.x, size.y, 0.05f);

        var bg = MakeOpaqueQuad("BG", btn.transform, new Vector3(0f, 0f, 0.012f), size,
            new Color(0.16f, 0.18f, 0.24f, 1f), unlit);

        var tmp = MakeText("Label", btn.transform, Vector3.zero, new Vector2(size.x - 0.05f, size.y - 0.02f),
            0.5f, TextAlignmentOptions.Center, font);
        tmp.text = labelText;
        tmp.ForceMeshUpdate();

        var mb = btn.AddComponent<ModeButton>();
        mb.modeId = modeId;
        mb.label = tmp;
        mb.background = bg.GetComponent<MeshRenderer>();
        return mb;
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
        // High max => autosize fills the box (short text gets big, like Brooks's 0.5 button tweak);
        // low floor lets the long description paragraph shrink to fit instead of overflowing.
        tmp.fontSizeMin = Mathf.Min(0.14f, fontSizeMax);
        tmp.fontSizeMax = fontSizeMax;
        tmp.fontSize = fontSizeMax;
        return tmp;
    }

    static Transform FindXform(string name) { var go = GameObject.Find(name); return go != null ? go.transform : null; }
    static void WarnIfNull(Object o, string name)
    {
        if (o == null) Debug.LogWarning($"[A&P Lab] Build Lab UI: scene object '{name}' not found — assign LabModeController's ref manually in the Inspector.");
    }
}

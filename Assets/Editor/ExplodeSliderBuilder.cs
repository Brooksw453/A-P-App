// A&P Lab — builds the user-facing EXPLODE SLIDER as static scene objects.
// Menu: "A&P Lab/Build Explode Slider" (run after "A&P Lab/Setup Exploding Skull").
//
// Creates a track + fill + handle + label below the skull, adds a PokeSlider with a
// poke-friendly trigger zone, and wires it to the scene's SkullExploder so the learner
// can open/close the skull with their finger. Built in the editor (never at runtime
// under the XR rig). Re-runnable: rebuilds the "Explode Slider" root.

using UnityEngine;
using UnityEditor;
using TMPro;
using APLab.View;

public static class ExplodeSliderBuilder
{
    const string RootName = "Explode Slider";
    const float Half = 0.15f;   // half track length (m) -> 30 cm slider

    [MenuItem("A&P Lab/Build Explode Slider")]
    public static void Build()
    {
        var exploder = Object.FindFirstObjectByType<SkullExploder>(FindObjectsInactive.Include);
        if (exploder == null)
        {
            Debug.LogError("[APLab Slider] No SkullExploder in the scene — run A&P Lab/Setup Exploding Skull first.");
            return;
        }

        var old = GameObject.Find(RootName);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var unlit = Shader.Find("Universal Render Pipeline/Unlit");
        // below + slightly toward the user (-Z) from the skull, so it doesn't block the bones
        Vector3 pos = exploder.transform.position + new Vector3(0f, -0.30f, -0.12f);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Explode Slider");
        root.transform.position = pos;
        root.transform.rotation = Quaternion.identity;   // track along world X, faces the user (-Z)

        var track  = CreateBar("Track", root.transform, unlit, new Color(0.25f, 0.25f, 0.28f),
                               new Vector3(Half * 2f, 0.012f, 0.012f), new Vector3(0f, 0f, 0f));
        var fill   = CreateBar("Fill",  root.transform, unlit, new Color(0.35f, 1f, 0.45f),
                               new Vector3(Half * 2f, 0.013f, 0.013f), new Vector3(0f, 0f, -0.003f));

        var handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        handle.name = "Handle";
        Undo.RegisterCreatedObjectUndo(handle, "Build Explode Slider");
        handle.transform.SetParent(root.transform, false);
        handle.transform.localScale = Vector3.one * 0.03f;
        handle.transform.localPosition = new Vector3(Half, 0f, -0.006f);
        var hcol = handle.GetComponent<Collider>(); if (hcol != null) Object.DestroyImmediate(hcol);
        if (unlit != null)
            handle.GetComponent<MeshRenderer>().sharedMaterial = new Material(unlit) { color = new Color(1f, 0.85f, 0.3f) };

        // label above the track
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(root.transform, false);
        labelGo.transform.localPosition = new Vector3(0f, 0.055f, 0f);
        var tmp = labelGo.AddComponent<TextMeshPro>();
        var font = TMP_Settings.defaultFontAsset; if (font != null) tmp.font = font;
        tmp.text = "← assemble        explode →";
        tmp.fontSize = 0.085f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.97f, 0.85f);
        tmp.rectTransform.sizeDelta = new Vector2(0.5f, 0.07f);
        tmp.ForceMeshUpdate();
        labelGo.AddComponent<LabelBillboard>();

        // poke zone (the ONLY collider) + the slider behaviour
        var box = root.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = Vector3.zero;
        box.size = new Vector3(Half * 2f + 0.04f, 0.06f, 0.06f);   // poke-friendly along the track

        var ps = root.AddComponent<PokeSlider>();
        ps.exploder = exploder;
        ps.handle = handle.transform;
        ps.fill = fill.transform;
        ps.halfLength = Half;
        ps.SetValue(Mathf.Clamp01(exploder.factor));   // place handle/fill to match current explode

        Selection.activeGameObject = root;
        MarkDirty();
        Debug.Log($"[APLab Slider] Built '{RootName}' at {pos} driving the SkullExploder " +
                  $"(value {ps.value:0.00}). Poke + slide along it to open/close the skull. " +
                  "Reposition the root live if you want it elsewhere.");
    }

    static Transform CreateBar(string name, Transform parent, Shader unlit, Color c, Vector3 scale, Vector3 localPos)
    {
        var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        Undo.RegisterCreatedObjectUndo(bar, "Build Explode Slider");
        bar.transform.SetParent(parent, false);
        bar.transform.localScale = scale;
        bar.transform.localPosition = localPos;
        var col = bar.GetComponent<Collider>(); if (col != null) Object.DestroyImmediate(col);
        if (unlit != null) bar.GetComponent<MeshRenderer>().sharedMaterial = new Material(unlit) { color = c };
        return bar.transform;
    }

    static void MarkDirty()
    {
        var s = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (s.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(s);
    }
}

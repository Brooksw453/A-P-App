// A&P Lab — builds the TURNTABLE rotate control (spin + tilt sliders) as static scene
// objects. Menu: "A&P Lab/Build Rotate Control" (run after "A&P Lab/Setup Exploding Skull").
//
// Adds a SkullRotator to the skull root and builds two poke-draggable sliders below the
// explode slider — one for turntable SPIN (yaw), one for TILT (pitch) — so the learner can
// turn the exploded skull to see every side, the top, and the base. Mirrors the explode
// slider exactly (the same PokeSlider widget the HandRaySelector already pinch-drags), so it
// needs NO changes to the ray input and never fights bone selection. Built in the editor,
// never spawned at runtime under the XR rig. Re-runnable: rebuilds the "Rotate Control" root.

using UnityEngine;
using UnityEditor;
using TMPro;
using APLab.View;

public static class RotateControlBuilder
{
    const string RootName = "Rotate Control";
    const float Half = 0.15f;   // half track length (m) -> 30 cm slider, same as the explode slider

    [MenuItem("A&P Lab/Build Rotate Control")]
    public static void Build()
    {
        // The SkullExploder lives on the skull root — use it to find the skull (same as the
        // explode-slider builder), and require it so we never build orphaned controls.
        var exploder = Object.FindFirstObjectByType<SkullExploder>(FindObjectsInactive.Include);
        if (exploder == null)
        {
            Debug.LogError("[APLab Rotate] No SkullExploder in the scene — run A&P Lab/Setup Exploding Skull first.");
            return;
        }
        var skull = exploder.transform;

        // --- add / refresh the SkullRotator on the skull root, capturing the rest pose ---
        var rotator = skull.GetComponent<SkullRotator>();
        if (rotator == null) rotator = Undo.AddComponent<SkullRotator>(skull.gameObject);
        Undo.RecordObject(rotator, "Build Rotate Control");
        Undo.RecordObject(skull, "Build Rotate Control");
        if (rotator.homeSet) skull.localRotation = rotator.homeRotation;   // neutralise a prior offset...
        rotator.CaptureHome();                                            // ...then capture the true rest pose
        rotator.spin = 0f; rotator.tilt = 0f;
        rotator.Apply();

        var old = GameObject.Find(RootName);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var unlit = Shader.Find("Universal Render Pipeline/Unlit");

        // Anchor the rotate sliders directly BENEATH the existing explode slider, so all three form
        // one clean reachable column — pulled toward the user, out of the exploded-bone cloud (the
        // bones were blocking the rotate sliders when they sat right under the skull). Fall back to a
        // skull-relative spot (≈ the explode slider's) if the explode slider isn't found.
        PokeSlider explode = null;
        foreach (var ps in Object.FindObjectsByType<PokeSlider>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (ps.drive == PokeSlider.Drive.Exploder) { explode = ps; break; }
        Vector3 anchorPos = explode != null ? explode.transform.position
                                            : skull.position + new Vector3(0f, -0.30f, -0.12f);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Rotate Control");
        root.transform.position = anchorPos;
        root.transform.rotation = Quaternion.identity;   // tracks along world X, facing the user (-Z)

        // stacked just below the explode slider (≈13 cm apart), same depth/plane
        BuildSlider(root.transform, unlit, "Spin Slider", new Vector3(0f, -0.13f, 0f),
                    PokeSlider.Drive.RotatorSpin, rotator, "◄   spin   ►",
                    Mathf.InverseLerp(-180f, 180f, rotator.spin));
        BuildSlider(root.transform, unlit, "Tilt Slider", new Vector3(0f, -0.26f, 0f),
                    PokeSlider.Drive.RotatorTilt, rotator, "◄   tilt   ►",
                    Mathf.InverseLerp(rotator.tiltMin, rotator.tiltMax, rotator.tilt));

        Selection.activeGameObject = root;
        MarkDirty();
        Debug.Log($"[APLab Rotate] Built '{RootName}' (spin + tilt) driving a SkullRotator on " +
                  $"'{skull.name}'. Pinch-drag to turn the exploded skull; centre = front-facing. " +
                  "Reposition the root live if you want the controls elsewhere. " +
                  "Tuning: SkullRotator.tiltMin/tiltMax (swap them to invert the tilt direction).");
    }

    // One poke-draggable slider (track + fill + handle + label + trigger zone), wired to drive
    // the rotator. Mirrors ExplodeSliderBuilder so the HandRaySelector drives it identically.
    static void BuildSlider(Transform parent, Shader unlit, string name, Vector3 localPos,
                            PokeSlider.Drive drive, SkullRotator rotator, string labelText, float value01)
    {
        var sroot = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(sroot, "Build Rotate Control");
        sroot.transform.SetParent(parent, false);
        sroot.transform.localPosition = localPos;
        sroot.transform.localRotation = Quaternion.identity;

        CreateBar("Track", sroot.transform, unlit, new Color(0.25f, 0.25f, 0.28f),
                  new Vector3(Half * 2f, 0.012f, 0.012f), Vector3.zero);
        var fill = CreateBar("Fill", sroot.transform, unlit, new Color(0.45f, 0.7f, 1f),
                             new Vector3(Half * 2f, 0.013f, 0.013f), new Vector3(0f, 0f, -0.003f));

        var handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        handle.name = "Handle";
        Undo.RegisterCreatedObjectUndo(handle, "Build Rotate Control");
        handle.transform.SetParent(sroot.transform, false);
        handle.transform.localScale = Vector3.one * 0.03f;
        handle.transform.localPosition = new Vector3(0f, 0f, -0.006f);   // x is set by PokeSlider.SetValue
        var hcol = handle.GetComponent<Collider>(); if (hcol != null) Object.DestroyImmediate(hcol);
        if (unlit != null)
            handle.GetComponent<MeshRenderer>().sharedMaterial = new Material(unlit) { color = new Color(0.4f, 0.85f, 1f) };

        // label above the track
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(sroot.transform, false);
        labelGo.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        var tmp = labelGo.AddComponent<TextMeshPro>();
        var font = TMP_Settings.defaultFontAsset; if (font != null) tmp.font = font;
        tmp.text = labelText;
        tmp.fontSize = 0.13f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.85f, 0.95f, 1f);
        tmp.rectTransform.sizeDelta = new Vector2(0.75f, 0.11f);
        tmp.ForceMeshUpdate();
        labelGo.AddComponent<LabelBillboard>();

        // poke zone (the ONLY collider) + the slider behaviour
        var box = sroot.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = Vector3.zero;
        box.size = new Vector3(Half * 2f + 0.04f, 0.06f, 0.06f);   // poke-friendly along the track

        var ps = sroot.AddComponent<PokeSlider>();
        ps.drive = drive;
        ps.rotator = rotator;
        ps.handle = handle.transform;
        ps.fill = fill.transform;
        ps.halfLength = Half;
        ps.SetValue(Mathf.Clamp01(value01));   // place handle/fill to match the current angle
    }

    static Transform CreateBar(string name, Transform parent, Shader unlit, Color c, Vector3 scale, Vector3 localPos)
    {
        var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        Undo.RegisterCreatedObjectUndo(bar, "Build Rotate Control");
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

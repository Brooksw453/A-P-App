// A&P Lab — one-click setup of the interactive EXPLODING skull (Phase A).
// Menu: "A&P Lab/Setup Exploding Skull".
//
// Builds on Assets/Explode 2.fbx (the clean per-bone model). In the open
// Module_Skeletal scene it:
//   1. instantiates the FBX, wraps it under a pivot-corrected root "Skull (Exploding)"
//      placed + scaled where the old solid skull sat,
//   2. disables the old solid "Skull" + the old "Skeletal Labels" markers (reversible),
//   3. maps each bone mesh to its JSON anchorName and stamps a pokeable BoneTarget
//      (mesh mode) + trigger BoxCollider on the bone itself — the bone IS the target,
//   4. adds a SkullExploder and separates every mesh radially (de-clustered + the
//      internal/posterior bones become reachable),
//   5. wires the ModuleHost's modelRoot + labelsRoot to the new skull.
//
// Re-runnable: rebuilds "Skull (Exploding)" from scratch each run. Phase B (re-home the
// TMP labels onto the separated bones) and Phase C (place-the-bone-in-socket) build on
// this. Nothing is saved until you save the scene; Undo reverts it.

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using APLab.View;

public static class ExplodingSkullSetup
{
    const string RootName = "Skull (Exploding)";
    const float TargetHeight = 0.75f;    // ~2x the original ~0.377 m
    // Explicit comfortable spot — ray interaction removes the reach constraint, so the skull
    // can sit further out + a bit higher (was "in the user's lap"). Tune + re-run / tell me.
    static readonly Vector3 SkullPos = new Vector3(0f, 1.30f, 1.15f);

    // bone-node name (lowercased, substring) -> exact JSON anchorName. Order matters only
    // where one substring could shadow another; "teeth" is excluded up front (visual only).
    static readonly (string key, string anchor)[] Map =
    {
        ("ethmoid",          "Ethmoid bone"),
        ("frontal",          "Frontal bone"),
        ("inferior_conchae", "Inferior nasal concha"),
        ("conchae",          "Inferior nasal concha"),
        ("lacrimal",         "Lacrimal bone"),
        ("maxilla",          "Maxilla"),
        ("_max",             "Maxilla"),            // "Right_max:STL..."
        ("nasal",            "Nasal bone"),
        ("palatine",         "Palatine bone"),
        ("parietal",         "Parietal bone"),
        ("temporal",         "Temporal bone"),
        ("zygomatic",        "Zygomatic bone"),
        ("occipital",        "Occipital bone"),
        ("sphenoid",         "Sphenoid bone"),
        ("vomer",            "Vomer"),
        ("mandible",         "Mandible"),
    };

    static readonly string[] AllAnchors =
    {
        "Frontal bone","Parietal bone","Temporal bone","Sphenoid bone","Zygomatic bone",
        "Maxilla","Occipital bone","Nasal bone","Ethmoid bone","Lacrimal bone",
        "Inferior nasal concha","Vomer","Palatine bone","Mandible"
    };

    static string MapAnchor(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var n = name.ToLowerInvariant();
        if (n.Contains("teeth")) return null;          // upper/lower teeth: visual only
        foreach (var (key, anchor) in Map)
            if (n.Contains(key)) return anchor;
        return null;
    }

    [MenuItem("A&P Lab/Setup Exploding Skull")]
    public static void Setup()
    {
        // --- locate the FBX ---
        var fbxPath = "Assets/Explode 2.fbx";
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
        {
            foreach (var g in AssetDatabase.FindAssets("Explode t:Model"))
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("explode 2"))
                { fbx = AssetDatabase.LoadAssetAtPath<GameObject>(p); fbxPath = p; break; }
            }
        }
        if (fbx == null) { Debug.LogError("[APLab Skull] Explode 2.fbx not found in the project."); return; }

        // --- placement: prefer the current exploding skull (stable re-runs), else the old
        //     solid skull (first run), else a default. Size up to ~2x + push back per Brooks. ---
        var prior = GameObject.Find(RootName);
        var oldSkull = GameObject.Find("Skull");

        Quaternion rot; Transform parent;
        if (prior != null)        { rot = prior.transform.rotation;    parent = prior.transform.parent; }
        else if (oldSkull != null){ rot = oldSkull.transform.rotation; parent = oldSkull.transform.parent; }
        else                      { rot = Quaternion.identity;         parent = null; }

        Vector3 pos = SkullPos;            // explicit comfortable placement; keep prior facing
        float targetHeight = TargetHeight;

        // disable the old solid skull + old marker labels (first run only)
        if (oldSkull != null)
        {
            Undo.RecordObject(oldSkull, "Disable old skull");
            oldSkull.name = "Skull (old solid - disabled)";
            oldSkull.SetActive(false);
        }
        var oldLabels = GameObject.Find("Skeletal Labels");
        if (oldLabels != null) { Undo.RecordObject(oldLabels, "Disable old labels"); oldLabels.SetActive(false); }

        if (prior != null) Undo.DestroyObjectImmediate(prior);

        // --- pivot-corrected wrapper root ---
        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Setup Exploding Skull");
        if (parent != null) root.transform.SetParent(parent, false);
        root.transform.SetPositionAndRotation(pos, rot);
        root.transform.localScale = Vector3.one;

        var inst = (GameObject)UnityEngine.Object.Instantiate(fbx);
        inst.name = "model";
        inst.transform.SetParent(root.transform, true);
        inst.transform.localRotation = Quaternion.identity;
        inst.transform.localScale = Vector3.one;
        inst.transform.localPosition = Vector3.zero;

        // pivot fix: shift the model so its mesh centre sits on the root origin
        var b = CombinedBounds(inst);
        inst.transform.position -= (b.center - root.transform.position);

        // scale to the target height (root scales about its origin == the mesh centre)
        b = CombinedBounds(inst);
        float h = Mathf.Max(0.0001f, b.size.y);
        root.transform.localScale = Vector3.one * (targetHeight / h);

        // --- map bones -> BoneTarget (mesh mode) + trigger collider ---
        int mapped = 0;
        var gotAnchors = new HashSet<string>();
        foreach (var mr in inst.GetComponentsInChildren<MeshRenderer>(true))
        {
            var mf = mr.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            string anchor = MapAnchor(mr.name);
            if (anchor == null && mr.transform.parent != null) anchor = MapAnchor(mr.transform.parent.name);
            if (anchor == null) continue;

            var go = mr.gameObject;
            var box = go.GetComponent<BoxCollider>();
            if (box == null) box = Undo.AddComponent<BoxCollider>(go);
            box.center = mf.sharedMesh.bounds.center;
            box.size = mf.sharedMesh.bounds.size * 1.05f;   // small forgiveness margin
            box.isTrigger = true;

            var bt = go.GetComponent<BoneTarget>();
            if (bt == null) bt = Undo.AddComponent<BoneTarget>(go);
            bt.anchorName = anchor;
            bt.meshMode = true;

            mapped++; gotAnchors.Add(anchor);
        }

        // --- explode controller (every mesh fans out, so teeth etc. travel with their bone) ---
        var exploder = root.GetComponent<SkullExploder>();
        if (exploder == null) exploder = Undo.AddComponent<SkullExploder>(root);
        var center = CombinedBounds(inst).center;
        var parts = new List<SkullExploder.Part>();
        foreach (var mr in inst.GetComponentsInChildren<MeshRenderer>(true))
        {
            var t = mr.transform;
            if (t.parent == null) continue;
            Vector3 dirWorld = mr.bounds.center - center;
            Vector3 dirLocal = t.parent.InverseTransformVector(dirWorld);
            parts.Add(new SkullExploder.Part { t = t, home = t.localPosition, dir = dirLocal });
        }
        exploder.parts = parts;
        exploder.spread = 2f;        // Brooks's tuned value (2026-06-17)
        exploder.factor = 1f;        // start exploded so bones de-cluster + internal bones show
        exploder.Apply();

        // --- wire the ModuleHost ---
        var host = UnityEngine.Object.FindFirstObjectByType<ModuleHost>(FindObjectsInactive.Include);
        if (host != null)
        {
            var so = new SerializedObject(host);
            var mp = so.FindProperty("modelRoot");
            var lp = so.FindProperty("labelsRoot");
            if (mp != null) mp.objectReferenceValue = root.transform;
            if (lp != null) lp.objectReferenceValue = root.transform;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(host);
        }

        Selection.activeGameObject = root;
        MarkDirty();

        Debug.Log($"[APLab Skull] Exploding skull ready: {mapped} bone targets across " +
                  $"{gotAnchors.Count}/{AllAnchors.Length} anchors; {parts.Count} meshes exploding; " +
                  $"height {targetHeight:0.000} m; SkullExploder spread {exploder.spread}, factor {exploder.factor}. " +
                  (host != null ? "ModuleHost wired (modelRoot + labelsRoot)." : "⚠ No ModuleHost found."));
        var missing = new List<string>();
        foreach (var a in AllAnchors) if (!gotAnchors.Contains(a)) missing.Add(a);
        if (missing.Count > 0)
            Debug.LogWarning("[APLab Skull] anchors with NO bone mapped (check Explode 2.fbx node names): " +
                             string.Join(", ", missing));
    }

    static Bounds CombinedBounds(GameObject go)
    {
        var rs = go.GetComponentsInChildren<Renderer>(true);
        if (rs.Length == 0) return new Bounds(go.transform.position, Vector3.zero);
        var bounds = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) bounds.Encapsulate(rs[i].bounds);
        return bounds;
    }

    static void MarkDirty()
    {
        var s = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (s.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(s);
    }
}

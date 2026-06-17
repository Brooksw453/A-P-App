// A&P Lab — decimate the exploding-skull bone meshes for Quest performance.
// Menu: "A&P Lab/Decimate Skull" (run after "A&P Lab/Setup Exploding Skull").
//
// The raw scan meshes are ~1.13M triangles total — way over the Quest's comfortable
// per-frame budget (the cause of the framerate/glitching). This simplifies each bone mesh
// (quadric-error decimation via UnityMeshSimplifier) to QUALITY of its original triangles,
// saves the result as a new asset, swaps it onto the in-scene skull, and turns shadow
// casting off (a second free win).
//
// Idempotent: always decimates from the ORIGINAL FBX meshes (matched by node name), so
// re-running never compounds. Originals are never modified. Tweak QUALITY + re-run to taste.

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityMeshSimplifier;

public static class SkullDecimator
{
    const string FbxPath   = "Assets/Explode 2.fbx";
    const string OutFolder = "Assets/APLab/Generated/SkullDecimated";
    const float  Quality   = 0.30f;   // fraction of triangles to KEEP (~1.13M -> ~340k; smoother than 0.15). Tweak + re-run.

    [MenuItem("A&P Lab/Decimate Skull")]
    public static void Decimate()
    {
        var root = GameObject.Find("Skull (Exploding)");
        if (root == null)
        {
            Debug.LogError("[APLab Decimate] No 'Skull (Exploding)' in scene — run A&P Lab/Setup Exploding Skull first.");
            return;
        }

        // locate the FBX
        var fbxPath = FbxPath;
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
        {
            foreach (var g in AssetDatabase.FindAssets("Explode t:Model"))
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("explode 2"))
                { fbxPath = p; fbx = AssetDatabase.LoadAssetAtPath<GameObject>(p); break; }
            }
        }
        if (fbx == null) { Debug.LogError("[APLab Decimate] Explode 2.fbx not found."); return; }

        // editor decimation needs CPU-readable vertex data
        var importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
            fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        }

        // node name -> ORIGINAL mesh (decimate from these every run, never from a prior result)
        var orig = new Dictionary<string, Mesh>();
        foreach (var mf in fbx.GetComponentsInChildren<MeshFilter>(true))
            if (mf.sharedMesh != null) orig[mf.name] = mf.sharedMesh;

        EnsureFolder(OutFolder);

        long before = 0, after = 0; int done = 0, skipped = 0;
        try
        {
            var mfs = root.GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < mfs.Length; i++)
            {
                var mf = mfs[i];
                if (!orig.TryGetValue(mf.name, out var src) || src == null) { skipped++; continue; }

                EditorUtility.DisplayProgressBar("Decimate Skull", mf.name.Split(':')[0], (float)i / mfs.Length);

                before += Tris(src);
                var ms = new MeshSimplifier();
                ms.Initialize(src);
                ms.SimplifyMesh(Quality);
                var dec = ms.ToMesh();
                dec.name = Safe(mf.name) + "_dec";
                dec.RecalculateBounds();
                after += Tris(dec);

                string path = OutFolder + "/" + Safe(mf.name) + ".asset";
                if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null) AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(dec, path);
                mf.sharedMesh = dec;

                var mr = mf.GetComponent<MeshRenderer>();
                if (mr != null) { mr.shadowCastingMode = ShadowCastingMode.Off; mr.receiveShadows = false; }
                done++;
            }
        }
        finally { EditorUtility.ClearProgressBar(); }

        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(root);
        MarkDirty();

        double ratio = before > 0 ? (double)after / before : 0;
        Debug.Log($"[APLab Decimate] Decimated {done} meshes at quality {Quality:P0} (skipped {skipped}): " +
                  $"{before:n0} → {after:n0} triangles ({ratio:P0} of original). Shadow casting OFF. " +
                  $"Saved under {OutFolder}/. ► Save the scene (Ctrl+S) + rebuild to test on the Quest.");
    }

    static long Tris(Mesh m) { long t = 0; for (int i = 0; i < m.subMeshCount; i++) t += (long)m.GetIndexCount(i) / 3; return t; }

    static string Safe(string n)
    {
        var s = n.Split(':')[0];
        foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
        return s;
    }

    static void EnsureFolder(string path)
    {
        var parts = path.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }

    static void MarkDirty()
    {
        var s = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (s.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(s);
    }
}

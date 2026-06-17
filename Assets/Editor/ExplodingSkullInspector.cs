// A&P Lab — read-only inspector for the exploding-skull asset(s).
// Menu: "A&P Lab/Inspect Exploding Skull".
//
// Logs (and does NOT modify the scene or any asset) everything needed to plan the
// exploding-skull integration:
//   • the GameObject hierarchy + each mesh node's local position and mesh size
//   • combined world bounds (overall size/height) and the PIVOT OFFSET (mesh center
//     vs. the root pivot) — the known "offset pivot" quirk we have to correct
//   • any "...Socket" transforms (targets for the place-the-bone-in-socket step)
//   • components already present (BoneTarget / LabelInfo / Canvas / Animator / colliders)
//   • AnimationClips on the asset (name/length) + a sampled explode displacement
//
// Each candidate is instantiated in memory, inspected, then DestroyImmediate'd — nothing
// persists. All output is prefixed [APLab Inspect] so it can be grepped straight out of
// Editor.log without the Unity MCP bridge.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using APLab.View;

public static class ExplodingSkullInspector
{
    [MenuItem("A&P Lab/Inspect Exploding Skull")]
    public static void Inspect()
    {
        var paths = FindCandidatePaths();
        if (paths.Count == 0)
        {
            Debug.LogWarning("[APLab Inspect] No exploding-skull candidates found " +
                             "(searched 'Exploding_skull' prefabs + 'Explode 2' model).");
            return;
        }

        Debug.Log($"[APLab Inspect] ===== {paths.Count} candidate(s): {string.Join(", ", paths)} =====");
        foreach (var p in paths)
        {
            try { InspectAsset(p); }
            catch (Exception e) { Debug.LogError($"[APLab Inspect] FAILED on {p}: {e}"); }
        }
        Debug.Log("[APLab Inspect] ===== done (nothing was modified) =====");
    }

    static List<string> FindCandidatePaths()
    {
        var set = new List<string>();
        void Add(string p) { if (!string.IsNullOrEmpty(p) && !set.Contains(p)) set.Add(p); }

        foreach (var g in AssetDatabase.FindAssets("Exploding_skull t:Prefab"))
            Add(AssetDatabase.GUIDToAssetPath(g));
        foreach (var g in AssetDatabase.FindAssets("Explode t:Model"))
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            if (!string.IsNullOrEmpty(p) &&
                Path.GetFileNameWithoutExtension(p).ToLowerInvariant().Contains("explode 2"))
                Add(p);
        }
        return set;
    }

    static void InspectAsset(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (asset == null) { Debug.LogWarning($"[APLab Inspect] (not a GameObject) {path}"); return; }

        Debug.Log($"[APLab Inspect] ----- {path} -----");
        var inst = (GameObject)UnityEngine.Object.Instantiate(asset);
        try
        {
            // Pivot at origin so the combined mesh center IS the pivot offset.
            inst.transform.position = Vector3.zero;
            inst.transform.rotation = Quaternion.identity;

            var rends = inst.GetComponentsInChildren<Renderer>(true);
            if (rends.Length > 0)
            {
                var b = rends[0].bounds;
                for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
                Debug.Log($"[APLab Inspect] bounds size={Fmt(b.size)} (height {b.size.y:0.000} m), renderers={rends.Length}");
                Debug.Log($"[APLab Inspect] PIVOT OFFSET (mesh center, pivot at origin) = {Fmt(b.center)}  |mag|={b.center.magnitude:0.000} m");
            }
            else Debug.LogWarning("[APLab Inspect] no renderers.");

            // sockets (place-the-bone-in-socket targets)
            var sockets = new List<Transform>();
            foreach (var t in inst.GetComponentsInChildren<Transform>(true))
                if (t.name.IndexOf("socket", StringComparison.OrdinalIgnoreCase) >= 0) sockets.Add(t);
            if (sockets.Count > 0)
            {
                var sb = new StringBuilder($"[APLab Inspect] sockets ({sockets.Count}):\n");
                foreach (var s in sockets) sb.AppendLine($"    '{s.name}' world={Fmt(s.position)}");
                Debug.Log(sb.ToString());
            }

            // components already on the asset
            Debug.Log($"[APLab Inspect] components present: " +
                      $"BoneTarget={inst.GetComponentsInChildren<BoneTarget>(true).Length}, " +
                      $"LabelInfo={inst.GetComponentsInChildren<LabelInfo>(true).Length}, " +
                      $"Canvas={inst.GetComponentsInChildren<Canvas>(true).Length}, " +
                      $"Animator={inst.GetComponentsInChildren<Animator>(true).Length}, " +
                      $"Animation={inst.GetComponentsInChildren<Animation>(true).Length}, " +
                      $"Colliders={inst.GetComponentsInChildren<Collider>(true).Length}");

            // full hierarchy
            var h = new StringBuilder("[APLab Inspect] hierarchy (name | localPos | meshSize):\n");
            DumpHierarchy(inst.transform, 0, h);
            Debug.Log(h.ToString());

            // animation clips + measured explode displacement
            InspectClips(path, inst);
        }
        finally { UnityEngine.Object.DestroyImmediate(inst); }
    }

    static void DumpHierarchy(Transform t, int depth, StringBuilder sb)
    {
        var mf = t.GetComponent<MeshFilter>();
        var smr = t.GetComponent<SkinnedMeshRenderer>();
        string mesh = "";
        if (mf != null && mf.sharedMesh != null) mesh = $" | mesh {Fmt(mf.sharedMesh.bounds.size)}";
        else if (smr != null && smr.sharedMesh != null) mesh = $" | skinned {Fmt(smr.sharedMesh.bounds.size)}";
        sb.AppendLine($"  {new string(' ', depth * 2)}{t.name} | lp={Fmt(t.localPosition)}{mesh}");
        for (int i = 0; i < t.childCount; i++) DumpHierarchy(t.GetChild(i), depth + 1, sb);
    }

    static void InspectClips(string assetPath, GameObject inst)
    {
        var clips = new List<AnimationClip>();
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            if (o is AnimationClip c && !c.name.StartsWith("__preview")) clips.Add(c);
        foreach (var anim in inst.GetComponentsInChildren<Animator>(true))
            if (anim.runtimeAnimatorController != null)
                foreach (var c in anim.runtimeAnimatorController.animationClips)
                    if (c != null && !clips.Contains(c)) clips.Add(c);

        if (clips.Count == 0) { Debug.Log("[APLab Inspect] no AnimationClips on this asset."); return; }

        foreach (var clip in clips)
        {
            Debug.Log($"[APLab Inspect] clip '{clip.name}' length={clip.length:0.00}s rate={clip.frameRate} legacy={clip.legacy}");
            bool started = false;
            try
            {
                var nodes = inst.GetComponentsInChildren<Transform>(true);
                AnimationMode.StartAnimationMode(); started = true;
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(inst, clip, 0f);
                AnimationMode.EndSampling();
                var start = new Vector3[nodes.Length];
                for (int i = 0; i < nodes.Length; i++) start[i] = nodes[i].localPosition;
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(inst, clip, clip.length);
                AnimationMode.EndSampling();
                float maxMove = 0f; string mover = "";
                for (int i = 0; i < nodes.Length; i++)
                {
                    float d = Vector3.Distance(start[i], nodes[i].localPosition);
                    if (d > maxMove) { maxMove = d; mover = nodes[i].name; }
                }
                Debug.Log($"[APLab Inspect]    explode displacement over the clip: max {maxMove:0.000} m on '{mover}'");
            }
            catch (Exception e) { Debug.Log($"[APLab Inspect]    (sampling skipped: {e.Message})"); }
            finally { if (started) AnimationMode.StopAnimationMode(); }
        }
    }

    static string Fmt(Vector3 v) => $"({v.x:0.000},{v.y:0.000},{v.z:0.000})";

    // Triangle/vertex budget check: measures the in-scene exploding skull AND every
    // candidate asset, so we can see how far over the Quest budget we are and whether any
    // candidate is lighter. Quest comfort budget is ~0.75-1.0M triangles per FRAME total.
    [MenuItem("A&P Lab/Skull Poly Count")]
    public static void PolyCount()
    {
        var root = GameObject.Find("Skull (Exploding)");
        if (root != null) ReportPolys("IN-SCENE: Skull (Exploding)", root.GetComponentsInChildren<MeshFilter>(true));
        else Debug.LogWarning("[APLab Poly] No 'Skull (Exploding)' in scene (run A&P Lab/Setup Exploding Skull first).");

        foreach (var path in FindCandidatePaths())
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go != null) ReportPolys("ASSET: " + path, go.GetComponentsInChildren<MeshFilter>(true));
        }
        Debug.Log("[APLab Poly] (Quest comfort budget is ~0.75-1.0M triangles per frame, TOTAL across everything drawn.)");
    }

    static void ReportPolys(string label, MeshFilter[] mfs)
    {
        long tris = 0, verts = 0; int n = 0;
        var top = new List<(long t, string name)>();
        foreach (var mf in mfs)
        {
            var m = mf.sharedMesh; if (m == null) continue;
            long t = 0;
            for (int i = 0; i < m.subMeshCount; i++) t += (long)m.GetIndexCount(i) / 3;
            tris += t; verts += m.vertexCount; n++;
            top.Add((t, mf.name));
        }
        top.Sort((a, b) => b.t.CompareTo(a.t));
        var sb = new StringBuilder($"[APLab Poly] {label}\n   TOTAL {tris:n0} tris, {verts:n0} verts, {n} meshes. Heaviest:\n");
        for (int i = 0; i < top.Count && i < 6; i++) sb.AppendLine($"      {top[i].t,10:n0}  {top[i].name}");
        Debug.Log(sb.ToString());
    }
}

// A&P Lab — builds anatomical labels for the exploded skull's bones.
// Menu: "A&P Lab/Build Bone Labels" (run after "A&P Lab/Setup Exploding Skull").
//
// One billboarded TMP label per bone, content (term + pronunciation) from the shared
// Assets/APLab/Content/skeletal-system.json. Each label:
//   • sits OUTSIDE the skull (LabelFollow parks it on a fixed radial, ≥ minRadius from centre)
//     so it's readable even when the skull is fully assembled,
//   • carries a BLACK OUTLINE (on a COPY of the font material, so the slider/banner text is
//     untouched),
//   • has an always-on-top LEADER LINE pointing accurately at its bone (tracks it through the
//     explode).
//
// DECLUTTER: the facial bones all point ~forward, so their labels would stack on top of each
// other. We relax the label DIRECTIONS apart on the unit sphere (repulsion, clamped so a label
// never drifts more than MaxDriftDeg from its own bone's direction) — so they fan out and stay
// readable, while the leader lines keep each one tied to its bone.
//
// Labels live under a world-scale "Bone Labels" root (constant text size) and carry NO collider.
// Re-runnable. (Labels render in every phase — gating them off during Practice is a small follow-up.)

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using TMPro;
using APLab.View;

public static class BoneLabelBuilder
{
    const string RootName = "Bone Labels";
    const string SkullName = "Skull (Exploding)";
    const string ContentPath = "APLab/Content/skeletal-system.json";   // under Assets/
    const float FontSize = 0.16f;       // world-space TMP size (tune + re-run)
    const float MinRadius = 0.5f;       // labels sit at least this far from the skull centre (outside it)
    const float Gap = 0.12f;            // extra metres beyond the bone / min radius
    const float OutlineWidth = 0.2f;    // TMP outline (0..1)
    const float LeaderWidth = 0.003f;

    // declutter tuning
    const float MinSeparationDeg = 16f; // labels closer than this (angularly) repel each other
    const float MaxDriftDeg = 55f;      // but a label never drifts more than this from its bone's direction
    const int   RelaxIterations = 60;

    [System.Serializable] class LabelDef { public string anchorName; public string term; public string pronunciation; public string definition; }
    [System.Serializable] class ContentDef { public LabelDef[] labels; }

    [MenuItem("A&P Lab/Build Bone Labels")]
    public static void Build()
    {
        var skull = GameObject.Find(SkullName);
        if (skull == null)
        {
            Debug.LogError($"[APLab Labels] No '{SkullName}' in the scene — run A&P Lab/Setup Exploding Skull first.");
            return;
        }

        // --- content (term/pronunciation) from the shared JSON ---
        var jsonPath = Path.Combine(Application.dataPath, ContentPath);
        if (!File.Exists(jsonPath)) { Debug.LogError("[APLab Labels] Content JSON not found at Assets/" + ContentPath); return; }
        var content = JsonUtility.FromJson<ContentDef>(File.ReadAllText(jsonPath));
        if (content?.labels == null || content.labels.Length == 0) { Debug.LogError("[APLab Labels] No labels parsed from JSON."); return; }
        var byAnchor = new Dictionary<string, LabelDef>();
        foreach (var l in content.labels) byAnchor[l.anchorName] = l;

        // --- one representative bone per anchor (first BoneTarget found), in a stable order ---
        var anchors = new List<string>();
        var bones = new List<Transform>();
        var seen = new HashSet<string>();
        foreach (var bt in skull.GetComponentsInChildren<BoneTarget>(true))
        {
            if (string.IsNullOrEmpty(bt.anchorName) || seen.Contains(bt.anchorName)) continue;
            if (!byAnchor.ContainsKey(bt.anchorName)) continue;   // no JSON content -> skip
            seen.Add(bt.anchorName);
            anchors.Add(bt.anchorName);
            bones.Add(bt.transform);
        }
        if (bones.Count == 0) { Debug.LogError("[APLab Labels] No labelled BoneTargets under the skull."); return; }

        Vector3 center = skull.transform.position;

        // base outward directions, then relax them apart so labels don't overlap
        var baseDirs = new Vector3[bones.Count];
        for (int i = 0; i < bones.Count; i++)
        {
            Vector3 d = BoneCenter(bones[i]) - center;
            baseDirs[i] = d.sqrMagnitude > 1e-6f ? d.normalized : Random.onUnitSphere;
        }
        var dirs = Spread(baseDirs, MinSeparationDeg, MaxDriftDeg, RelaxIterations);

        var old = GameObject.Find(RootName);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Bone Labels");
        root.transform.position = Vector3.zero;
        root.transform.localScale = Vector3.one;

        var font = TMP_Settings.defaultFontAsset;
        var overlay = Shader.Find("APLab/RayOverlay");
        if (overlay == null) overlay = Shader.Find("Universal Render Pipeline/Unlit");

        Material outlineMat = null;
        Material leaderMat = overlay != null ? new Material(overlay) { name = "Leader Line", color = Color.white } : null;

        for (int i = 0; i < bones.Count; i++)
        {
            var anchor = anchors[i]; var bone = bones[i]; var def = byAnchor[anchor];

            var go = new GameObject("Label - " + anchor);
            Undo.RegisterCreatedObjectUndo(go, "Build Bone Labels");
            go.transform.SetParent(root.transform, true);

            var tmp = go.AddComponent<TextMeshPro>();
            if (font != null) tmp.font = font;
            var pron = string.IsNullOrEmpty(def.pronunciation) ? "" :
                $"<size=55%>\n<color=#DDDDDD><i>{def.pronunciation}</i></color></size>";
            tmp.text = $"<b>{def.term}</b>{pron}";
            tmp.fontSize = FontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.rectTransform.sizeDelta = new Vector2(1.3f, 0.42f);

            // black outline on a COPY of the font material (shared default untouched)
            if (outlineMat == null && tmp.fontSharedMaterial != null)
            {
                outlineMat = new Material(tmp.fontSharedMaterial) { name = "BoneLabel Outlined" };
                outlineMat.SetFloat(ShaderUtilities.ID_OutlineWidth, OutlineWidth);
                outlineMat.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
            }
            if (outlineMat != null) tmp.fontSharedMaterial = outlineMat;
            tmp.ForceMeshUpdate();

            go.AddComponent<LabelBillboard>();

            // leader line — own child (not billboarded), always-on-top so it isn't occluded
            var leaderGo = new GameObject("Leader");
            leaderGo.transform.SetParent(go.transform, false);
            var lr = leaderGo.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.startWidth = LeaderWidth; lr.endWidth = LeaderWidth;
            lr.numCapVertices = 2;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if (leaderMat != null)
            {
                lr.sharedMaterial = leaderMat;
                lr.startColor = lr.endColor = new Color(1f, 1f, 1f, 0.7f);
            }

            var follow = go.AddComponent<LabelFollow>();
            follow.bone = bone;
            follow.skullCenter = skull.transform;
            // store the decluttered direction in the skull's LOCAL frame so the label rotates with
            // the skull (stays by its bone when spun, instead of stretching the leader line).
            follow.outwardDir = Quaternion.Inverse(skull.transform.rotation) * dirs[i];
            follow.minRadius = MinRadius;
            follow.gap = Gap;
            follow.leader = lr;
        }

        Selection.activeGameObject = root;
        MarkDirty();
        Debug.Log($"[APLab Labels] Built {bones.Count} bone labels — OUTSIDE the skull, black outline, leader " +
                  "lines, decluttered. Tune: BoneLabelBuilder.FontSize / MinRadius / MinSeparationDeg / MaxDriftDeg (re-run).");
    }

    // Relax directions apart on the unit sphere so labels don't overlap, while clamping each
    // direction to stay within maxDriftDeg of its bone's true direction (so a label stays near
    // its bone — the leader line covers the rest).
    static Vector3[] Spread(Vector3[] baseDirs, float minDeg, float maxDriftDeg, int iters)
    {
        int n = baseDirs.Length;
        var dirs = (Vector3[])baseDirs.Clone();
        float minCos = Mathf.Cos(minDeg * Mathf.Deg2Rad);
        for (int it = 0; it < iters; it++)
        {
            var force = new Vector3[n];
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                {
                    float d = Vector3.Dot(dirs[i], dirs[j]);
                    if (d <= minCos) continue;                  // far enough apart
                    Vector3 push = dirs[i] - dirs[j];
                    if (push.sqrMagnitude < 1e-6f)              // (near-)identical — break the tie
                        push = Vector3.Cross(dirs[i], Vector3.up).normalized + Vector3.up * 0.001f;
                    push = push.normalized * (d - minCos);
                    force[i] += push;
                    force[j] -= push;
                }
            for (int i = 0; i < n; i++)
            {
                if (force[i].sqrMagnitude < 1e-12f) continue;
                Vector3 nd = (dirs[i] + force[i] * 1.2f).normalized;
                float drift = Vector3.Angle(baseDirs[i], nd);
                if (drift > maxDriftDeg) nd = Vector3.Slerp(baseDirs[i], nd, maxDriftDeg / drift).normalized;
                dirs[i] = nd;
            }
        }
        return dirs;
    }

    static Vector3 BoneCenter(Transform bone)
    {
        var r = bone.GetComponent<Renderer>();
        return r != null ? r.bounds.center : bone.position;
    }

    static void MarkDirty()
    {
        var s = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (s.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(s);
    }
}

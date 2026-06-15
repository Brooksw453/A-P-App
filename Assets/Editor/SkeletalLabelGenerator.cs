// A&P Lab — Skeletal label generator.
// Stamps anatomical labels (TMP text + leader line + marker) onto the Skull in
// the open Module_Skeletal scene. Run via menu: "A&P Lab/Generate Skeletal Labels".
//
// Data-driven: label TERM / PRONUNCIATION / DEFINITION are pulled from
// Assets/APLab/Content/skeletal-system.json (single source of truth, shared with
// the course). Only the spatial layout (where each landmark + label sits in the
// MR scene) lives here, since that is tuned to the Skull_full model, not content.
//
// Each label gets a LabelInfo (carries the JSON content + landmark for the upcoming
// tap-the-bone Practice) and a LabelBillboard (faces the user at runtime).
//
// TMP is configured in code (font + ForceMeshUpdate) so it renders reliably —
// ad-hoc TMP/TextMesh created through the MCP bridge does not.
//
// Layout: Skull_full sits at ~[0,1.1,0.4] facing -Z (toward the user on the -Z side).
// Labels stack in two columns (right x=+0.33, left x=-0.33) with leader lines to the
// bone landmark. Several deep/posterior bones (sphenoid, ethmoid, occipital, lacrimal,
// palatine, inferior nasal concha, vomer) use APPROXIMATE landmark points — flagged
// below and on LabelInfo.approximate — and are best nudged live in the Editor, or
// shown on the exploded skull later. Re-runnable: rebuilds the "Skeletal Labels" root.

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using TMPro;
using APLab.View;

public static class SkeletalLabelGenerator
{
    const string RootName = "Skeletal Labels";
    const string ContentPath = "APLab/Content/skeletal-system.json"; // under Assets/
    const float FontSize = 0.16f;         // TMP world font size (tweak + re-run if needed)
    // Marker sizing in world metres. The VISIBLE sphere is what the learner aims at;
    // the HIT sphere is a slightly larger, forgiving trigger co-located with it (no more
    // tiny dot floating in a huge invisible hit zone). Tune these two + re-run the menu item.
    const float MarkerVisualDiameter = 0.030f; // ~3 cm glowing target you can actually see
    const float MarkerHitDiameter    = 0.042f; // ~4.2 cm forgiving poke zone (was ~3.6 cm)
    const float LineWidth = 0.0012f;

    // --- JSON shapes (JsonUtility ignores the fields we don't declare) ---
    [System.Serializable] class LabelDef { public string anchorName; public string term; public string pronunciation; public string definition; }
    [System.Serializable] class ContentDef { public LabelDef[] labels; }

    // --- Spatial layout, keyed by anchorName (must match the JSON anchorName) ---
    struct Place { public Vector3 point; public Vector3 label; public bool approx; }

    static Dictionary<string, Place> Layout() => new Dictionary<string, Place>
    {
        // RIGHT column (label.x > 0, left-aligned text), top -> bottom
        { "Frontal bone",   new Place{ point=new Vector3( 0.020f,1.275f,0.310f), label=new Vector3( 0.33f,1.330f,0.27f) } },
        { "Parietal bone",  new Place{ point=new Vector3( 0.055f,1.265f,0.400f), label=new Vector3( 0.33f,1.285f,0.27f) } },
        { "Temporal bone",  new Place{ point=new Vector3( 0.060f,1.185f,0.410f), label=new Vector3( 0.33f,1.240f,0.27f) } },
        { "Sphenoid bone",  new Place{ point=new Vector3( 0.058f,1.205f,0.355f), label=new Vector3( 0.33f,1.195f,0.27f), approx=true } },
        { "Zygomatic bone", new Place{ point=new Vector3( 0.045f,1.165f,0.330f), label=new Vector3( 0.33f,1.150f,0.27f) } },
        { "Maxilla",        new Place{ point=new Vector3( 0.000f,1.135f,0.310f), label=new Vector3( 0.33f,1.105f,0.27f) } },
        { "Occipital bone", new Place{ point=new Vector3( 0.030f,1.205f,0.500f), label=new Vector3( 0.33f,1.060f,0.27f), approx=true } },

        // LEFT column (label.x < 0, right-aligned text), top -> bottom
        { "Nasal bone",            new Place{ point=new Vector3( 0.000f,1.205f,0.300f), label=new Vector3(-0.33f,1.330f,0.27f) } },
        { "Ethmoid bone",          new Place{ point=new Vector3( 0.012f,1.195f,0.305f), label=new Vector3(-0.33f,1.285f,0.27f), approx=true } },
        { "Lacrimal bone",         new Place{ point=new Vector3( 0.028f,1.205f,0.325f), label=new Vector3(-0.33f,1.240f,0.27f), approx=true } },
        { "Inferior nasal concha", new Place{ point=new Vector3( 0.012f,1.165f,0.305f), label=new Vector3(-0.33f,1.195f,0.27f), approx=true } },
        { "Vomer",                 new Place{ point=new Vector3( 0.000f,1.155f,0.310f), label=new Vector3(-0.33f,1.150f,0.27f), approx=true } },
        { "Palatine bone",         new Place{ point=new Vector3( 0.000f,1.100f,0.400f), label=new Vector3(-0.33f,1.105f,0.27f), approx=true } },
        { "Mandible",              new Place{ point=new Vector3( 0.000f,1.085f,0.320f), label=new Vector3(-0.33f,1.060f,0.27f) } },
    };

    [MenuItem("A&P Lab/Generate Skeletal Labels")]
    public static void Generate()
    {
        // Load content (term/pronunciation/definition) from the shared JSON.
        var jsonPath = Path.Combine(Application.dataPath, ContentPath);
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("[A&P Lab] Content JSON not found at Assets/" + ContentPath);
            return;
        }
        var content = JsonUtility.FromJson<ContentDef>(File.ReadAllText(jsonPath));
        if (content?.labels == null || content.labels.Length == 0)
        {
            Debug.LogError("[A&P Lab] No labels parsed from " + ContentPath);
            return;
        }
        var byAnchor = new Dictionary<string, LabelDef>();
        foreach (var l in content.labels) byAnchor[l.anchorName] = l;

        var old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Generate Skeletal Labels");

        var font = TMP_Settings.defaultFontAsset;
        var unlit = Shader.Find("Universal Render Pipeline/Unlit");

        int made = 0;
        var approxNames = new List<string>();
        var missingContent = new List<string>();

        foreach (var kv in Layout())
        {
            var anchor = kv.Key;
            var p = kv.Value;
            if (!byAnchor.TryGetValue(anchor, out var def))
            {
                missingContent.Add(anchor); // layout has it but JSON doesn't — skip
                continue;
            }

            var group = new GameObject("Label - " + anchor);
            group.transform.SetParent(root.transform, true);

            // marker sphere at the bone landmark — also the pokeable BoneTarget
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Marker";
            marker.transform.SetParent(group.transform, true);
            marker.transform.position = p.point;
            marker.transform.localScale = Vector3.one * MarkerVisualDiameter;
            // Trigger = a slightly larger forgiveness zone, centred on the visible sphere.
            // radius is in LOCAL units: worldHitRadius = radius * localScale, so
            // radius = (hitDiameter/2) / visualDiameter keeps the world size exact.
            var col = marker.GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = (MarkerHitDiameter * 0.5f) / MarkerVisualDiameter;
            if (unlit != null)
                marker.GetComponent<MeshRenderer>().sharedMaterial =
                    new Material(unlit) { color = new Color(1f, 0.85f, 0.3f) };
            var target = marker.AddComponent<BoneTarget>();
            target.anchorName = anchor;

            // TMP text: term (bold) + pronunciation (small italic)
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(group.transform, true);
            textGo.transform.position = p.label;
            textGo.transform.rotation = Quaternion.identity; // readable face points -Z (toward the user)
            var tmp = textGo.AddComponent<TextMeshPro>();
            if (font != null) tmp.font = font;
            var pron = string.IsNullOrEmpty(def.pronunciation) ? "" :
                $"<size=52%>\n<color=#C9C2A8><i>{def.pronunciation}</i></color></size>";
            tmp.text = $"<b>{def.term}</b>{pron}";
            tmp.fontSize = FontSize;
            tmp.color = new Color(1f, 0.97f, 0.85f);
            bool rightCol = p.label.x >= 0f;
            tmp.alignment = rightCol ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            tmp.rectTransform.sizeDelta = new Vector2(0.55f, 0.12f);
            // Pivot on the inner edge so text starts at the leader endpoint and grows
            // outward (away from the skull) instead of overlapping the mesh.
            tmp.rectTransform.pivot = new Vector2(rightCol ? 0f : 1f, 0.5f);
            textGo.transform.position = p.label; // re-apply: pivot change keeps inner edge on the endpoint
            tmp.ForceMeshUpdate();

            // billboard so the label faces the user at runtime
            textGo.AddComponent<LabelBillboard>();

            // carry the content + landmark for the Practice interaction
            var info = group.AddComponent<LabelInfo>();
            info.anchorName = anchor;
            info.term = def.term;
            info.pronunciation = def.pronunciation;
            info.definition = def.definition;
            info.landmark = p.point;
            info.approximate = p.approx;

            // leader line: landmark -> label
            var lr = group.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.SetPosition(0, p.point);
            lr.SetPosition(1, p.label);
            lr.startWidth = LineWidth;
            lr.endWidth = LineWidth;
            lr.numCapVertices = 2;
            if (unlit != null)
                lr.sharedMaterial = new Material(unlit) { color = new Color(1f, 1f, 1f, 0.65f) };
            lr.startColor = lr.endColor = new Color(1f, 1f, 1f, 0.7f);

            if (p.approx) approxNames.Add(anchor);
            made++;
        }

        Debug.Log($"[A&P Lab] Generated {made} skeletal labels under '{RootName}' " +
                  $"(from Assets/{ContentPath}).");
        if (approxNames.Count > 0)
            Debug.LogWarning("[A&P Lab] Approximate landmark points (nudge in Editor): " +
                             string.Join(", ", approxNames) + ".");
        if (missingContent.Count > 0)
            Debug.LogWarning("[A&P Lab] Layout entries with no JSON content (skipped): " +
                             string.Join(", ", missingContent) + ".");

        Selection.activeGameObject = root;
        EditorSceneMarkDirty();
    }

    static void EditorSceneMarkDirty()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
    }
}

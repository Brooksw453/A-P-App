// A&P Lab — Skeletal label generator.
// Stamps anatomical labels (TMP text + leader line + marker) onto the Skull in
// the open Module_Skeletal scene. Run via menu: "A&P Lab/Generate Skeletal Labels".
// TMP is configured in code (font + ForceMeshUpdate) so it renders reliably —
// ad-hoc TMP/TextMesh created through the MCP bridge does not.
//
// Positions are world coords tuned for Skull (Skull_full) at ~[0,1.1,0.4] facing -Z.
// Fine-tune size/position after running via the Inspector or the bridge.

using UnityEngine;
using UnityEditor;
using TMPro;

public static class SkeletalLabelGenerator
{
    const string RootName = "Skeletal Labels";
    const float FontSize = 0.2f;          // TMP world font size (tweak + re-run if needed)
    const float MarkerSize = 0.006f;
    const float LineWidth = 0.0015f;

    struct Bone { public string name; public Vector3 point; public Vector3 label; }

    static Bone[] Bones() => new[]
    {
        // Right column (label.x > 0, left-aligned), top to bottom
        new Bone{ name="Frontal bone",   point=new Vector3( 0.02f,1.275f,0.31f), label=new Vector3( 0.30f,1.32f,0.28f) },
        new Bone{ name="Parietal bone",  point=new Vector3( 0.055f,1.265f,0.40f),label=new Vector3( 0.30f,1.26f,0.28f) },
        new Bone{ name="Temporal bone",  point=new Vector3( 0.060f,1.185f,0.41f),label=new Vector3( 0.30f,1.20f,0.28f) },
        new Bone{ name="Zygomatic bone", point=new Vector3( 0.045f,1.165f,0.33f),label=new Vector3( 0.30f,1.14f,0.28f) },
        // Left column (label.x < 0, right-aligned), top to bottom
        new Bone{ name="Nasal bone",     point=new Vector3( 0.00f,1.205f,0.30f), label=new Vector3(-0.30f,1.24f,0.28f) },
        new Bone{ name="Maxilla",        point=new Vector3( 0.00f,1.135f,0.31f), label=new Vector3(-0.30f,1.16f,0.28f) },
        new Bone{ name="Mandible",       point=new Vector3( 0.00f,1.085f,0.32f), label=new Vector3(-0.30f,1.08f,0.28f) },
    };

    [MenuItem("A&P Lab/Generate Skeletal Labels")]
    public static void Generate()
    {
        var old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Generate Skeletal Labels");

        var font = TMP_Settings.defaultFontAsset;
        var unlit = Shader.Find("Universal Render Pipeline/Unlit");

        foreach (var b in Bones())
        {
            var group = new GameObject("Label - " + b.name);
            group.transform.SetParent(root.transform, true);

            // marker sphere at the bone landmark
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Marker";
            Object.DestroyImmediate(marker.GetComponent<Collider>());
            marker.transform.SetParent(group.transform, true);
            marker.transform.position = b.point;
            marker.transform.localScale = Vector3.one * MarkerSize;
            if (unlit != null)
                marker.GetComponent<MeshRenderer>().sharedMaterial =
                    new Material(unlit) { color = new Color(1f, 0.85f, 0.3f) };

            // TMP text
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(group.transform, true);
            textGo.transform.position = b.label;
            textGo.transform.rotation = Quaternion.identity; // TMP readable face already points -Z (toward the user)
            var tmp = textGo.AddComponent<TextMeshPro>();
            if (font != null) tmp.font = font;
            tmp.text = b.name;
            tmp.fontSize = FontSize;
            tmp.color = new Color(1f, 0.97f, 0.85f);
            tmp.alignment = b.label.x >= 0f ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            tmp.rectTransform.sizeDelta = new Vector2(0.5f, 0.1f);
            tmp.ForceMeshUpdate();

            // leader line marker -> text
            var lr = group.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.SetPosition(0, b.point);
            lr.SetPosition(1, b.label);
            lr.startWidth = LineWidth;
            lr.endWidth = LineWidth;
            lr.numCapVertices = 2;
            if (unlit != null)
            {
                var lineMat = new Material(unlit) { color = new Color(1f, 1f, 1f, 0.65f) };
                lr.sharedMaterial = lineMat;
            }
            lr.startColor = lr.endColor = new Color(1f, 1f, 1f, 0.7f);
        }

        Debug.Log("[A&P Lab] Generated " + Bones().Length + " skeletal labels under '" + RootName + "'.");
        Selection.activeGameObject = root;
    }
}

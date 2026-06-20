// A&P Lab — builds the hand-ray visuals (ray line + cursor per hand) as STATIC scene
// objects and wires up a HandRaySelector. Menu: "A&P Lab/Build Hand Ray".
//
// Run AFTER adding the Meta "Hand Tracking" building block to the scene (that provides
// the OVRHand components the selector reads). Re-runnable: rebuilds the "Hand Ray" root.
// Visuals are created here in the editor (never at runtime under the XR rig).

using UnityEngine;
using UnityEditor;
using APLab.View;

public static class HandRayBuilder
{
    const string RootName = "Hand Ray";

    [MenuItem("A&P Lab/Build Hand Ray")]
    public static void Build()
    {
        var old = GameObject.Find(RootName);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var unlit = Shader.Find("Universal Render Pipeline/Unlit");
        // Rays use an ALWAYS-ON-TOP overlay shader so they're never hidden behind the skull /
        // teeth (that was the "right ray invisible" bug — it was being occluded). Fall back to
        // Unlit if the shader didn't import.
        var rayShader = Shader.Find("APLab/RayOverlay");
        if (rayShader == null) rayShader = unlit;
        var rayColor = new Color(0.45f, 0.85f, 1f, 1f);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Hand Ray");
        var sel = root.AddComponent<HandRaySelector>();

        var rays = new LineRenderer[2];
        var cursors = new Transform[2];
        for (int i = 0; i < 2; i++)
        {
            var lineGo = new GameObject($"Ray {i}");
            lineGo.transform.SetParent(root.transform, false);
            var lr = lineGo.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.startWidth = 0.012f;
            lr.endWidth = 0.008f;
            lr.numCapVertices = 2;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if (rayShader != null)
            {
                lr.sharedMaterial = new Material(rayShader) { color = rayColor };
                lr.startColor = lr.endColor = rayColor;
            }
            lr.enabled = false;
            rays[i] = lr;

            var cur = GameObject.CreatePrimitive(PrimitiveType.Sphere);   // editor-time = safe
            cur.name = $"Cursor {i}";
            cur.transform.SetParent(root.transform, false);
            cur.transform.localScale = Vector3.one * 0.022f;
            var col = cur.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);                // visual only
            var mr = cur.GetComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if (unlit != null) mr.sharedMaterial = new Material(unlit) { color = new Color(0.6f, 0.95f, 1f) };
            cur.SetActive(false);
            cursors[i] = cur.transform;
        }

        sel.rays = rays;
        sel.cursors = cursors;

        Selection.activeGameObject = root;
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[APLab HandRay] Built 'Hand Ray' (2 rays + cursors) + HandRaySelector. " +
                  "Make sure the Meta Hand Tracking building block is in the scene so OVRHand exists.");
    }
}

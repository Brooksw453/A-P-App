using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ColliderSetter: MonoBehaviour
{
    private static readonly List<string> ExcludeNames = new List<string>();

    [ContextMenu("SetBoxColliderToEncapsulateModels")]
    public void SetBoxColliderToEncapsulateModels()
    {
        SetBoxColliderToEncapsulateModels<BoxCollider>(1, gameObject);
    }

    public void SetBoxColliderToEncapsulateModels<CollType>(int colliderPadding, params GameObject[] from) where CollType : Collider
    {
        var boundsForEach = new List<Bounds>();
        var totalBounds = new Bounds();
        var index = 0;

        foreach (var go in from)
        {
            if (!ExcludeNames.Any(go.name.Contains))
            {
                var bounds = GetChildRendererBounds(go);
                boundsForEach.Add(bounds);
                totalBounds.Encapsulate(bounds);
            }
        }

        foreach (var go in from)
        {
            if (typeof(CollType) == typeof(BoxCollider))
            {
                BoxCollider bc = go.AddComponent<BoxCollider>();
                bc.enabled = true;
                bc.size = (boundsForEach[index].size * 2) * colliderPadding;
                bc.center = new Vector3(0f, (bc.size.y / 2), 0f);
                index++;
            }
        }
    }

    private Bounds GetChildRendererBounds(GameObject go)
    {
        var meshFilters = go.GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length > 0)
        {
            var bounds = meshFilters[0].sharedMesh.bounds;
            foreach (var rend in meshFilters)
            {
                if (!ExcludeNames.Any(rend.gameObject.name.Contains))
                {
                    bounds.Encapsulate(rend.sharedMesh.bounds);
                }
            }
            return bounds;
        }
        else { return new Bounds(); }
    }
}
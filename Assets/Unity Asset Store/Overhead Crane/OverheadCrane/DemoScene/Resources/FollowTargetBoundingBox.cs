using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetBoundingBox : MonoBehaviour
{
    public SkinnedMeshRenderer targetMesh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetMesh == null)
        {
            Debug.LogWarning("Target Mesh is not set. Please select the " + gameObject.name + " object and drop a target skinned mesh into the Target Mesh field.");
            return;
        }

        Vector3 pos = targetMesh.bounds.center;

        pos.y = targetMesh.transform.parent.parent.position.y/2;
        transform.position = pos;
    }
}

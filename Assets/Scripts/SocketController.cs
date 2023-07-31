using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketController : MonoBehaviour
{

    public string socketName;
    public GameObject connectedBone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        public void AttachBone(GameObject bone)
    {
        connectedBone = bone;
        bone.transform.SetParent(transform);
        bone.transform.localPosition = Vector3.zero;
        bone.transform.localRotation = Quaternion.identity;
    }

    public void DetachBone()
    {
        if (connectedBone != null)
        {
            connectedBone.transform.SetParent(null);
            connectedBone = null;
        }
    }

}

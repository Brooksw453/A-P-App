using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamPanner : MonoBehaviour
{
    public Transform cam;
    public float panSpeed = 2f;
    private Vector3 lastPos;
    private 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!cam.gameObject.activeInHierarchy)
            return;

        transform.rotation = cam.rotation;

        if (Input.GetMouseButtonDown(2))
        {
            lastPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {

            Vector3 delta = Input.mousePosition - lastPos;
            transform.Translate(-delta.x * panSpeed * Time.deltaTime, -delta.y * panSpeed * Time.deltaTime, 0);
            lastPos = Input.mousePosition;
        }

        ClampPos();
    }

    public void ClampPos()
    {
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -5, 5), Mathf.Clamp(transform.localPosition.y, 0, 6), Mathf.Clamp(transform.localPosition.z, -12, 6));
    }
}

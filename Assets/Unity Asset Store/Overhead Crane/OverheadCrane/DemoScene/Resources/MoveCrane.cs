using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NOT_Lonely;

public class MoveCrane : MonoBehaviour
{
    public NL_OverheadCrane crane;
    private float speed = 0.5f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 3;
        }
        else
        {
            speed = 0.5f;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {   
            MoveZ(speed);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveZ(-speed);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveX(-speed);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveX(speed);
        }
    }

    public void MoveX(float speed)
    {
        crane.transform.localPosition = new Vector3(Mathf.Clamp(crane.transform.localPosition.x - speed * Time.deltaTime, -7, 7 - crane.craneWidth), 0, Mathf.Clamp(crane.transform.localPosition.z, -14.7f + crane.craneLength, 6.25f));
    }

    public void MoveZ(float speed)
    {
        crane.transform.localPosition = new Vector3(Mathf.Clamp(crane.transform.localPosition.x, -7, 7 - crane.craneWidth), 0, Mathf.Clamp(crane.transform.localPosition.z - speed * Time.deltaTime, -14.7f + crane.craneLength, 6.25f));
    }

    /*
    public void MoveRight(float speed)
    {
        crane.transform.localPosition = new Vector3(crane.transform.localPosition.x - speed * Time.deltaTime, 0, crane.transform.localPosition.z);
        ClampCranePos();
    }

    public void MoveLeft(float speed)
    {
        crane.transform.localPosition = new Vector3(crane.transform.localPosition.x + speed * Time.deltaTime, 0, crane.transform.localPosition.z);
        ClampCranePos();
    }

    public void MoveForward(float speed)
    {
        crane.transform.localPosition = new Vector3(crane.transform.localPosition.x, 0, crane.transform.localPosition.z - speed * Time.deltaTime);
        ClampCranePos();
    }

    public void MoveBackward(float speed)
    {
        crane.transform.localPosition = new Vector3(crane.transform.localPosition.x, 0, crane.transform.localPosition.z + speed * Time.deltaTime);
        ClampCranePos();
    }
    */

    public void ClampCranePos()
    {
        crane.transform.localPosition = new Vector3(Mathf.Clamp(crane.transform.localPosition.x, -7, 7 - crane.craneWidth), 0, Mathf.Clamp(crane.transform.localPosition.z, -14.7f + crane.craneLength, 6.25f));
    }

}

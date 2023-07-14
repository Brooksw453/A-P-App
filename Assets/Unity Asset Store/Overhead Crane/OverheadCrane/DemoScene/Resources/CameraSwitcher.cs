namespace NOT_Lonely
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class CameraSwitcher : MonoBehaviour
    {
        public GameObject[] cameraObjects;
        public int selectedCameraID;
        public Text text;
        private NL_Remote craneRemote;

        // Start is called before the first frame update
        void Start()
        {
            craneRemote = FindObjectOfType<NL_Remote>();
            SwitchCam();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                NextCamera();
            }
        }

        public void NextCamera()
        {
            selectedCameraID++;

            if (selectedCameraID > cameraObjects.Length - 1)
            {
                selectedCameraID = 0;
            }

            SwitchCam();
        }

        void SwitchCam()
        {
            for (int i = 0; i < cameraObjects.Length; i++)
            {
                cameraObjects[i].gameObject.SetActive(false);
            }
            cameraObjects[selectedCameraID].gameObject.SetActive(true);

            if (text != null)
            {
                text.text = cameraObjects[selectedCameraID].name;
            }

            if (selectedCameraID == 0)
            {
                craneRemote.enableKeyboardControls = false;
            }
            else
            {
                craneRemote.enableKeyboardControls = true;
            }
        }
    }
}

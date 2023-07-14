namespace NOT_Lonely
{
    using UnityEngine;
    using UnityEngine.UI;

    public class FPInteractionExample : MonoBehaviour
    {
        private NL_Remote remote;
        public Camera playerCamera;
        public KeyCode interactionKey = KeyCode.E;

        public Canvas interactionText;
        private Text text;

        [Tooltip("The maximum distance that the player can interact from.")]
        public float maxInteractionDistance = 1.5f;

        public LayerMask layerMask = -1;

        private NL_RemoteButton curruntBtn;

        void Start()
        {
            remote = GetComponent<NL_Remote>();

            if (interactionText != null)
            {
                interactionText = Instantiate(interactionText);

                text = interactionText.GetComponentInChildren<Text>();
                text.text = "Press " + interactionKey.ToString() + " key to interact";

                interactionText.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if(Vector3.SqrMagnitude(playerCamera.transform.position - remote.transform.position) < maxInteractionDistance * maxInteractionDistance)
            {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxInteractionDistance))
                {
                    for (int i = 0; i < remote.buttons.Length; i++)
                    {
                        if (hit.collider == remote.buttons[i].btnCollider)
                        {
                            curruntBtn = remote.buttons[i];

                            if (Input.GetKeyDown(interactionKey))
                            {
                                remote.buttons[i].OnPressed();
                            }

                            if (Input.GetKey(interactionKey)) {
                                remote.buttons[i].OnHold();                                
                            }
                        }
                        else
                        {
                            //released  
                            if (curruntBtn == remote.buttons[i])
                            {
                                curruntBtn.OnRelease();
                                curruntBtn = null;
                            }
                        }
                    }
                    if (curruntBtn != null)
                    {
                        interactionText.gameObject.SetActive(true);
                    }
                    else
                    {
                        interactionText.gameObject.SetActive(false);
                    }
                }
                }

            //released
            if (Input.GetKeyUp(interactionKey))
            {
                if (curruntBtn != null)
                {
                    curruntBtn.OnRelease();
                    curruntBtn = null;
                }
            }
        }
    }
}

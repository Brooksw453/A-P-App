namespace NOT_Lonely
{
    using UnityEngine.Events;
    using UnityEngine;

    public class NL_RemoteButton : MonoBehaviour
    {
        public Collider[] excludeTriggers;
        private bool excludedTrigger;
        [HideInInspector] public Collider btnCollider;

        public UnityEvent onButtonPressed;
        public UnityEvent onButtonHold;
        public UnityEvent onButtonRelease;
        [HideInInspector] public bool isInteractable = true;

        private void Start()
        {
            btnCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != transform.parent.parent.transform) {

                excludedTrigger = false;

                for (int i = 0; i < excludeTriggers.Length; i++)
                {
                    if (other == excludeTriggers[i])
                    {
                        excludedTrigger = true;
                        break;
                    }
                }

                if (excludedTrigger)
                    return;

                OnPressed();

                //Debug.Log(other.name + " object pressed the " + gameObject.name + " button");
            } 
        }

        private void OnTriggerStay(Collider other)
        {
            if (other != transform.parent.parent.transform)
            {
                excludedTrigger = false;

                for (int i = 0; i < excludeTriggers.Length; i++)
                {
                    if (other == excludeTriggers[i])
                    {
                        excludedTrigger = true;
                        break;
                    }
                }

                if (excludedTrigger)
                    return;

                OnHold();
                //Debug.Log(other.name + " object is holding the " + gameObject.name + " button");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other != transform.parent.parent.transform)
            {
                excludedTrigger = false;

                for (int i = 0; i < excludeTriggers.Length; i++)
                {
                    if (other == excludeTriggers[i])
                    {
                        excludedTrigger = true;
                        break;
                    }
                }

                if (excludedTrigger)
                    return;

                OnRelease();
               // Debug.Log(other.name + " object released the " + gameObject.name + " button");
            }
        }

        public void OnPressed()
        {
            if (!isInteractable)
                return;

            onButtonPressed.Invoke();
            ButtonSwitch(true);
        }

        public void OnHold()
        {
            if (!isInteractable)
                return;

            onButtonHold.Invoke();
            ButtonSwitch(true);
        }

        public void OnRelease()
        {
            if (!isInteractable)
                return;

            onButtonRelease.Invoke();
            ButtonSwitch(false);
        }

        public void ButtonSwitch(bool value)
        {
            float pos;

            if (value)
            {
                pos = -0.005f;
            }
            else
            {
                pos = 0;
            }

            transform.localPosition = new Vector3(0, transform.localPosition.y, pos);
        }
    }
}

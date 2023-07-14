namespace NOT_Lonely
{
    using UnityEngine;
    using System.Collections;

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class NL_Hook : MonoBehaviour
    {
        [SerializeField] public Transform wireDrumMount;
        [SerializeField] public Transform wireHookMount;
        [SerializeField] public SkinnedMeshRenderer wires_mesh;
        [SerializeField] public MaterialPropertyBlock mtlPropBlock;
        [SerializeField] public ConfigurableJoint hookJoint;
        [SerializeField] public Rigidbody rb;

        public LayerMask layerMask = -1;
        public float hookOffset = 0.3f;
        private NL_OverheadCrane crane;
        public NL_RemoteButton buttonDown;
        public NL_AttachCargo attachCargo;

        public void Init()
        {
            wireDrumMount = GetChildByName(transform, "WireDrumMount");
            wireHookMount = GetChildByName(transform, "WireHookMount");
            wires_mesh = GetChildByName(transform, "Wires_mesh").GetComponent<SkinnedMeshRenderer>();

            hookJoint = wireHookMount.GetComponent<ConfigurableJoint>();
            rb = hookJoint.GetComponent<Rigidbody>();

            mtlPropBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            crane = transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<NL_OverheadCrane>();
            crane.OnHookHeightChanged += UpdateHookHeight;
        }

        private void OnDisable()
        {
            crane.OnHookHeightChanged -= UpdateHookHeight;
        }


        private void Start()
        {
            //attachCargo = GetComponentInChildren<NL_AttachCargo>();

            if (Application.isPlaying)
            {
                Rigidbody inertiaBody = new GameObject("Hook Inertial Body").AddComponent<Rigidbody>();
                FixedJoint inertiaBodyJoint = inertiaBody.gameObject.AddComponent<FixedJoint>();
                inertiaBody.transform.parent = wireHookMount;
                inertiaBody.transform.localPosition = Vector3.zero;
                inertiaBodyJoint.connectedBody = rb;
                inertiaBody.transform.parent = null;
            }
        }

        private Transform GetChildByName(Transform parent, string childName)
        {
            Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
            Transform child = null;

            for (int i = 0; i < allChildren.Length; i++)
            {
                if (allChildren[i].name == childName)
                {
                    child = allChildren[i];
                }
            }

            if (child != null)
            {
                //Debug.Log("Object " + child.name + " found");
                return child;
            }
            else
            {
                Debug.Log("Object with name " + childName + " NOT found");
                return null;
            }
        }

        private void Update()
        {
            if (!rb.IsSleeping())
            {
                wires_mesh.localBounds = new Bounds(new Vector3(wireHookMount.localPosition.x / 2, wireHookMount.localPosition.y / 2, wireHookMount.localPosition.z / 2 + 0.488f), new Vector3(-(Mathf.Abs(wireHookMount.localPosition.x) + 0.4f), -wireHookMount.localPosition.y, -(Mathf.Abs(wireHookMount.localPosition.z) + 0.5f)));
            }
        }


        public void UpdateHookHeight(float speed, float t, float craneHeight)
        {
            RaycastHit hit;
            if (Physics.Raycast(hookJoint.transform.position, Vector3.down, out hit, 0.35f, layerMask, QueryTriggerInteraction.Collide))
            {
                if (!attachCargo.isAttached)
                {
                    buttonDown.isInteractable = false;
                }
            }
            else
            {
                buttonDown.isInteractable = true;
            }
            if (attachCargo.isAttached)
            {
                buttonDown.isInteractable = true;
            }
            wireDrumMount.localPosition = new Vector3(0.024f, 0, Mathf.Lerp(-0.798f, -0.488f, t));

            //hookJoint.anchor = new Vector3(0, Mathf.Lerp(craneHeight + 0.3f, 0.5f, t), 0);
            hookJoint.anchor = new Vector3(0, Mathf.Lerp(craneHeight + hookOffset, 0.5f, t), 0);
            hookJoint.connectedAnchor = new Vector3(hookJoint.connectedAnchor.x, 0, Mathf.Lerp(0.64f, 0.488f, t));
            
            hookJoint.targetRotation = new Quaternion(0, Mathf.Lerp(-0.66f, 0, t), 0 , 1);

            mtlPropBlock = new MaterialPropertyBlock();

            mtlPropBlock.SetVector("_TilingXY", new Vector2(Mathf.Lerp(craneHeight + 0.3f, 0.5f, t) * 2, 1));
            wires_mesh.SetPropertyBlock(mtlPropBlock);
        }
    }
}
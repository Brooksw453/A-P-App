namespace NOT_Lonely
{
    using UnityEngine;
    using UnityEngine.Events;
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class NL_Remote : MonoBehaviour
    {
        [HideInInspector] public Transform cable_remoteMount;
        [HideInInspector] public SkinnedMeshRenderer cable_mesh;
        [HideInInspector] public ConfigurableJoint joint;
        [HideInInspector] public MaterialPropertyBlock mtlPropBlock;
        public bool enableKeyboardControls = true;

        public NL_RemoteButton[] buttons = new NL_RemoteButton[6];

        private NL_OverheadCrane crane;

        void Start()
        {
            cable_remoteMount = transform.parent;
            cable_mesh = FindNeighbourMesh(cable_remoteMount);
            joint = cable_remoteMount.GetComponent<ConfigurableJoint>();

            mtlPropBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            crane = transform.parent.parent.parent.parent.parent.GetComponent<NL_OverheadCrane>();
            crane.OnRemoteHeightChanged += UpdateRemoteHeight;
            crane.OnKeyboardControlsSwitched += SwitchKeyboardControls;
        }

        private void OnDisable()
        {
            crane.OnRemoteHeightChanged -= UpdateRemoteHeight;
            crane.OnKeyboardControlsSwitched -= SwitchKeyboardControls;
        }

        private void SwitchKeyboardControls(bool value)
        {
            enableKeyboardControls = value;
        }

        private void Update()
        {
            UpdateRemoteCableBounds();

            if (!Application.isPlaying)
                return;

            if (!enableKeyboardControls)
                return;

            if (Input.GetKey(KeyCode.W) && buttons[0] != null) //forward hold
            {
                buttons[0].OnHold();
            }
            
            if (Input.GetKeyDown(KeyCode.W) && buttons[0] != null) //forward pressed
            {
                buttons[0].OnPressed();
            }

            if (Input.GetKeyUp(KeyCode.W) && buttons[0] != null) //forward released
            {
                buttons[0].OnRelease();
            }

            if (Input.GetKey(KeyCode.S) && buttons[1] != null) //backward hold
            {
                buttons[1].OnHold();
            }

            if (Input.GetKeyDown(KeyCode.S) && buttons[1] != null) //backward pressed
            {
                buttons[1].OnPressed();
            }

            if (Input.GetKeyUp(KeyCode.S) && buttons[1] != null) //backward released
            {
                buttons[1].OnRelease();
            }

            if (Input.GetKey(KeyCode.A) && buttons[2] != null) //left hold
            {
                buttons[2].OnHold();
            }

            if (Input.GetKeyDown(KeyCode.A) && buttons[2] != null) //left pressed
            {
                buttons[2].OnPressed();
            }

            if (Input.GetKeyUp(KeyCode.A) && buttons[2] != null) //left released
            {
                buttons[2].OnRelease();
            }

            if (Input.GetKey(KeyCode.D) && buttons[3] != null) //right hold
            {
                buttons[3].OnHold();
            }

            if (Input.GetKeyDown(KeyCode.D) && buttons[3] != null) //right pressed
            {
                buttons[3].OnPressed();
            }

            if (Input.GetKeyUp(KeyCode.D) && buttons[3] != null) //right released
            {
                buttons[3].OnRelease();
            }

            if (Input.GetKey(KeyCode.E) && buttons[4] != null) // up
            {
                buttons[4].OnHold();
            }

            if (Input.GetKeyUp(KeyCode.E) && buttons[4] != null) //up released
            {
                buttons[4].OnRelease();
            }

            if (Input.GetKey(KeyCode.Q) && buttons[5] != null) //down
            {
                buttons[5].OnHold();
            }

            if (Input.GetKeyUp(KeyCode.Q) && buttons[5] != null) //down released
            {
                buttons[5].OnRelease();
            }

        }

        private SkinnedMeshRenderer FindNeighbourMesh(Transform neighbour)
        {
            
            SkinnedMeshRenderer mesh = null;
            SkinnedMeshRenderer[] allChildMeshes = neighbour.parent.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < allChildMeshes.Length; i++)
            {
                if (allChildMeshes[i].transform.parent == neighbour.parent)
                {
                    mesh = allChildMeshes[i];
                    //Debug.Log(mesh);
                }
            }

            return mesh;
        }

        public void UpdateRemoteHeight(float craneHeight, float t)
        {
            float height = craneHeight - 0.5f;

            cable_remoteMount.localPosition = new Vector3(0, Mathf.Lerp(-height, -0.3f, t), 0);

            joint.anchor = new Vector3(0, -cable_remoteMount.localPosition.y, 0);

            mtlPropBlock = new MaterialPropertyBlock();

            mtlPropBlock.SetVector("_TilingXY", new Vector2(Mathf.Lerp(height, 0.3f, t) * 0.5f, 1));
            cable_mesh.SetPropertyBlock(mtlPropBlock);
        }

        public void UpdateRemoteCableBounds()
        {
            cable_mesh.localBounds = new Bounds(new Vector3(cable_remoteMount.localPosition.x / 2, cable_remoteMount.localPosition.y / 2, cable_remoteMount.localPosition.z / 2), new Vector3(-cable_remoteMount.localPosition.x + 0.024f, -cable_remoteMount.localPosition.y, -cable_remoteMount.localPosition.z + 0.024f));
        }
    }
}

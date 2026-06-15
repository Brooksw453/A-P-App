// A&P Lab — input layer for poke/select, decoupled from any XR toolkit.
//
// IPokeReceiver  : anything pokeable (BoneTarget now; PokeButton in slice 2).
// PokeTip        : a trigger collider on a fingertip / controller tip; pokes a
//                  receiver on contact. Works with OVR hand/controller anchors —
//                  parent this under MR Camera Rig's RightHandAnchor (or a tracked
//                  index-tip transform). Needs a kinematic Rigidbody for triggers.
// MouseRaySelector: editor/desktop fallback — click to select under the pointer.
//                  Compiled in only when the legacy Input Manager is enabled.

using UnityEngine;

namespace APLab.View
{
    public interface IPokeReceiver { void OnPoke(); }

    [RequireComponent(typeof(Rigidbody))]
    public class PokeTip : MonoBehaviour
    {
        [Header("Tip placement")]
        [Tooltip("Local position of the poke point on the hand/controller anchor. The trigger " +
                 "sphere (and the visible tip, if assigned) are placed here so they line up — " +
                 "fixes the 'visible ball isn't where the trigger is' mismatch.")]
        public Vector3 tipCenter = new Vector3(0f, 0f, 0.12f);

        [Tooltip("World radius of the poke trigger sphere. Match it to the visible tip ball.")]
        public float tipRadius = 0.02f;

        [Tooltip("Optional: the visible tip mesh (a child sphere). Snapped onto the trigger so " +
                 "what you see is exactly what pokes.")]
        public Transform visibleTip;

        [Header("Behaviour")]
        [Tooltip("Re-arm delay so one touch fires once.")]
        public float debounce = 0.3f;
        float _nextAllowed;

        void Reset()  => Configure();
        void Awake()  => Configure();

        // Click this in the inspector (component gear menu) to re-align the tip while tuning.
        [ContextMenu("Align tip (collider + visible marker)")]
        void Configure()
        {
            var rb = GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

            // Place + size the trigger on an EXISTING SphereCollider. We never create a
            // collider at runtime under the XR rig — that caused the tracking-origin drift
            // ("skull flies up"). Add the SphereCollider in the editor; this keeps it aligned.
            var sphere = GetComponent<SphereCollider>();
            if (sphere != null)
            {
                sphere.isTrigger = true;
                sphere.center = tipCenter;
                float s = Mathf.Max(0.0001f, MaxAbs(transform.lossyScale));
                sphere.radius = tipRadius / s;   // keep world radius == tipRadius under any anchor scale
            }
            foreach (var col in GetComponents<Collider>()) col.isTrigger = true;

            // Auto-locate the visible tip if it wasn't wired in the inspector: the only
            // child mesh under a Poke Tip is the static marker sphere. Finding an existing
            // child is runtime-safe — no object/collider creation under the XR rig.
            if (visibleTip == null)
            {
                var mr = GetComponentInChildren<MeshRenderer>();
                if (mr != null && mr.transform != transform) visibleTip = mr.transform;
            }

            // Co-locate the visible ball with the trigger (it's a child of this object).
            if (visibleTip != null) visibleTip.localPosition = tipCenter;
        }

        static float MaxAbs(Vector3 v) =>
            Mathf.Max(Mathf.Abs(v.x), Mathf.Max(Mathf.Abs(v.y), Mathf.Abs(v.z)));

        void OnTriggerEnter(Collider other)
        {
            if (Time.unscaledTime < _nextAllowed) return;
            var recv = other.GetComponentInParent<IPokeReceiver>();
            if (recv == null) return;
            _nextAllowed = Time.unscaledTime + debounce;
            recv.OnPoke();
        }
    }

    public class MouseRaySelector : MonoBehaviour
    {
        [Tooltip("Camera to ray from. Defaults to Camera.main.")]
        public Camera cam;
        public float maxDistance = 20f;

#if ENABLE_LEGACY_INPUT_MANAGER
        void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var c = cam != null ? cam : Camera.main;
            if (c == null) return;
            var ray = c.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, maxDistance, ~0, QueryTriggerInteraction.Collide))
            {
                var recv = hit.collider.GetComponentInParent<IPokeReceiver>();
                recv?.OnPoke();
            }
        }
#endif
    }
}

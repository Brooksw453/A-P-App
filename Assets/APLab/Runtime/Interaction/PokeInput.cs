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
        [Tooltip("Re-arm delay so one touch fires once.")]
        public float debounce = 0.3f;
        float _nextAllowed;

        void Reset()    => Configure();
        void Awake()    => Configure();

        void Configure()
        {
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            foreach (var col in GetComponents<Collider>()) col.isTrigger = true;
        }

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

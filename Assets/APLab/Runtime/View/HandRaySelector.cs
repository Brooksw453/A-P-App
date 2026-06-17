// A&P Lab — hand-tracking RAY input.
//
// For each tracked hand, casts a ray from the system pointer pose. A PINCH selects:
//   • bones / quiz answers -> fires our existing IPokeReceiver.OnPoke() (pinch down)
//   • the explode slider    -> pinch-and-hold drags it (continuous)
// Reuses every collider + the Select()/OnPoke() contract we already have — NO Meta
// interactables, NO per-target changes. Works for either hand.
//
// The ray line + cursor are STATIC scene objects built by "A&P Lab/Build Hand Ray" and
// just repositioned here each frame — we never spawn objects at runtime under the XR rig
// (that was the tracking-origin drift / "skull flies up" cause).
//
// Pointer pose + pinch come from OVRHand (added by the Hand Tracking building block), so
// the actual sensing is Meta's tuned data, not ours.

using UnityEngine;

namespace APLab.View
{
    public class HandRaySelector : MonoBehaviour
    {
        [Header("Visuals (built by A&P Lab/Build Hand Ray)")]
        public LineRenderer[] rays;
        public Transform[] cursors;

        [Header("Tuning")]
        public float maxDistance = 6f;
        [Tooltip("Pinch strength to ENGAGE a click/drag.")]
        public float pinchOn = 0.75f;
        [Tooltip("Pinch strength to RELEASE (lower than pinchOn = hysteresis, kills flicker).")]
        public float pinchOff = 0.4f;

        OVRHand[] _hands;
        bool[] _pinching;
        readonly RaycastHit[] _hits = new RaycastHit[16];
        BoneTarget _hovered;

        void Start() => FindHands();

        void FindHands()
        {
            _hands = FindObjectsByType<OVRHand>(FindObjectsSortMode.None);
            _pinching = new bool[_hands.Length];
        }

        void Update()
        {
            if (_hands == null || _hands.Length == 0)
            {
                FindHands();
                if (_hands.Length == 0) { HideAll(); return; }
            }

            for (int h = 0; h < _hands.Length; h++)
            {
                var hand = _hands[h];
                var line = (rays != null && h < rays.Length) ? rays[h] : null;
                var cursor = (cursors != null && h < cursors.Length) ? cursors[h] : null;

                if (hand == null || !hand.IsTracked || !hand.IsPointerPoseValid)
                {
                    SetRay(line, cursor, false, Vector3.zero, Vector3.zero);
                    continue;
                }

                var pose = hand.PointerPose;
                Vector3 origin = pose.position, dir = pose.forward;

                // pinch with hysteresis
                float strength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
                bool was = _pinching[h];
                bool now = was ? strength > pinchOff : strength > pinchOn;
                _pinching[h] = now;
                bool pinchDown = now && !was;

                // nearest valid target along the ray (skip the controller poke tip)
                IPokeReceiver recv = null; PokeSlider slider = null;
                Vector3 point = origin + dir * maxDistance;
                int n = Physics.RaycastNonAlloc(new Ray(origin, dir), _hits, maxDistance, ~0, QueryTriggerInteraction.Collide);
                float best = float.MaxValue;
                for (int i = 0; i < n; i++)
                {
                    var c = _hits[i].collider;
                    if (c.GetComponentInParent<PokeTip>() != null) continue;
                    var s = c.GetComponentInParent<PokeSlider>();
                    var r = c.GetComponentInParent<IPokeReceiver>();
                    if (s == null && r == null) continue;
                    if (_hits[i].distance < best)
                    {
                        best = _hits[i].distance;
                        point = _hits[i].point;
                        slider = s;
                        recv = (s == null) ? r : null;
                    }
                }

                if (slider != null)
                {
                    if (now) slider.DriveFromWorldPoint(point);     // pinch-hold drag
                    Hover(null);
                }
                else if (recv != null)
                {
                    Hover(recv as BoneTarget);
                    if (pinchDown) recv.OnPoke();
                }
                else Hover(null);

                SetRay(line, cursor, true, origin, point);
            }
        }

        void Hover(BoneTarget bt)
        {
            if (_hovered == bt) return;
            if (_hovered != null) _hovered.SetHover(false);
            _hovered = bt;
            if (_hovered != null) _hovered.SetHover(true);
        }

        void SetRay(LineRenderer line, Transform cursor, bool on, Vector3 a, Vector3 b)
        {
            if (line != null)
            {
                line.enabled = on;
                if (on) { line.positionCount = 2; line.SetPosition(0, a); line.SetPosition(1, b); }
            }
            if (cursor != null)
            {
                cursor.gameObject.SetActive(on);
                if (on) cursor.position = b;
            }
        }

        void HideAll()
        {
            if (rays != null) foreach (var l in rays) if (l != null) l.enabled = false;
            if (cursors != null) foreach (var c in cursors) if (c != null) c.gameObject.SetActive(false);
        }
    }
}

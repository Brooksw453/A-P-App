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

        [Header("Cursor feedback (where the ray is pointing)")]
        [Tooltip("Cursor size when NOT over a selectable target.")]
        public float cursorBaseSize = 0.03f;
        [Tooltip("Cursor size when OVER a bone / quiz answer / slider (grows = obvious).")]
        public float cursorHoverSize = 0.052f;
        public Color cursorBaseColor = new Color(0.55f, 0.85f, 1f, 1f);
        [Tooltip("Cursor + ray colour when over a selectable target.")]
        public Color cursorHoverColor = new Color(0.40f, 1f, 0.55f, 1f);

        [Header("Hand gestures (manipulate the skull directly — in addition to the sliders)")]
        [Tooltip("Enable one-hand grab-to-rotate + two-hand pull-apart-to-explode. PARKED (off) for now — " +
                 "during Practice every bone is armed, so an empty-space pinch was hard to isolate from bone " +
                 "selection (kept selecting bones). Revisit if/when bones are only selectively armed.")]
        public bool enableGestures = false;
        [Tooltip("Turntable driven by one-hand grab. Auto-found if empty.")]
        public SkullRotator rotator;
        [Tooltip("Explode driven by two-hand pull-apart. Auto-found if empty.")]
        public SkullExploder exploder;
        [Tooltip("Degrees of spin per metre of horizontal hand motion (negative to invert).")]
        public float spinSensitivity = 220f;
        [Tooltip("Degrees of tilt per metre of vertical hand motion (negative to invert).")]
        public float tiltSensitivity = 220f;
        [Tooltip("Explode-factor (0..1) change per metre of two-hand spread (negative to invert).")]
        public float explodeSensitivity = 2.2f;

        OVRHand[] _hands;
        bool[] _pinching;
        readonly RaycastHit[] _hits = new RaycastHit[16];
        IRayHoverable[] _hovered;   // per hand — both rays highlight independently

        // gesture layer state (set in Update's per-hand loop, consumed by HandleGestures)
        enum Gesture { None, Rotate, Explode }
        Gesture _gesture = Gesture.None;
        bool[] _free;            // per hand: pinch is free (not on a slider or an armed target)
        Vector3[] _handPos;      // per hand: pointer-pose position this frame
        int _rotateHand = -1;
        Vector3 _lastRotatePos;
        float _explodeBaseDist, _explodeBaseFactor;
        Camera _cam;
        MaterialPropertyBlock _curMpb;
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");

        void Start() => FindHands();

        void FindHands()
        {
            _hands = FindObjectsByType<OVRHand>(FindObjectsSortMode.None);
            _pinching = new bool[_hands.Length];
            _hovered = new IRayHoverable[_hands.Length];
            _free = new bool[_hands.Length];
            _handPos = new Vector3[_hands.Length];
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
                    Hover(h, null);                                 // drop this hand's highlight if it loses tracking
                    _free[h] = false;
                    SetRay(line, cursor, false, Vector3.zero, Vector3.zero, false);
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

                bool overTarget = false;
                if (slider != null)
                {
                    if (now) slider.DriveFromWorldPoint(point);     // pinch-hold drag
                    Hover(h, null);
                    overTarget = true;                              // the slider is a target too
                }
                else if (recv != null)
                {
                    Hover(h, recv as IRayHoverable);                // bones AND quiz answers highlight
                    if (pinchDown) recv.OnPoke();
                    overTarget = true;
                }
                else Hover(h, null);

                // record this hand's pinch availability + position for the gesture layer:
                // a pinch on a slider or an ARMED target is NOT free (it selects/drags instead).
                bool armedTarget = recv != null &&
                    ((recv is BoneTarget abt && abt.Armed) || (recv is QuizOption aqo && aqo.Armed));
                _free[h] = now && slider == null && !armedTarget;
                _handPos[h] = origin;

                SetRay(line, cursor, true, origin, point, overTarget);
            }

            if (enableGestures) HandleGestures();
        }

        // Per-hand hover so BOTH rays highlight independently. (A single shared field let the
        // last-iterated hand overwrite it every frame, so only one ray's target ever glowed —
        // that was the "right ray doesn't highlight" bug.) When a hand leaves a target, only
        // clear its glow if the OTHER hand isn't still pointing at it.
        void Hover(int hand, IRayHoverable h)
        {
            var prev = _hovered[hand];
            if (ReferenceEquals(prev, h)) return;
            _hovered[hand] = h;
            if (prev != null && !StillHovered(prev, hand)) prev.SetHover(false);
            if (h != null) h.SetHover(true);
        }

        bool StillHovered(IRayHoverable t, int exceptHand)
        {
            for (int i = 0; i < _hovered.Length; i++)
                if (i != exceptHand && ReferenceEquals(_hovered[i], t)) return true;
            return false;
        }

        void SetRay(LineRenderer line, Transform cursor, bool on, Vector3 a, Vector3 b, bool overTarget)
        {
            Color c = overTarget ? cursorHoverColor : cursorBaseColor;
            if (line != null)
            {
                line.enabled = on;
                if (on)
                {
                    line.positionCount = 2; line.SetPosition(0, a); line.SetPosition(1, b);
                    line.startColor = line.endColor = c;            // ray brightens on a target
                }
            }
            if (cursor != null)
            {
                cursor.gameObject.SetActive(on);
                if (on)
                {
                    cursor.position = b;
                    cursor.localScale = Vector3.one * (overTarget ? cursorHoverSize : cursorBaseSize);
                    ApplyCursorColor(cursor, c);                    // grows + recolours = obvious hover
                }
            }
        }

        void ApplyCursorColor(Transform cursor, Color c)
        {
            var r = cursor.GetComponent<Renderer>();
            if (r == null) return;
            _curMpb ??= new MaterialPropertyBlock();
            r.GetPropertyBlock(_curMpb);
            _curMpb.SetColor(BaseColorId, c);
            _curMpb.SetColor(ColorId, c);
            r.SetPropertyBlock(_curMpb);
        }

        void HideAll()
        {
            if (rays != null) foreach (var l in rays) if (l != null) l.enabled = false;
            if (cursors != null) foreach (var c in cursors) if (c != null) c.gameObject.SetActive(false);
            if (_hovered != null)
                for (int i = 0; i < _hovered.Length; i++)
                {
                    if (_hovered[i] != null) _hovered[i].SetHover(false);
                    _hovered[i] = null;
                }
            if (_free != null) for (int i = 0; i < _free.Length; i++) _free[i] = false;
            _gesture = Gesture.None;
            _rotateHand = -1;
        }

        // ===== Hand gestures: one-hand grab-to-rotate, two-hand pull-apart to explode =====
        // A pinch that isn't on a slider or an ARMED target is "free" and feeds this layer, so
        // selecting an armed bone / quiz answer never fights it:
        //   • two free hands -> the inter-hand distance drives the SkullExploder (apart = explode,
        //     together = reassemble)
        //   • one  free hand -> the hand's motion across the view drives spin (horizontal) + tilt
        //     (vertical), via the SkullRotator.
        void HandleGestures()
        {
            if (_free == null) return;
            if (rotator == null)  rotator  = FindFirstObjectByType<SkullRotator>(FindObjectsInactive.Include);
            if (exploder == null) exploder = FindFirstObjectByType<SkullExploder>(FindObjectsInactive.Include);
            if (_cam == null) { _cam = Camera.main; if (_cam == null) _cam = FindFirstObjectByType<Camera>(); }

            int a = -1, b = -1, count = 0;
            for (int h = 0; h < _free.Length; h++)
                if (_free[h]) { if (a < 0) a = h; else if (b < 0) b = h; count++; }

            if (count >= 2 && exploder != null)
            {
                float d = Vector3.Distance(_handPos[a], _handPos[b]);
                if (_gesture != Gesture.Explode)
                {
                    _gesture = Gesture.Explode;          // grab — baseline the spread + current factor
                    _explodeBaseDist = d;
                    _explodeBaseFactor = exploder.factor;
                }
                else exploder.SetFactor(_explodeBaseFactor + (d - _explodeBaseDist) * explodeSensitivity);
            }
            else if (count == 1 && rotator != null && _cam != null)
            {
                if (_gesture != Gesture.Rotate || _rotateHand != a)
                {
                    _gesture = Gesture.Rotate;           // grab — baseline, no rotation applied this frame
                    _rotateHand = a;
                    _lastRotatePos = _handPos[a];
                }
                else
                {
                    Vector3 delta = _handPos[a] - _lastRotatePos;
                    _lastRotatePos = _handPos[a];
                    float dx = Vector3.Dot(delta, _cam.transform.right);   // horizontal hand motion -> spin
                    float dy = Vector3.Dot(delta, _cam.transform.up);      // vertical hand motion   -> tilt
                    rotator.AddSpin(-dx * spinSensitivity);
                    rotator.AddTilt(dy * tiltSensitivity);
                }
            }
            else
            {
                _gesture = Gesture.None;
                _rotateHand = -1;
            }
        }
    }
}

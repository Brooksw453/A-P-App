// A&P Lab — a world-space, poke-draggable slider.
//
// While the PokeTip is inside the slider's trigger zone, the handle follows the finger
// along the local X track and the 0..1 value drives a target (here, a SkullExploder:
// 0 = assembled, 1 = exploded). "Touch anywhere along the track to set it" — forgiving
// for VR poke. A mouse fallback (legacy Input) lets you drive it in the editor too.
//
// Built in the editor by "A&P Lab/Build Explode Slider" as STATIC scene objects — never
// spawned at runtime under the XR rig (that caused the tracking-origin drift).

using UnityEngine;

namespace APLab.View
{
    [RequireComponent(typeof(Collider))]
    public class PokeSlider : MonoBehaviour
    {
        public enum Drive { Exploder, RotatorSpin, RotatorTilt }

        [Tooltip("What this slider controls. Exploder = open/close the skull; " +
                 "RotatorSpin / RotatorTilt = turntable spin / tilt.")]
        public Drive drive = Drive.Exploder;

        [Tooltip("Exploder this slider drives when drive = Exploder (0 = assembled, 1 = exploded).")]
        public SkullExploder exploder;
        [Tooltip("Rotator this slider drives when drive = RotatorSpin / RotatorTilt.")]
        public SkullRotator rotator;
        [Tooltip("The knob that slides along the track (moved along local X).")]
        public Transform handle;
        [Tooltip("Optional fill bar (scaled along local X from the left end to the handle).")]
        public Transform fill;
        [Tooltip("Half the track length in local-X metres (handle travels -half .. +half).")]
        public float halfLength = 0.15f;

        [Range(0f, 1f)] public float value = 1f;

        void OnEnable()
        {
            value = CurrentValue();   // start the handle where the driven target already is
            ApplyVisual();
        }

        // Read the current 0..1 value back from whatever this slider drives, so the handle/fill
        // initialise in the right place (explode amount, or the rotator's spin/tilt angle).
        float CurrentValue()
        {
            switch (drive)
            {
                case Drive.RotatorSpin: return rotator != null ? Mathf.InverseLerp(-180f, 180f, rotator.spin) : value;
                case Drive.RotatorTilt: return rotator != null ? Mathf.InverseLerp(rotator.tiltMin, rotator.tiltMax, rotator.tilt) : value;
                default:                return exploder != null ? Mathf.Clamp01(exploder.factor) : value;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.GetComponentInParent<PokeTip>() == null) return;   // only the poke tip drives it
            DriveFromWorldPoint(other.bounds.center);
        }

        public void DriveFromWorldPoint(Vector3 world)   // public so the hand-ray can drive it too
        {
            Vector3 local = transform.InverseTransformPoint(world);
            float x = Mathf.Clamp(local.x, -halfLength, halfLength);
            SetValue(Mathf.InverseLerp(-halfLength, halfLength, x));
        }

        public void SetValue(float v)
        {
            value = Mathf.Clamp01(v);
            switch (drive)
            {
                case Drive.RotatorSpin: if (rotator != null) rotator.SetSpin01(value); break;
                case Drive.RotatorTilt: if (rotator != null) rotator.SetTilt01(value); break;
                default:                if (exploder != null) exploder.SetFactor(value); break;
            }
            ApplyVisual();
        }

        void ApplyVisual()
        {
            float x = Mathf.Lerp(-halfLength, halfLength, value);
            if (handle != null)
            {
                var lp = handle.localPosition;
                handle.localPosition = new Vector3(x, lp.y, lp.z);
            }
            if (fill != null)
            {
                float len = Mathf.Max(0.0001f, x + halfLength);   // from left end (-half) to handle
                var s = fill.localScale; fill.localScale = new Vector3(len, s.y, s.z);
                var fp = fill.localPosition; fill.localPosition = new Vector3(-halfLength + len * 0.5f, fp.y, fp.z);
            }
        }

        bool _dragging;
        void Update()
        {
            // Keep the handle/fill synced when SOMETHING ELSE drives the same target (e.g. a hand
            // gesture driving the SkullExploder / SkullRotator), so the slider never shows a stale
            // position. Idempotent when the slider itself is the driver.
            if (!_dragging)
            {
                float cur = CurrentValue();
                if (Mathf.Abs(cur - value) > 0.0005f) { value = cur; ApplyVisual(); }
            }
#if ENABLE_LEGACY_INPUT_MANAGER
            var cam = Camera.main;
            if (cam == null) return;
            if (Input.GetMouseButtonDown(0))
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                if (GetComponent<Collider>().Raycast(ray, out _, 100f)) _dragging = true;
            }
            if (Input.GetMouseButtonUp(0)) _dragging = false;
            if (_dragging)
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                var plane = new Plane(transform.forward, transform.position);
                if (plane.Raycast(ray, out float d)) DriveFromWorldPoint(ray.GetPoint(d));
            }
#endif
        }
    }
}

// A&P Lab — turntable rotation for the exploding skull.
//
// Lives on the skull root ("Skull (Exploding)") so the WHOLE hierarchy — every bone, its
// trigger collider, and the SkullExploder's per-bone offsets — turns rigidly as one. Because
// SkullExploder works in each bone's PARENT-LOCAL space (localPosition), rotating the root
// doesn't disturb the explode at all, and the bone colliders ride along so ray/poke selection
// stays perfectly aligned after a spin (that was the whole reason rotation goes on the root).
//
//   spin = yaw around world-up   (the turntable platter — see every side, incl. the back)
//   tilt = pitch around world-right (nod the skull toward/away from you — see the cranial
//          vault on top and the base/foramen magnum underneath)
//
// Both apply ON TOP of a captured rest pose (homeRotation), so spin = tilt = 0 returns the
// skull to its front-facing pose. Driven by the two PokeSliders that "A&P Lab/Build Rotate
// Control" builds — the same proven widget the HandRaySelector already pinch-drags, so this
// needs zero changes to the ray input and never fights bone selection.

using UnityEngine;

namespace APLab.View
{
    public class SkullRotator : MonoBehaviour
    {
        [Header("Current angles (degrees)")]
        [Range(-180f, 180f)] public float spin = 0f;   // yaw — turntable
        [Range(-89f, 89f)]   public float tilt = 0f;    // pitch — look up / down

        [Header("Tilt range the slider maps across (swap min/max to invert tilt direction)")]
        public float tiltMin = -80f;
        public float tiltMax = 80f;

        [Header("Rest pose (captured by Build Rotate Control — don't edit by hand)")]
        [Tooltip("The skull's resting rotation; spin/tilt are applied on top of this.")]
        public Quaternion homeRotation = Quaternion.identity;
        public bool homeSet = false;

        void OnEnable()   { if (!homeSet) CaptureHome(); Apply(); }
        void OnValidate() { if (homeSet) Apply(); }      // live-scrub spin/tilt in the inspector

        /// <summary>Record the current localRotation as the rest pose (front-facing, no spin/tilt).</summary>
        public void CaptureHome()
        {
            homeRotation = transform.localRotation;
            homeSet = true;
        }

        /// <summary>Drive spin from a 0..1 slider value (0 = -180°, 1 = +180°; 0.5 = front).</summary>
        public void SetSpin01(float v01) { spin = Mathf.Lerp(-180f, 180f, Mathf.Clamp01(v01)); Apply(); }

        /// <summary>Drive tilt from a 0..1 slider value across [tiltMin..tiltMax] (0.5 = level).</summary>
        public void SetTilt01(float v01) { tilt = Mathf.Lerp(tiltMin, tiltMax, Mathf.Clamp01(v01)); Apply(); }

        /// <summary>Add to spin (degrees), wrapping into −180..180. For one-hand grab-to-rotate.</summary>
        public void AddSpin(float deltaDeg) { spin = Mathf.Repeat(spin + deltaDeg + 180f, 360f) - 180f; Apply(); }

        /// <summary>Add to tilt (degrees), clamped to [tiltMin, tiltMax]. For one-hand grab-to-rotate.</summary>
        public void AddTilt(float deltaDeg) { tilt = Mathf.Clamp(tilt + deltaDeg, tiltMin, tiltMax); Apply(); }

        // Spin around world-up, THEN tilt around world-right, on top of the rest pose — so tilt
        // always nods toward the viewer no matter how far it's been spun. (Assumes the skull
        // root's parent is unrotated, which it is in Module_Skeletal.)
        public void Apply()
        {
            if (!homeSet) return;
            transform.localRotation =
                Quaternion.AngleAxis(tilt, Vector3.right) *
                Quaternion.AngleAxis(spin, Vector3.up) *
                homeRotation;
        }
    }
}

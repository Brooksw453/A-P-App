// A&P Lab — procedural skull "explode" controller.
// Separates each bone of the exploding skull radially outward from the skull's centre
// by a 0..1 factor, so bones de-cluster (each individually pokeable) and the internal /
// posterior bones (sphenoid, ethmoid, vomer, occipital...) become reachable. Fully
// reversible: factor 0 = assembled (every bone's home doubles as its socket), 1 = fully
// exploded.
//
// Set up by the editor menu "A&P Lab/Setup Exploding Skull", which fills `parts` with
// each mesh's home localPosition + radial direction. Animate `factor` at runtime to
// open/close the skull; the later place-the-bone-in-socket step just returns a bone home.

using System.Collections.Generic;
using UnityEngine;

namespace APLab.View
{
    public class SkullExploder : MonoBehaviour
    {
        [System.Serializable]
        public class Part
        {
            public Transform t;
            public Vector3 home;   // assembled localPosition (also the bone's "socket")
            public Vector3 dir;    // radial offset in the bone's parent-local space
        }

        [Range(0f, 1f)] public float factor = 1f;
        [Tooltip("How far bones fan out, as a multiple of each bone's distance from the skull centre.")]
        public float spread = 2f;
        public List<Part> parts = new List<Part>();

        float _lastFactor = float.NaN;
        float _lastSpread = float.NaN;

        void OnEnable()   => Apply();
        void OnValidate() => Apply();

        void Update()
        {
            if (!Mathf.Approximately(factor, _lastFactor) || !Mathf.Approximately(spread, _lastSpread))
                Apply();
        }

        /// <summary>Set the explode amount (0 = assembled .. 1 = exploded) and apply now.</summary>
        public void SetFactor(float f)
        {
            factor = Mathf.Clamp01(f);
            Apply();
        }

        public void Apply()
        {
            if (parts == null) return;
            _lastFactor = factor;
            _lastSpread = spread;
            float k = spread * Mathf.Clamp01(factor);
            foreach (var p in parts)
                if (p != null && p.t != null)
                    p.t.localPosition = p.home + p.dir * k;
        }
    }
}

// A&P Lab — keeps a bone label OUTSIDE the skull, with a leader line to its bone.
//
// The label is NOT a child of the (scaled) skull — it lives under a world-scale "Bone Labels"
// root — so its text size stays constant. Each LateUpdate it sits along a FIXED outward
// direction from the skull centre (captured at build time, when the bones are spread, so the
// directions are well distributed), at radius max(minRadius, boneRadius + gap):
//   • assembled  -> the bone is near the centre, so the label rests just outside the compact
//     skull (minRadius) — readable even when nothing is exploded (the old labels were buried).
//   • exploded   -> the bone is far out, so the label rides just beyond it.
// A leader line runs from the label to the bone's visual centre, so it always points accurately
// at the bone and tracks it as the skull explodes / reassembles.
//
// Hides itself (text + leader) whenever its bone is hidden (e.g. the skull is off during the
// quiz), so label visibility needs no wiring in ModuleHost. Pair with LabelBillboard.

using UnityEngine;
using TMPro;

namespace APLab.View
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class LabelFollow : MonoBehaviour
    {
        [Tooltip("The bone this label names (label parks outside it; leader line points to it).")]
        public Transform bone;
        [Tooltip("Skull root — the centre the label is pushed radially out from.")]
        public Transform skullCenter;

        [Tooltip("Outward direction in the SKULL's local frame (captured + decluttered at build). It's rotated " +
                 "by the skull each frame, so labels turn WITH the skull and stay by their bones when you spin it.")]
        public Vector3 outwardDir = Vector3.up;
        [Tooltip("Label never sits closer to the centre than this — keeps it OUTSIDE the assembled skull.")]
        public float minRadius = 0.45f;
        [Tooltip("Extra metres the label sits beyond the bone / the min radius.")]
        public float gap = 0.12f;

        [Tooltip("Leader line from the label to the bone (optional).")]
        public LineRenderer leader;

        TMP_Text _text;
        Renderer _boneRenderer;

        void Awake()     { _text = GetComponent<TMP_Text>(); CacheBone(); }
        void OnEnable()  { CacheBone(); }
        void CacheBone() { if (bone != null) _boneRenderer = bone.GetComponent<Renderer>(); }

        void LateUpdate()
        {
            if (bone == null) return;

            // hide label + leader whenever the bone is hidden (e.g. the skull is off in the quiz)
            bool show = bone.gameObject.activeInHierarchy;
            if (_text == null) _text = GetComponent<TMP_Text>();
            if (_text != null && _text.enabled != show) _text.enabled = show;
            if (leader != null && leader.enabled != show) leader.enabled = show;
            if (!show) return;

            Vector3 center = skullCenter != null ? skullCenter.position : bone.position;
            if (_boneRenderer == null) CacheBone();
            Vector3 boneC = _boneRenderer != null ? _boneRenderer.bounds.center : bone.position;

            // outwardDir is in the skull's local frame -> rotate it by the skull so the label turns
            // WITH the skull (stays next to its bone when you spin it; the old world-space dir left
            // the labels behind and stretched the leader lines).
            Vector3 dir;
            if (outwardDir.sqrMagnitude > 1e-6f)
                dir = (skullCenter != null ? skullCenter.rotation * outwardDir : outwardDir).normalized;
            else
                dir = (boneC - center).sqrMagnitude > 1e-6f ? (boneC - center).normalized : Vector3.up;
            float boneRadius = Vector3.Distance(center, boneC);
            float r = Mathf.Max(minRadius, boneRadius + gap);
            Vector3 labelPos = center + dir * r;
            transform.position = labelPos;

            if (leader != null)
            {
                leader.SetPosition(0, labelPos);
                leader.SetPosition(1, boneC);
            }
        }
    }
}

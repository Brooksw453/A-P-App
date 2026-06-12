// A&P Lab — content carried by a generated anatomy label, sourced from the
// module content JSON (e.g. skeletal-system.json). The Practice / "tap the bone"
// interaction (next milestone) reads these to score and to show definitions.

using UnityEngine;

namespace APLab.View
{
    [DisallowMultipleComponent]
    public class LabelInfo : MonoBehaviour
    {
        [Tooltip("Matches the content JSON anchorName and the model's bone/part.")]
        public string anchorName;
        public string term;
        public string pronunciation;
        [TextArea(1, 4)] public string definition;

        [Tooltip("Landmark on the model this label points to (world space).")]
        public Vector3 landmark;

        [Tooltip("Set when the landmark is an approximate/deep placement needing review.")]
        public bool approximate;
    }
}

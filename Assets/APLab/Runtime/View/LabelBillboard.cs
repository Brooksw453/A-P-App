// A&P Lab — keeps a world-space label turned toward the viewer.
// TMP text's readable face points local -Z, so we aim local +Z along the
// camera->label direction (readable side then faces back toward the camera).
// Active at runtime (the headset). In the Editor it no-ops unless
// previewInEditor is set, so generated labels stay put for static framing.

using UnityEngine;

namespace APLab.View
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class LabelBillboard : MonoBehaviour
    {
        [Tooltip("Camera to face. Falls back to Camera.main, then Camera.current.")]
        public Camera target;

        [Tooltip("Yaw-only: keep text upright instead of tilting with the camera.")]
        public bool lockUpright = true;

        [Tooltip("Also billboard while in the Editor (off = labels hold their generated pose).")]
        public bool previewInEditor = false;

        void LateUpdate()
        {
            if (!Application.isPlaying && !previewInEditor) return;

            var cam = target != null ? target
                    : (Camera.main != null ? Camera.main : Camera.current);
            if (cam == null) return;

            Vector3 dir = transform.position - cam.transform.position; // camera -> label
            if (lockUpright) dir.y = 0f;                                // keep text vertical
            if (dir.sqrMagnitude < 1e-8f) return;

            transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }
    }
}

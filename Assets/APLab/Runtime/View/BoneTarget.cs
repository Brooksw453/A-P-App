// A&P Lab — a pokeable/selectable bone target.
// Lives on a label's Marker sphere. Carries the bone's anchorName and raises
// Selected when the learner pokes it (PokeTip) or it is hit by the ray/mouse
// selector. Decoupled from any specific XR toolkit: anything can call Select().
//
// The ModuleHost arms targets only during the relevant Practice step, so stray
// pokes elsewhere are ignored.

using System;
using System.Collections;
using UnityEngine;

namespace APLab.View
{
    [RequireComponent(typeof(Collider))]
    public class BoneTarget : MonoBehaviour, IPokeReceiver
    {
        public string anchorName;

        /// <summary>True while this target is a valid answer to poke right now.</summary>
        public bool Armed { get; private set; }

        public event Action<BoneTarget> Selected;

        static readonly Color Dim    = new Color(0.55f, 0.45f, 0.18f, 1f); // opaque: URP Unlit ignores alpha
        static readonly Color Armed_ = new Color(1f, 0.85f, 0.3f, 1f);
        static readonly Color Right  = new Color(0.35f, 1f, 0.45f, 1f);
        static readonly Color Wrong  = new Color(1f, 0.35f, 0.3f, 1f);

        Renderer _renderer;
        Coroutine _flash;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            ApplyColor(Dim);
        }

        public void SetArmed(bool on)
        {
            Armed = on;
            if (_flash == null) ApplyColor(on ? Armed_ : Dim);
        }

        /// <summary>Invoked by any input source (poke tip, ray, mouse).</summary>
        public void Select()
        {
            if (!Armed) return;
            Selected?.Invoke(this);
        }

        // IPokeReceiver
        public void OnPoke() => Select();

        public void FlashCorrect() => Flash(Right);
        public void FlashWrong()   => Flash(Wrong);

        void Flash(Color c)
        {
            if (_flash != null) StopCoroutine(_flash);
            _flash = StartCoroutine(FlashRoutine(c));
        }

        IEnumerator FlashRoutine(Color c)
        {
            ApplyColor(c);
            yield return new WaitForSeconds(0.6f);
            _flash = null;
            ApplyColor(Armed ? Armed_ : Dim);
        }

        void ApplyColor(Color c)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.sharedMaterial != null)
                _renderer.sharedMaterial.color = c;
        }
    }
}

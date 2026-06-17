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

        [Tooltip("Mesh mode: this target IS a real bone mesh (the exploding skull), not a " +
                 "marker dot. Leaves the bone's natural material alone and highlights a " +
                 "correct/wrong poke via a MaterialPropertyBlock, instead of recolouring the " +
                 "shared skull material.")]
        public bool meshMode = false;

        /// <summary>True while this target is a valid answer to poke right now.</summary>
        public bool Armed { get; private set; }

        public event Action<BoneTarget> Selected;

        static readonly Color Dim    = new Color(0.55f, 0.45f, 0.18f, 1f); // opaque: URP Unlit ignores alpha
        static readonly Color Armed_ = new Color(1f, 0.85f, 0.3f, 1f);
        static readonly Color Right  = new Color(0.35f, 1f, 0.45f, 1f);
        static readonly Color Wrong  = new Color(1f, 0.35f, 0.3f, 1f);
        static readonly Color Hover  = new Color(0.45f, 0.85f, 1f, 1f);   // ray hover (mesh mode)

        Renderer _renderer;
        Coroutine _flash;
        MaterialPropertyBlock _mpb;
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (!meshMode) ApplyColor(Dim);     // mesh mode keeps the bone's natural material
        }

        public void SetArmed(bool on)
        {
            Armed = on;
            if (meshMode) return;               // bones stay natural; every bone is armed during practice
            if (_flash == null) ApplyColor(on ? Armed_ : Dim);
        }

        /// <summary>Ray-hover highlight (mesh mode only); ignored while a flash is playing.</summary>
        public void SetHover(bool on)
        {
            if (!meshMode || _flash != null) return;
            if (on) SetBlockColor(Hover); else ClearBlock();
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
            _flash = StartCoroutine(meshMode ? MeshFlashRoutine(c) : FlashRoutine(c));
        }

        IEnumerator FlashRoutine(Color c)
        {
            ApplyColor(c);
            yield return new WaitForSeconds(0.6f);
            _flash = null;
            ApplyColor(Armed ? Armed_ : Dim);
        }

        // Mesh mode: tint the bone briefly via a MaterialPropertyBlock (per-renderer, so it
        // never clobbers the shared skull material), then clear back to its natural look.
        IEnumerator MeshFlashRoutine(Color c)
        {
            SetBlockColor(c);
            yield return new WaitForSeconds(0.6f);
            _flash = null;
            ClearBlock();
        }

        void SetBlockColor(Color c)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer == null) return;
            _mpb ??= new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(BaseColorId, c);
            _mpb.SetColor(ColorId, c);
            _renderer.SetPropertyBlock(_mpb);
        }

        void ClearBlock()
        {
            if (_renderer != null) _renderer.SetPropertyBlock(null);
        }

        void ApplyColor(Color c)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.sharedMaterial != null)
                _renderer.sharedMaterial.color = c;
        }
    }
}

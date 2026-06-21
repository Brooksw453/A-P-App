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
    public class BoneTarget : MonoBehaviour, IPokeReceiver, IRayHoverable
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
        static readonly Color Hover  = new Color(0.55f, 0.9f, 1f, 1f);    // ray hover — glows via emission
        static readonly Color Selected_ = new Color(0.30f, 1f, 0.45f, 1f); // persistent click-select (Explore mode)

        Renderer _renderer;
        Coroutine _flash;
        MaterialPropertyBlock _mpb;
        bool _emissionReady;
        bool _selected;                  // held green (Explore); survives hover changes
        const float EmissionBoost = 1.4f;
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");
        static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

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

        /// <summary>Ray-hover highlight (mesh mode only); ignored while a flash is playing.
        /// Un-hovering restores the held green if this bone is currently selected (Explore).</summary>
        public void SetHover(bool on)
        {
            if (!meshMode || _flash != null) return;
            if (on) SetBlockColor(Hover);
            else if (_selected) SetBlockColor(Selected_);
            else ClearBlock();
        }

        /// <summary>True while this bone is the current Explore-mode selection.</summary>
        public bool IsSelected => _selected;

        /// <summary>Persistent green "selected" highlight for Explore mode. Held until cleared
        /// (single-selection is managed by LabModeController). Distinct from the transient
        /// correct/wrong flash and from the cyan hover.</summary>
        public void SetSelected(bool on)
        {
            _selected = on;
            if (!meshMode || _flash != null) return;   // a flash, if any, restores the right state on finish
            if (on) SetBlockColor(Selected_); else ClearBlock();
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
            if (_selected) SetBlockColor(Selected_);   // a selected bone keeps its green after a flash
            else ClearBlock();
        }

        void SetBlockColor(Color c)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer == null) return;
            EnsureEmission();
            _mpb ??= new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(BaseColorId, c);
            _mpb.SetColor(ColorId, c);
            _mpb.SetColor(EmissionColorId, c * EmissionBoost);   // GLOW so the tint reads on the lit bone
            _renderer.SetPropertyBlock(_mpb);
        }

        void ClearBlock()
        {
            if (_renderer != null) _renderer.SetPropertyBlock(null);   // back to natural (emission black)
        }

        // A plain _BaseColor tint just MULTIPLIES the bone's albedo texture, so cyan came out as a
        // muddy darkening that didn't read (the "bones don't turn cyan on hover" bug). Enabling
        // emission lets the hover / correct / wrong colors actually glow. We instance only THIS bone's
        // material (the shared skull material is untouched) and leave emission black at rest — the MPB
        // drives the glow. Harmless if the shader has no _EMISSION keyword. Lazy, on first highlight.
        void EnsureEmission()
        {
            if (_emissionReady) return;
            _emissionReady = true;
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer == null) return;
            var mat = _renderer.material;                 // per-renderer instance
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            mat.SetColor(EmissionColorId, Color.black);
        }

        void ApplyColor(Color c)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.sharedMaterial != null)
                _renderer.sharedMaterial.color = c;
        }
    }
}

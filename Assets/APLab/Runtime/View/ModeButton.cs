// A&P Lab — a pokeable mode-toggle button (Explore / Info Quiz / Bone Quiz).
//
// Same input contract as QuizOption/BoneTarget: a trigger collider + IPokeReceiver +
// IRayHoverable, so the existing HandRaySelector (ray pinch) and PokeTip (touch) drive
// it with ZERO extra wiring. Unlike QuizOption it's ALWAYS live (no "armed" gate) — the
// mode bar must always be reachable, including to bail out of a quiz mid-round.
//
// LabModeController owns the modes; this just reports a poke (Picked) and shows
// idle / current-mode / hover states. Built as a STATIC scene object by LabUIBuilder
// (never created at runtime — the XR-rig tracking-origin drift rule).

using System;
using TMPro;
using UnityEngine;

namespace APLab.View
{
    [RequireComponent(typeof(Collider))]
    public class ModeButton : MonoBehaviour, IPokeReceiver, IRayHoverable
    {
        [Tooltip("Which mode this button selects (for debugging/labels). Mapping is by reference in " +
                 "LabModeController, so the exact string isn't load-bearing.")]
        public string modeId;

        [Tooltip("The button's text label (assigned by LabUIBuilder).")]
        public TextMeshPro label;

        [Tooltip("The button background whose material is tinted for state. Defaults to a Renderer " +
                 "on this object; set explicitly when the mesh is a child.")]
        public Renderer background;

        public event Action<ModeButton> Picked;

        static readonly Color Idle    = new Color(0.16f, 0.18f, 0.24f, 1f); // opaque: URP Unlit ignores alpha
        static readonly Color Current = new Color(0.20f, 0.50f, 0.34f, 1f); // the active mode (green)
        static readonly Color HoverC  = new Color(0.34f, 0.62f, 0.92f, 1f); // ray hover (cyan-blue)

        bool _isCurrent;
        MaterialPropertyBlock _mpb;
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");

        void Awake()
        {
            if (background == null) background = GetComponent<Renderer>();
            ApplyColor(Idle);
        }

        /// <summary>Show whether this is the currently-selected mode (held green).</summary>
        public void SetCurrent(bool on)
        {
            _isCurrent = on;
            ApplyColor(on ? Current : Idle);
        }

        /// <summary>Ray-hover highlight via a per-renderer MaterialPropertyBlock; clearing it reverts
        /// to the underlying idle/current color. (Mode buttons are opaque, so this reads directly.)</summary>
        public void SetHover(bool on)
        {
            if (background == null) background = GetComponent<Renderer>();
            if (background == null) return;
            if (on)
            {
                _mpb ??= new MaterialPropertyBlock();
                background.GetPropertyBlock(_mpb);
                _mpb.SetColor(BaseColorId, HoverC);
                _mpb.SetColor(ColorId, HoverC);
                background.SetPropertyBlock(_mpb);
            }
            else background.SetPropertyBlock(null);   // back to idle/current (sharedMaterial color)
        }

        // IPokeReceiver — always live; the mode bar is never disarmed.
        public void OnPoke() => Picked?.Invoke(this);

        void ApplyColor(Color c)
        {
            if (background == null) background = GetComponent<Renderer>();
            if (background != null && background.sharedMaterial != null)
                background.sharedMaterial.color = c;
        }
    }
}

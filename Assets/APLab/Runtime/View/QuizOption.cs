// A&P Lab — a pokeable multiple-choice answer button.
// Same input contract as BoneTarget: a trigger collider + IPokeReceiver, armed
// only while a question is live. The existing PokeTip (and the MouseRaySelector
// fallback) drive it with no extra wiring. QuizPanel owns the round; this just
// reports a poke and shows armed / correct / wrong states.

using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace APLab.View
{
    [RequireComponent(typeof(Collider))]
    public class QuizOption : MonoBehaviour, IPokeReceiver, IRayHoverable
    {
        [Tooltip("Which answer option this is (index into the question's answerOptions).")]
        public int index;

        [Tooltip("The option's text label (assigned by QuizPanelBuilder).")]
        public TextMeshPro label;

        [Tooltip("The button background whose material is tinted for state. Defaults to a " +
                 "Renderer on this object; set explicitly when the mesh is a child.")]
        public Renderer background;

        /// <summary>True while this option can be picked right now.</summary>
        public bool Armed { get; private set; }

        public event Action<QuizOption> Selected;

        static readonly Color Dim    = new Color(0.16f, 0.18f, 0.24f, 1f);
        static readonly Color Armed_ = new Color(0.20f, 0.34f, 0.52f, 1f);
        static readonly Color Right   = new Color(0.22f, 0.55f, 0.30f, 1f);
        static readonly Color Wrong   = new Color(0.60f, 0.22f, 0.20f, 1f);
        static readonly Color HoverC  = new Color(0.34f, 0.62f, 0.92f, 1f);  // ray hover (brighter than armed)

        Coroutine _flash;
        MaterialPropertyBlock _mpb;
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");

        void Awake()
        {
            if (background == null) background = GetComponent<Renderer>();
            ApplyColor(Dim);
        }

        public void SetArmed(bool on)
        {
            Armed = on;
            if (_flash == null) ApplyColor(on ? Armed_ : Dim);
        }

        /// <summary>Ray-hover highlight. Per-renderer (MaterialPropertyBlock) so it only tints THIS
        /// option; clearing it reverts to the current armed/dim color. Ignored mid-flash.</summary>
        public void SetHover(bool on)
        {
            if (_flash != null) return;
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
            else background.SetPropertyBlock(null);
        }

        /// <summary>Invoked by any input source (poke tip, ray, mouse).</summary>
        public void Select()
        {
            if (!Armed) return;
            Selected?.Invoke(this);
        }

        // IPokeReceiver
        public void OnPoke() => Select();

        public void SetVisible(bool on) => gameObject.SetActive(on);

        public void FlashCorrect() => Flash(Right);
        public void FlashWrong()   => Flash(Wrong);

        void Flash(Color c)
        {
            if (background != null) background.SetPropertyBlock(null);   // drop any hover tint so the flash shows
            if (!isActiveAndEnabled) { ApplyColor(c); return; }
            if (_flash != null) StopCoroutine(_flash);
            _flash = StartCoroutine(FlashRoutine(c));
        }

        IEnumerator FlashRoutine(Color c)
        {
            ApplyColor(c);
            yield return new WaitForSeconds(0.7f);
            _flash = null;
            ApplyColor(Armed ? Armed_ : Dim);
        }

        void ApplyColor(Color c)
        {
            if (background == null) background = GetComponent<Renderer>();
            if (background != null && background.sharedMaterial != null)
                background.sharedMaterial.color = c;
        }
    }
}

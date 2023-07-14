using System;
using System.Collections;
using System.Collections.Generic;
using ReliableSolutions.Unity.Common.Extensions;
using UnityEngine;

    [RequireComponent(typeof(GuidedSnapEnabledElement))]
    [DisallowMultipleComponent]
    public class ChangeParentOnAttachmentStateChange: MonoBehaviour
    {
        [SerializeField] private GuidedSnapEnabledElement _guidedSnapEnabledElement;
        public Vector3 LocalScaleBeforeUnparenting { get; private set; }

        void OnEnable()
        {
            _guidedSnapEnabledElement.Snapped += GuidedSnapEnabledElementOnSnapped;
            _guidedSnapEnabledElement.Unsnapped += GuidedSnapEnabledElementOnUnsnapped;
        }

        void OnDisable()
        {
            _guidedSnapEnabledElement.Snapped -= GuidedSnapEnabledElementOnSnapped;
            _guidedSnapEnabledElement.Unsnapped -= GuidedSnapEnabledElementOnUnsnapped;
        }

        private void GuidedSnapEnabledElementOnUnsnapped(object sender, EventArgs e)
        {
            if (transform.parent != null)
            {
                LocalScaleBeforeUnparenting = transform.localScale;
                transform.SetParentTracked(null);
            }
        }

        private void GuidedSnapEnabledElementOnSnapped(object sender, SnappedEventArgs e)
        {
            //parent to target that's snapping to
            var guidedSnapTarget = e.GuidedSnapTarget;
            ParentToGuidedSnapTarget(guidedSnapTarget);
        }

        public void ParentToGuidedSnapTarget(GuidedSnapTarget guidedSnapTarget)
        {
            transform.SetParentTracked(guidedSnapTarget.transform);
            LocalScaleBeforeUnparenting = Vector3.zero;
        }

        void Reset()
        {
            _guidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
        }
    }

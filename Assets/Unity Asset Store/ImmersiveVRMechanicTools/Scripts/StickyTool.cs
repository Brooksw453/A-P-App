using System;
using System.Collections.Generic;
using System.Linq;
using Common.Runtime.Utilities;
using ReliableSolutions.Unity.Common.Debug;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

//TODO: document, experimental - this is used to remove some small parts that would be difficult to reach via hand. Eg o-rings on regulators. Use at your own risk, no official support
public class StickyTool : GuidedSnapEnabledElement
{
    [Header("Sticky Tool")]
    [SerializeField] private float _autoDetachInSpecificAreasRadius = 0.01f;

    [ShowIf(nameof(IsDebug))] [SerializeField] private GuidedSnapEnabledElement _currentlyAttachedGuidedSnapEnabledElement;
    
    public bool IsElementAlreadySticked => _currentlyAttachedGuidedSnapEnabledElement != null;

    private TransformHolder _transformHolder = new TransformHolder();

    protected override void Start()
    {
        base.Start();
        
        Snapped += DetachOtherTransformDriverIfHoldingElement;
    }

    private void DetachOtherTransformDriverIfHoldingElement(object sender, SnappedEventArgs args)
    {
        var guidedSnapEnabledElementToKeepInPlace = new List<GuidedSnapEnabledElement>()
            {
                args.GuidedSnapTarget.gameObject.GetComponent<GuidedSnapEnabledElement>()
            }
            .Concat(args.GuidedSnapTarget.gameObject.GetComponentsInParent<GuidedSnapEnabledElement>())
            .FirstOrDefault(e => e && e.TransformPositionDriver != TransformPositionDriver);

        if (guidedSnapEnabledElementToKeepInPlace)
        {
            StartCoroutine(_transformHolder.HoldTransformInPlace(guidedSnapEnabledElementToKeepInPlace.transform));
            Unsnapped += StopKeepingOtherElementInPlace;
        }
    }

    private void StopKeepingOtherElementInPlace(object sender, EventArgs e)
    {
        _transformHolder.Stop();
    }

    void Update()
    {
        if(_currentlyAttachedGuidedSnapEnabledElement != null)
        {
            _currentlyAttachedGuidedSnapEnabledElement.transform.position = SnapRaycastOrigin.transform.position;
            _currentlyAttachedGuidedSnapEnabledElement.transform.up = RaycastDirection;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_currentlyAttachedGuidedSnapEnabledElement != null)
        {
            var colliders = Physics.OverlapSphere(_currentlyAttachedGuidedSnapEnabledElement.transform.position, _autoDetachInSpecificAreasRadius);
            DetachElementIfInAutoDetachmentArea(colliders);
           
            if (IsDebug)
            {
                DebugDraw.Point(_currentlyAttachedGuidedSnapEnabledElement.transform.position, Color.yellow, _autoDetachInSpecificAreasRadius);
            }
        }
    }

    private void DetachElementIfInAutoDetachmentArea(Collider[] colliders)
    {
        foreach (var collider in colliders)
        {
            var stickedElementAutoDetachmentArea = collider.GetComponent<StickedElementAutoDetachmentArea>();
            if (stickedElementAutoDetachmentArea != null)
            {
                stickedElementAutoDetachmentArea.DetachElement(_currentlyAttachedGuidedSnapEnabledElement);
            }
        }
    }

    public override bool ShouldPreventDetach(float detachNotAllowedIfRotationProgressMoreThan)
    {
        return false;
    }

    public override bool IsConsideredTool { get; } = true;

    public void StickGuidedSnapElement(GuidedSnapEnabledElement guidedSnapEnabledElement)
    {
        _currentlyAttachedGuidedSnapEnabledElement = guidedSnapEnabledElement;
        _currentlyAttachedGuidedSnapEnabledElement.Unsnapped += HandleAttachedElementUnsnappedEvent;
        _currentlyAttachedGuidedSnapEnabledElement.ElementDriverRegistered += HandleStickedObjectGrabbed;

        _currentlyAttachedGuidedSnapEnabledElement.XRFrameworkTransformControl.PassControlBackToXrFramework();
        _currentlyAttachedGuidedSnapEnabledElement.SetCollisionsWith(_collider, false);
    }

    private void HandleStickedObjectGrabbed(object sender, EventArgs e)
    {
        _currentlyAttachedGuidedSnapEnabledElement = null;
    }

    private void HandleAttachedElementUnsnappedEvent(object sender, EventArgs e)
    {
        if(_currentlyAttachedGuidedSnapEnabledElement == null) return;

        _currentlyAttachedGuidedSnapEnabledElement.SetCollisionsWith(_collider, true);

        _currentlyAttachedGuidedSnapEnabledElement.Unsnapped -= HandleAttachedElementUnsnappedEvent;
        _currentlyAttachedGuidedSnapEnabledElement.ElementDriverRegistered -= HandleStickedObjectGrabbed;
        _currentlyAttachedGuidedSnapEnabledElement = null;
    }
}
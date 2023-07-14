using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InsertElement : GuidedSnapEnabledElement
{
    [Header("Insert Element Options")]
    [SerializeField] private bool _canAttachRotationProgressElements;

    [SerializeField] private bool _isElementDetachmentDisallowingChildElementsAttachmentDetachment = true;

    private List<GuidedSnapTarget> _guidedSnapTargetsForRotationProgressElements;

    public List<GuidedSnapTarget> GuidedSnapTargetsForRotationProgressElements =>
        _guidedSnapTargetsForRotationProgressElements;

    public bool IsElementDetachmentDisallowingChildElementsAttachmentDetachment =>
        _isElementDetachmentDisallowingChildElementsAttachmentDetachment;

    public void AllowRotationProgressElementsAttachment()
    {
        _canAttachRotationProgressElements = true;
        SetCollisionCheckingState();
    }

    public void DisallowRotationProgressElementsAttachment()
    {
        _canAttachRotationProgressElements = false;
        SetCollisionCheckingState();
    }

    private void SetCollisionCheckingState()
    {
        GuidedSnapTargetsForRotationProgressElements
            .ForEach(e => e.GuidedSnapTargetCollider.SetCollisionCheckingState(_canAttachRotationProgressElements));
    }

    protected override void Start()
    {
        base.Start();
        FindAndSetGuidedSnapTargetChildren();
        SetCollisionCheckingState();
    }

    public override bool ShouldPreventDetach(float detachNotAllowedIfRotationProgressMoreThan)
    {
        var isAnyRotationProgressElementInsertedBeyondDetachPoint = GuidedSnapTargetsForRotationProgressElements
            .Select(s => s.CurrentlyAttachedRotationProgressElement)
            .Where(e => e != null)
            .Any(e => e.RotationProgress > detachNotAllowedIfRotationProgressMoreThan);
        return isAnyRotationProgressElementInsertedBeyondDetachPoint;
    }

    public override bool IsConsideredTool { get; } = false;

    protected override void Reset()
    {
        base.Reset();

        FindAndSetGuidedSnapTargetChildren();
        SetCollisionCheckingState();
    }

    [ContextMenu("FindAndSetGuidedSnapTargetChildren")]
    private void FindAndSetGuidedSnapTargetChildren()
    {
        _guidedSnapTargetsForRotationProgressElements = GetComponentsInChildren<GuidedSnapTarget>().ToList();
    }
}

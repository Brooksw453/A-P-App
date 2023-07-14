using UnityEngine;

[RequireComponent(typeof(GuidedSnapEnabledElement))]
public class NotAttachedToGuidedSnapTargetCondition: InteractionCondition
{
    [SerializeField] private GuidedSnapEnabledElement _guidedSnapEnabledElement;

    public override bool IsSatisfied()
    {
        return _guidedSnapEnabledElement.CurrentlySnappedToTarget == null;
    }

    void Reset()
    {
        _guidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
    }
}
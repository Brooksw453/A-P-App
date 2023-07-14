using ReliableSolutions.Unity.Common.Extensions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StickedElementAutoDetachmentArea: MonoBehaviour
{
    [SerializeField] private Transform MoveDetachedElementTo;

    void Reset()
    {
        var collider = gameObject.GetOrAddComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    public void DetachElement(GuidedSnapEnabledElement guidedSnapEnabledElement)
    {
        guidedSnapEnabledElement.Unsnap();

        if (MoveDetachedElementTo != null)
        {
            guidedSnapEnabledElement.transform.SetPosition(MoveDetachedElementTo.position);
        }
    }
}

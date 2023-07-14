using ReliableSolutions.Unity.Common.Extensions;
using UnityEngine;

public class GuidedSnapTargetCollider : MonoBehaviour
{
    [SerializeField] private GuidedSnapTarget _guidedSnapTarget;
    public GuidedSnapTarget SnapTarget => _guidedSnapTarget;

    void Reset()
    {
        if (SnapTarget == null)
        {
            _guidedSnapTarget = GetComponentInParent<GuidedSnapTarget>();
        }
    }

    public static GuidedSnapTargetCollider Create(GameObject go, GuidedSnapTarget guidedSnapTarget)
    {
        var component = go.AddComponent<GuidedSnapTargetCollider>();
        component._guidedSnapTarget = guidedSnapTarget;

        return component;
    }

    public void SetCollisionCheckingState(bool isEnabled)
    {
        gameObject.SetActiveTracked(isEnabled);
    }
}

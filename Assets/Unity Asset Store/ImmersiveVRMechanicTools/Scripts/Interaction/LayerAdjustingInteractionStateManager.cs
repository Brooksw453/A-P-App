using Assets.Common.Runtime;
using ReliableSolutions.Unity.Common.Extensions;
using UnityEngine;

public class LayerAdjustingInteractionStateManager : InteractionStateManager
{
    [Header("Lock Adjustments")]
    [SerializeField] /*[PhysicsLayer] TODO: readd layer without relying on MRTK*/ private int AdjustToWhenLocked = Layer.IgnoreRaycast;

    private int _layerMaskBeforeAdjustment = -1;

    protected override void Update()
    {
        base.Update();

        if (IsInteractionLocked)
        {
            if (gameObject.layer != AdjustToWhenLocked)
            {
                //something end changed layer of the object, need to remember that so it can be returned to proper value
                TrySetLayer(AdjustToWhenLocked);
            }
        }
    }

    protected override void OnInteractionLocked()
    {
        base.OnInteractionLocked();

        TrySetLayer(AdjustToWhenLocked);
    }

    protected override void OnInteractionUnlocked()
    {
        base.OnInteractionUnlocked();

        TrySetLayer(_layerMaskBeforeAdjustment);
    }

    private void TrySetLayer(int layer)
    {
        if (_layerMaskBeforeAdjustment == -1)
        {
            _layerMaskBeforeAdjustment = gameObject.layer;
        }

        if(layer == -1)
            return;

        gameObject.SetLayer(layer);
    }
}
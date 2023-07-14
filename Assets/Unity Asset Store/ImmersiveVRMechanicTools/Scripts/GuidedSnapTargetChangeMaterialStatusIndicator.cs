using System;
using System.Collections.Generic;
using UnityEngine;

public class GuidedSnapTargetChangeMaterialStatusIndicator : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private MeshRenderer MeshRenderer;
    
    [SerializeField] private Material UnableToAttachAngleNotWithinRangeMaterial;
    [SerializeField] private Material UnableToAttachOtherElementAlreadyAttachedMaterial;
    [SerializeField] private Material AttachedSuccesfullyMaterial;
    private Material _originalMaterial;
#pragma warning restore 649

    private static readonly Dictionary<TryAttachStatus, Func<GuidedSnapTargetChangeMaterialStatusIndicator, Material>> TryAttachStatusToGetMaterialMap = new Dictionary<TryAttachStatus, Func<GuidedSnapTargetChangeMaterialStatusIndicator, Material>>()
    {
        [TryAttachStatus.Attached] = t => t.AttachedSuccesfullyMaterial,
        [TryAttachStatus.UnableToAttachAngleNotWithinRange] = t => t.UnableToAttachAngleNotWithinRangeMaterial,
        [TryAttachStatus.UnableToAttachOtherElementAlreadyAttached] = t => t.UnableToAttachOtherElementAlreadyAttachedMaterial,
    };

    void Start()
    {
        if (MeshRenderer == null)
        {
            MeshRenderer = GetComponent<MeshRenderer>();
        }
    }

    public void RestoreOriginalMaterial()
    {
        MeshRenderer.material = _originalMaterial;
        _originalMaterial = null;
    }

    public void HandleTryAttach(TryAttachResult result)
    {
        if (_originalMaterial == null)
        {
            _originalMaterial = MeshRenderer.material;
        }

        if (TryAttachStatusToGetMaterialMap.TryGetValue(result.TryAttachStatus, out var getMaterialFn))
        {
            MeshRenderer.material = getMaterialFn(this);
        }
        else
        {
            MeshRenderer.material = UnableToAttachAngleNotWithinRangeMaterial;
        }
    }
}

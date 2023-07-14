using System;
using ReliableSolutions.Unity.Common.Extensions;
using ReliableSolutions.Unity.Common.Utilities;
using UnityEngine;

namespace ReliableSolutions.Unity.Common
{
    public class FollowObjectTarget : MonoBehaviour
    {
        [SerializeField] private Transform Source;
        public Func<Vector3> GetPositionOffset;

        [SerializeField] private bool FollowPosition = true;
    
        [SerializeField] private Boolean3 FollowRotation = new Boolean3(true);
        [SerializeField] private bool RelativeToInitialSourceRotation;
    
        private Quaternion _initialSourceRotation;
        private Quaternion _initialTargetRotation;

        public void SetSource(Transform sourceTransform, Func<Vector3> getPostionOffset = null, bool  relativeToInitialSourceRotation = false)
        {
            Source = sourceTransform;
            GetPositionOffset = getPostionOffset;

            if (sourceTransform != null)
            {
                _initialSourceRotation = sourceTransform.rotation;
                _initialTargetRotation = transform.rotation;
            }

            RelativeToInitialSourceRotation = relativeToInitialSourceRotation;
        }

        [ContextMenu("Update")]
        void Update()
        {
            if(!isActiveAndEnabled) return;

            if (Source != null)
            {
                if (FollowRotation.AnyTrue())
                {
                    Quaternion targetRotation;
                    if (RelativeToInitialSourceRotation)
                    {
                        var sourceRotationDiffFromInitial = _initialSourceRotation * Quaternion.Inverse(Source.rotation);
                        var newTargetRotation = Quaternion.Inverse(sourceRotationDiffFromInitial) * _initialTargetRotation;
                        targetRotation = newTargetRotation;
                    }
                    else
                    {
                        targetRotation = Source.rotation;
                    }

                    transform.SetRotation(Quaternion.Euler(
                        FollowRotation.x ? targetRotation.eulerAngles.x : 0f,
                        FollowRotation.y ? targetRotation.eulerAngles.y : 0f,
                        FollowRotation.z ? targetRotation.eulerAngles.z : 0f
                    ));
                }

                if (FollowPosition)
                {
                    transform.position = Source.transform.position - (GetPositionOffset?.Invoke() ?? Vector3.zero);
                }
            }
        }
    }
}

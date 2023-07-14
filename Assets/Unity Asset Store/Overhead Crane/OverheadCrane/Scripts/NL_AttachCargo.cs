using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif



public class NL_AttachCargo : MonoBehaviour
{
    public Rigidbody rbCurrent;
    private Rigidbody rbTarget;
    private ConfigurableJoint confJoint;
    private FixedJoint fixedJoint;
    private CapsuleCollider trigger;
    private Rigidbody connectPoint;

    public Collider[] excludeTriggers;
    private bool excludedTrigger;

    private bool inZone = false;

    public KeyCode attachKey = KeyCode.Space;

    public enum ConnectionType
    {
        FixedJoint,
        ConfigurableJoint
    }
    public ConnectionType connectionType = ConnectionType.ConfigurableJoint;

    [Tooltip("When using a Configurable Joint connection type this value will be used as an offset for the connection point.")]
    public float connectPointOffset = 0.365f;
    private float posY;
    [HideInInspector] public bool isAttached = false;

    void Start()
    {
        //rbCurrent = GetComponent<Rigidbody>();

        if (Application.isPlaying)
        {
            trigger = gameObject.AddComponent<CapsuleCollider>();
            trigger.radius = 0.08f;
            trigger.height = 0.28f;
            trigger.center = new Vector3(0, -0.35f, 0);
            trigger.isTrigger = true;

            connectPoint = new GameObject("Connect Point").AddComponent<Rigidbody>();
            connectPoint.isKinematic = true;
            connectPoint.useGravity = false;

            connectPoint.transform.parent = rbCurrent.transform;
            connectPoint.transform.localPosition = new Vector3(0, -connectPointOffset, 0);
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            if (Input.GetKeyDown(attachKey))
            {
                if (!isAttached)
                {
                    if (inZone && rbTarget != null)
                    {
                        isAttached = true;
                        if (connectionType == ConnectionType.ConfigurableJoint)
                        {
                            if (rbTarget.GetComponent<ConfigurableJoint>() == null)
                            {
                                confJoint = rbTarget.gameObject.AddComponent<ConfigurableJoint>();
                            }
                            else
                            {
                                confJoint = rbTarget.GetComponent<ConfigurableJoint>();
                            }

                            confJoint.xMotion = ConfigurableJointMotion.Locked;
                            confJoint.yMotion = ConfigurableJointMotion.Locked;
                            confJoint.zMotion = ConfigurableJointMotion.Locked;
                            confJoint.autoConfigureConnectedAnchor = false;
                            confJoint.connectedAnchor = Vector3.zero;
                            confJoint.angularXMotion = ConfigurableJointMotion.Limited;
                            confJoint.angularYMotion = ConfigurableJointMotion.Locked;
                            confJoint.angularZMotion = ConfigurableJointMotion.Limited;

                            SoftJointLimitSpring spring = new SoftJointLimitSpring();
                            spring.spring = 500;

                            SoftJointLimit limitZ = new SoftJointLimit();
                            limitZ.limit = 3;

                            SoftJointLimit limitHighX = new SoftJointLimit();
                            limitHighX.limit = 10;

                            SoftJointLimit limitLowX = new SoftJointLimit();
                            limitLowX.limit = 1;

                            confJoint.angularZLimit = limitZ;
                            confJoint.highAngularXLimit = limitHighX;
                            confJoint.lowAngularXLimit = limitLowX;

                            confJoint.angularXLimitSpring = spring;
                            confJoint.angularYZLimitSpring = spring;

                            confJoint.connectedBody = connectPoint;
                        }
                        else
                        {
                            if (rbTarget.GetComponent<FixedJoint>() == null)
                            {
                                fixedJoint = rbTarget.gameObject.AddComponent<FixedJoint>();
                            }
                            else
                            {
                                fixedJoint = rbTarget.GetComponent<FixedJoint>();
                            }

                            fixedJoint.connectedBody = connectPoint;
                        }
                    }
                }
                else
                {
                    if (connectionType == ConnectionType.ConfigurableJoint)
                    {
                        Destroy(confJoint);
                    }
                    else
                    {
                        Destroy(fixedJoint);
                    }

                    isAttached = false;
                }
            }
        }

        if (Application.isEditor)
        {
            if (rbCurrent.transform.position.y >= 0)
            {
                posY = rbCurrent.transform.position.y - connectPointOffset;
            }
            else
            {
                posY = rbCurrent.transform.position.y + connectPointOffset;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttached)
            return;

        excludedTrigger = false;

        for (int i = 0; i < excludeTriggers.Length; i++)
        {
            if(other == excludeTriggers[i])
            {
                excludedTrigger = true;
                break;
            }
        }

        if (excludedTrigger)
            return;
        
        inZone = true;

        if (other.GetComponent<Rigidbody>())
        {
            rbTarget = other.GetComponent<Rigidbody>();
        }

        //print(other.name + " object inside the hook trigger. Ready to attach.");
    }

    private void OnTriggerExit(Collider other)
    {

        excludedTrigger = false;

        for (int i = 0; i < excludeTriggers.Length; i++)
        {
            if (other == excludeTriggers[i])
            {
                excludedTrigger = true;
                break;
            }
        }

        if (excludedTrigger)
            return;

        inZone = false;

        if (other.GetComponent<Rigidbody>() == rbTarget)
        {
            rbTarget = null;
        }
        //print(other.name + " left the hook trigger.");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        if (rbCurrent != null) {
            Gizmos.DrawWireSphere(new Vector3(rbCurrent.transform.position.x, posY, rbCurrent.transform.position.z), 0.05f);
        }
    }
#endif
}

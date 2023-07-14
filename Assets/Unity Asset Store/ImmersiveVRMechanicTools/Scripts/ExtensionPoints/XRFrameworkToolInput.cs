using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class XRFrameworkToolInput : MonoBehaviour
{
    [Serializable] public class UnityEvent: UnityEvent<GameObject> { }

    [SerializeField] [Range(0, 3)] private float AllowDirectionChangeEveryNSeconds = 0.5f;

    public UnityEvent IncreaseToolForce = new UnityEvent();
    public UnityEvent DecreaseToolForce = new UnityEvent();
    public UnityEvent ChangeToolDirection = new UnityEvent();

    private float _lastChangeToolDirection;
    
    void Update()
    {
        if (IsIncreasingToolForce())
        {
            IncreaseToolForce?.Invoke(gameObject);
        }

        if (IsDecreasingToolForce())
        {
            DecreaseToolForce?.Invoke(gameObject);
        }

        if (IsChangingToolDirection())
        {
            if (Time.fixedTime - _lastChangeToolDirection > AllowDirectionChangeEveryNSeconds)
            {
                ChangeToolDirection?.Invoke(gameObject);
                _lastChangeToolDirection = Time.fixedTime;
            }
        }
    }

    protected abstract bool IsIncreasingToolForce();
    protected abstract bool IsDecreasingToolForce();
    protected abstract bool IsChangingToolDirection();
}

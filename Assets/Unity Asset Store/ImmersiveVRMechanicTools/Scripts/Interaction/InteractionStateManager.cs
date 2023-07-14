using System.Collections.Generic;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class InteractionStateManager : MonoBehaviour
{
    [Header("Conditions")] 
    [SerializeField] private List<ConditionSet> AndConditionSets = new List<ConditionSet>();

    [Header("Events")]
    public UnityEvent InteractionUnlocked = new UnityEvent();
    public UnityEvent InteractionLocked = new UnityEvent();

    [Header("Debug")]
    [SerializeField] public bool IsDebug;

    [SerializeField] [ShowIf(nameof(IsDebug))] private bool _isInteractionLocked;
    [SerializeField] [ShowIf(nameof(IsDebug))] private InteractionCondition LastNotSatifiedCondition;
    [SerializeField] [ShowIf(nameof(IsDebug))] private bool _skipConditionCheck;

    private bool _previousIsInteractionLocked;
    public bool IsInteractionLocked => _isInteractionLocked;

    protected virtual void Start()
    {
        _previousIsInteractionLocked = _isInteractionLocked;
        _isInteractionLocked = !AreConditionsSatisfied();
        TriggerInteractionStateHandlers();
    }

    protected virtual void Update()
    {
        _previousIsInteractionLocked = _isInteractionLocked;
        _isInteractionLocked = !AreConditionsSatisfied();
        if (_isInteractionLocked != _previousIsInteractionLocked)
        {
            TriggerInteractionStateHandlers();
        }
    }

    private void TriggerInteractionStateHandlers()
    {
        if (_isInteractionLocked) OnInteractionLocked();
        else OnInteractionUnlocked();

        if(IsDebug) Debug.Log($"Interaction {(_isInteractionLocked ? "LOCKED" : "UNLOCKED")} - {name}", gameObject);
    }

    protected virtual void OnInteractionLocked()
    {
        InteractionLocked?.Invoke();
    }

    protected virtual void OnInteractionUnlocked()
    {
        InteractionUnlocked?.Invoke();
    }

    [ContextMenu(nameof(AreConditionsSatisfied))]
    private bool AreConditionsSatisfied()
    {
        if (_skipConditionCheck) return true;

        LastNotSatifiedCondition = null;

        foreach (var andConditionSet in AndConditionSets)
        {
            var isAnyOrConditionSatisfied = false;
            foreach (var orCondition in andConditionSet.OrConditions)
            {
                if (orCondition.IsSatisfied())
                {
                    isAnyOrConditionSatisfied = true;
                }
                else
                {
                    LastNotSatifiedCondition = orCondition;
                }
            }

            if (!isAnyOrConditionSatisfied) return false;
        }

        return true;
    }
}

[System.Serializable]
public class ConditionSet
{
    [FormerlySerializedAs("Conditions")] public List<InteractionCondition> OrConditions;
}

public abstract class InteractionCondition: MonoBehaviour
{
    public abstract bool IsSatisfied();
}
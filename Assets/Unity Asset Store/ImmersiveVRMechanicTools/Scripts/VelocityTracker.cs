using System.Collections.Generic;
using System.Linq;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public class VelocityTracker : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private bool IsTracking = true;
    [SerializeField] private bool IsDebug;
    [SerializeField] private int PositionSampleCount = 20;
    
    [ShowIf(nameof(IsDebug))] [ReadOnly] [SerializeField] private Vector3 LastCalculatedAcceleration;
#pragma warning restore 649

    private int _previoustPositionSampleCount;
    private FixedSizedQueue<Vector3> _positionSamples = new FixedSizedQueue<Vector3>(5);

    public List<Vector3> GetSamples(int count)
    {
        return _positionSamples.Take(count).ToList();
    }

    void Start()
    {
        _positionSamples = new FixedSizedQueue<Vector3>(PositionSampleCount);
    }

    void Update()
    {
        if (_previoustPositionSampleCount != PositionSampleCount)
        {
            _positionSamples = new FixedSizedQueue<Vector3>(PositionSampleCount);
            _previoustPositionSampleCount = PositionSampleCount;
        }

        if (IsTracking)
        {
            _positionSamples.Enqueue(transform.position);
        }

        if (IsDebug)
        {
            GetAcceleration();
        }
    }

    public Vector3 GetAcceleration()
    {
        var average = Vector3.zero;
        var samples = _positionSamples.ToList();
        for (int sampleIndex = 2; sampleIndex < samples.Count; sampleIndex++)
        {
            if (sampleIndex >= 2)
            {
                int first = sampleIndex - 2;
                int second = sampleIndex - 1;

                Vector3 v1 = samples[first % samples.Count];
                Vector3 v2 = samples[second % samples.Count];
                average += v2 - v1;
            }
        }
        average *= 1.0f / Time.deltaTime;
        LastCalculatedAcceleration = average;

        return average;
    }
}

public class FixedSizedQueue<T> : Queue<T>
{
    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        while (base.Count > Size)
        {
            base.Dequeue();
        }
    }
}
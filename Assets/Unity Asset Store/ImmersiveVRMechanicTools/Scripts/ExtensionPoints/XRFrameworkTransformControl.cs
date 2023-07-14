using System;
using UnityEngine;

public abstract class XRFrameworkTransformControl : MonoBehaviour
{
    public abstract void TakeControlFromXrFramework();

    public abstract void PassControlBackToXrFramework();
}

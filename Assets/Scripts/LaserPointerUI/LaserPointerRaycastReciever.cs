using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class LaserPointerRaycastReceiver : MonoBehaviour
{
    public abstract void OnUpdate(RaycastResult raycast);
}

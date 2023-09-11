using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isActive; //verifying if an action is active
    protected Action onActionComplete; 
    protected virtual void Awake() // since its virtual inheritors can overide making it easier for more custom actions
    {
        unit = GetComponent<Unit>();
    }
}

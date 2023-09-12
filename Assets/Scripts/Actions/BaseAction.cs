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

    public abstract string GetActionName(); // forced to implement this in all actions

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    // using our list of grid positions, check to see if its valid to move here
    //SideNote this kind of idea can be modified for AI combat in PFF
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

}

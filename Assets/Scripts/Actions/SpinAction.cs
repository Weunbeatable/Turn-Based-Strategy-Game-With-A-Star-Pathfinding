using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;
    private void Update()
    {
        if (!isActive) { return; }

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;
        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }

    }

    public override void TakeAction(GridPosition gridPosition, Action OnSpinComplete)
    {
        totalSpinAmount = 0f;
        ActionStart(OnSpinComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {

        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0, // will do the action if it can't do anythingelse, very little value
        };
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab; // reference for nade

    private int maxThrowDistance = 7;
    private void Update()
    {
        if(!isActive) { return; }

    }
    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridposition = unit.GetGridPosition();

        //Loop through grid and check for points on the grid within 4 units of current unit
        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridposition + offsetGridPostion;

                // If not valid we want to discard that position, if it is we want to keep it.
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; // skip to the next iteration of the loop if not valid
                }

                int testDistance = (int)(MathF.Abs(x) + MathF.Abs(z)); // our offsets
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform =  Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity); // spawn point 
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
      
        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete() // callback
    {
        ActionComplete(); 
    }

}

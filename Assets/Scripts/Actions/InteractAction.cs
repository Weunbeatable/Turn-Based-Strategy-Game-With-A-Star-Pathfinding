using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int maxInteractDistance = 1;


    private void Update()
    {
        if (!isActive) { return; }
    }
    public override string GetActionName()
    {
        return "interact";
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
        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridposition + offsetGridPostion;

                // If not valid we want to discard that position, if it is we want to keep it.
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; // skip to the next iteration of the loop if not valid
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

                if(interactable == null)
                {
                    // no door on this grid position.
                    continue;
                }
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);
        ActionStart(onActionComplete);
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{
    //Since actions belong to the unit design wise the action should ask the Unit for a grid position
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    [SerializeField] private int maxMoveDistance = 4; // for ease of testing this field will be serialized
    
    private Vector3 targetPosition;
    
    protected override void Awake()
    {
        base.Awake(); // we'll run the base awake action first before calling target pos;
        targetPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        Vector3 moveDirection = (targetPosition - transform.position).normalized; // only care about direction so we normalize
        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            
            float moveSpeed = 4;
            transform.position += moveDirection * moveSpeed * Time.deltaTime; // framerate independent movement
                                                                             // using transform. forward/up/right * a vector allows for easy rotation
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime); // we'll lerp rotation to make it smoother
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridposition = unit.GetGridPosition();

        //Loop through grid and check for points on the grid within 4 units of current unit
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridposition + offsetGridPostion;

                // If not valid we want to discard that position, if it is we want to keep it.
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                  continue; // skip to the next iteration of the loop if not valid
                }

                if(unitGridposition == testGridPosition)
                {
                    continue; // same grid position where the unit is already at
                }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue; // Grid Position already occupied with another unit
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
    public override int GetActionPointsCost()
    {
        return 1;
    }

    // works a little differently from other actions as it has to do with positioning and player proximity, 
    // Enemy should move to player, find if ther are shootable targets fromc current positions and then depending on number of targets that will define the actual action value; 
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
     int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition); // grabbing target count at this current position
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 20, // Enemy will prioritize a place with lots of enemies
        };
    }
}

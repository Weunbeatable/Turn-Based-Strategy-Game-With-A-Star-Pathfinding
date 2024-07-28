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
    
    private List <Vector3> positionList;
    // keep track of each following position 
    private int currentPositionIndex;
    
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized; // only care about direction so we normalize
        float stoppingDistance = .1f;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime); // we'll lerp rotation to make it smoother

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            
            float moveSpeed = 4;
            transform.position += moveDirection * moveSpeed * Time.deltaTime; // framerate independent movement
                                                                             // using transform. forward/up/right * a vector allows for easy rotation
        }
        else
        {
            currentPositionIndex++;
            // if gone past last index
            if(currentPositionIndex >= positionList.Count)
            {
                // reached end of list so we should stop moving. 
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
           
        }

       
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>(); // fill in position list.

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

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
                GridPosition offsetGridPostion = new GridPosition(x, z, 0);
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

                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue; // checking if spot is a walkable position. 
                }

                if (!Pathfinding.Instance.HasPath(unitGridposition, testGridPosition))
                {
                    continue; // checking for unreachable areas.
                }

                int pathFindingDistanceMultiplier = 10;
                // path length costs were multiplied by 10 to work with ints instead of floats so the max distance check
                // is also multiplied by 10 so it still works as it should. 
                if (Pathfinding.Instance.GetPathLength(unitGridposition, testGridPosition) > maxMoveDistance * pathFindingDistanceMultiplier)
                {
                    continue; // path length is too long. 
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

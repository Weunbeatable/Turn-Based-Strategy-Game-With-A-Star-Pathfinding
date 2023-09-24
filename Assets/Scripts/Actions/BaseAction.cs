using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

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

    public virtual int GetActionPointsCost() => 1;

    protected void ActionStart(Action onActionComplete) // global way of taking actions and checkign to see if an action has started or completed
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit() => unit;

    // we want to get the best enemy AI action
    public EnemyAIAction GetBestEnemyAIAction()
    {
        // create a list of enemyAIactions
        List<EnemyAIAction> enemyAIActionsList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList) //want to generate the enemy AI action for this specific actio non this grid position
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionsList.Add(enemyAIAction);
        }
        // Actually using the sort function I teach my students for once (this time using one of its overloads)
        // We'll use the delegate sort function and call some anonymous functions and return b.action - a.action which will sort our action values 
        // Then we'll return the best one.
        // This should only run if there are actual possible actions to do (list must be populated)

        if(enemyAIActionsList.Count > 0)
        {
            enemyAIActionsList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue); // sort actions
            return enemyAIActionsList[0]; // return best action
        }
        else
        {
            // No possible Enemy AI Actions
            return null;
        }
        
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition); // abstract because I want every action to implemen this function
    
}

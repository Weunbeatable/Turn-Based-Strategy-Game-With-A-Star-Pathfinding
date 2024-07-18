using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler onAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }
    private int maxSwordDistance = 1;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    [SerializeField]
    float beforeHitStateTime = 0.7f;
    [SerializeField]
    float afterHitStateTime = 0.5f; // making this serialized gives me more flexibililty

    private void Update()
    {
        if (!isActive) { return; }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 targetPosition = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Slerp(transform.forward, targetPosition, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
                break;

        }
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }
    private void NextState()
    {
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                onAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }

    }
    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridposition = unit.GetGridPosition();

        //Loop through grid and check for points on the grid within 4 units of current unit
        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridposition + offsetGridPostion;

                // If not valid we want to discard that position, if it is we want to keep it.
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; // skip to the next iteration of the loop if not valid
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty no unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // if on same team ignore
                    continue;
                }


                // for sword want all neighbours since its a close range melee so only validating grid positions is needed

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.SwingingSwordBeforeHit;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance() => maxSwordDistance;
}

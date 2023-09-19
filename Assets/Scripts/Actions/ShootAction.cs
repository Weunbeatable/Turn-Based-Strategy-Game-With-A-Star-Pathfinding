using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }
    private enum State
    {
        Aiming, 
        Shooting,
        CoolOff,
    }

    private State state;
    [SerializeField] private int maxShootDistance = 7;
    [SerializeField] float newShootingStateTime = 0.1f;
    [SerializeField] float newCoolOffStateTime = 0.1f;
    [SerializeField] float newAimingStateTime = 1f;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    private void Update()
    {
        if (!isActive) { return; }

        stateTimer -= Time.deltaTime;


        switch (state)
        {
            case State.Aiming:
                Vector3 targetPosition = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Slerp(transform.forward, targetPosition, Time.deltaTime * rotateSpeed); // we'll lerp rotation to make it smoother
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.CoolOff:
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
            case State.Aiming:
                
                    state = State.Shooting;
                   float shootingStateTime = newShootingStateTime;
                    stateTimer = shootingStateTime;                
                break;
            case State.Shooting:
                    state = State.CoolOff;
                   float coolOffStateTime = newCoolOffStateTime;
                    stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
                ActionComplete();
                break;
        }

    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        targetUnit.Damage(40);
    }
    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridposition = unit.GetGridPosition();

        //Loop through grid and check for points on the grid within 4 units of current unit
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridposition + offsetGridPostion;

                // If not valid we want to discard that position, if it is we want to keep it.
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; // skip to the next iteration of the loop if not valid
                }

                int testDistance = (int)(MathF.Abs(x) + MathF.Abs(z)); // our offsets
                if(testDistance > maxShootDistance)
                {
                    continue;
                }
            
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty no unit
                    continue; 
                }

                Unit targetUnit =  LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy()) 
                {
                    // if on same team ignore
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnShootComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float aimingStateTime = newAimingStateTime;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(OnShootComplete);
    }

    public Unit GetTargetUnit() => targetUnit;
}

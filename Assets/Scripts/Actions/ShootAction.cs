using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot; // this way we check on any instance of shooting. 
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

    [SerializeField] private LayerMask obstaclesLayerMask; 

    private State state;
    [SerializeField] private int maxShootDistance = 7;
    [SerializeField] float newShootingStateTime = 0.1f;
    [SerializeField] float newCoolOffStateTime = 0.1f;
    [SerializeField] float newAimingStateTime = 1f;
    [SerializeField] private int damageToDeal = 40; 
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
                targetUnit.Damage(40);
                ActionComplete();
                break;
        }

    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        // Moved targetUnit.Damage(40); from here to cooloff period this was to avoid weird visual of damage being done before animation, will adjust cooloff and shooting timers so 
        // each animation attack lines up better and feels nicer
    }
    public override string GetActionName()
    {
        return "Shoot";
    }
    public override List<GridPosition> GetValidActionGridPositionList() 
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition) // this version will take any valid grid pos so we can see valid shootable targets
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridposition = unit.GetGridPosition();

        //Loop through grid and check for points on the grid within 4 units of current unit
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPostion = new GridPosition(x, z, 0);
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

                // raycast from unit position to target pos to validate if you can shoot (avoid shooting through walls)
                // also leaving it flexible to shoot over crates and low walls. so there should be a height check and it should use 
                // vector3.up 
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridposition); // using unit world position since code was refactored to use unit position.
                Vector3 shootDir = targetUnit.GetWorldPosition() - unitWorldPosition.normalized;
                float unitShoulderHeight = 1.7f; // got that value using the units themselves. 
              if (Physics.Raycast
                    (unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask)) // if this raycast does hit something then its not a valid place to shoot. 
                {
                    // blocked by an obstacle
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

    public int GetMaxShootDistance() => maxShootDistance;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // make sure theAI shoots the weakest player unit first
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            // will do the action if it can't do anythingelse, very little value
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f), // use 1- so that we look for enemy with lowest not highest health 
            // value for enemy with full health is 100 (1 -1) * 100 at 50 (.5 -.5) .5 *100 =  50, and so on
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
       return GetValidActionGridPositionList(gridPosition).Count; // list of valid targets
    }
}

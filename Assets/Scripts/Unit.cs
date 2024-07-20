using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    [SerializeField] private Animator unitAnimator;

    private const int ACTION_POINTS_MAX = 5;

    public static event EventHandler OnAnyActionPointsChanged; // this will fire whenever action points change
    // this will be for a specific event that we are certain will change after changing action points. 
    public static event EventHandler OnAnyUnitSpawned;

    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private HealthSystem healthSystem;
    private GridPosition gridPosition;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>(); //store components attached to this unit that extends base action
    }

    private void Start()
    {
        // When unit starts it finds its own pos then calls setunit on level grid,
        // setunitatGridPos func from level grid gets grid object on underlying grid system and calls a function to set unit
        // Grid object stores unit and displays it on tostring
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.onDead += healthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition) // if a unit has moved update the grid to show that
        {
            // Unit changed Grid position
            GridPosition oldGridposition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridposition, newGridPosition);
        }
    }

    public void ReadyAnim()
    {
        unitAnimator.Play("Intro");
    }
   
    public float GetNormalizedTime(Animator animator, string tag) // purpose here is to check how far through the animation and if we pass a threshold so if they player is still attacking or holding attack , go to the next animation.
    {
        // want to know the data for current and next state to figure out which one we are in whenever blending
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0); // gettinig animation layer info and storing in variables
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag)) // already know we are in layer 0, so if we are in layer 0 and transitioning to an attack we want to get the data for the next state.

        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag)) // if not in an attack animation case
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f; // just in case none of this is true;s
        }
    }

    public Animator GetAnimator() => unitAnimator;

    // Using Generics we'll grab the action types so we don't have to create a new one each time in unit
    // This same idea can be used in an ability system. 
    public T GetAction<T>() where T : BaseAction // only valid types that extend base action0
    {
        foreach (BaseAction baseAction in baseActionArray) // Cycle through all actions attached to this unit
        {
            if(baseAction is T) // if this action is of type T
            {
                return (T)baseAction; // return the base action
            }
        }
        return null; // or dont
    }
    public GridPosition GetGridPosition() => gridPosition;

    public Vector3 GetWorldPosition() => transform.position;
    public BaseAction[] GetBaseActionArray() => baseActionArray; 

    public bool TrySpendActionPointsToTakeACertainAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints() => actionPoints;

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e )
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn())||   
            (!IsEnemy() & TurnSystem.Instance.IsPlayerTurn())) // if it is the enemy and not the players turn
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
       
    }

    public bool IsEnemy() => isEnemy;

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() => healthSystem.GetHealthNormalized();
}

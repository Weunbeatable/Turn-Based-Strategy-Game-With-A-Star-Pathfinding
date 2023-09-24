using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    // enemy state machine
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn; 
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }


    void Update()
    {

        if (TurnSystem.Instance.IsPlayerTurn()) // if player turn do nothing
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Busy;
                    if(TryTakeEnemyAIAction(SetStateTakingTurn)) // We'll paste it here so actions aren't instant, make it feel like enemy is considering options 
                    {
                        state = State.WaitingForEnemyTurn;
                    }
                    else
                    {
                        // no more enemies have actions they can take so we end the enemy turn
                        TurnSystem.Instance.NextTurn();
                    }
                    
                    
                }
                break;
            case State.Busy:
                break;
        }

       

    }

    // give the enemy some pause before taking an action to make it feel like its thinking
    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        // check if enemy's turn
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
        }
        timer = 2f;
    }

    // we want to keep taking actions until we've exhausted options for that turn, we'll also need to keep a track of AI enemies while this is going on. before
    // we can take an ENEMYAIACTION
    private bool TryTakeEnemyAIAction(Action OnEnemyAIActionComplete)
    {
 
        foreach (Unit enemyAI in UnitManager.Instance.GetEnemyUnitList())
        {
            // will attempt to do some action
            if(TryTakeEnemyAIAction(enemyAI, OnEnemyAIActionComplete))
            {
                return true;
            }
            
        }
        // If an enemy is unable to take an action then just return false
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        // Need to always keep track of best action to take
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

       foreach(BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
           if(!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) // first check to see if we have enough points to take an action
            {
                // Too Broke to do tis action
                continue;
            }
           if(bestEnemyAIAction == null) // if currently no best action assign one for the enemy to take
            {
                // first action enemy can take
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                //we already had a best enemy AI action
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                // assuming the test action isn't empty, we want to compare it with the currently stored best action value
                // depending on the results we get back we'll have the enemyAI choose to do that action. 
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
            
        }
       // If we have an action and can spend action points, we'll take that action
       if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeACertainAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }

}

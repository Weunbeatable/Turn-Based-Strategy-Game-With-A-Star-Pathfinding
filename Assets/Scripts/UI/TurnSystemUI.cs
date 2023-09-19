using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurn;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private void Start()
    {
        endTurn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {

        turnNumberText.text = "TURN : " + TurnSystem.Instance.GetTurnNumber();       
    }

    // relies on turn system, if its not enemy's turn when we grab the turn system instance, then set the enemy turn UI to being active. 
    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn()); 
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}

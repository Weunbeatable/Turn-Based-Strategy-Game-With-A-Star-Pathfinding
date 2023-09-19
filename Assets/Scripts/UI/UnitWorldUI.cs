using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.onDamaged += HealthSSystem_OnDamaged;

        UpdateACtionPointsText();
        UpdateHealthBar();
    }



    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateACtionPointsText();
    }

    private void UpdateACtionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }
    private void HealthSSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
}

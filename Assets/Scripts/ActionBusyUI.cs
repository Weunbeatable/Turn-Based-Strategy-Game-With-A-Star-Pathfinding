using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;

        HideBusyObject();
    }

    private void ShowBusyObject()
    {
        gameObject.SetActive(true);
    }

    public void HideBusyObject()
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            ShowBusyObject();
        }
        else
        {
            HideBusyObject();
        }
    }
}

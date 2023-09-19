using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted; // we listen to any action
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

     
    }

    private void ShowACtionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender) // using a switch case we can listen for the object sender and this way we can have the action camera triggered specifically for the action we want.
        {
            case ShootAction shootAction:
                //Want the camera to be roughly set shoulder height
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.5f;

                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;
                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() +
                    cameraCharacterHeight +
                    shoulderOffset +
                    (shootDir * -1); // move camera to shooters position, get right height, shoulder offset and direction * -1 so we can see shooter and a bit of target as well

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                ShowACtionCamera();

                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender) // using a switch case we can listen for the object sender and this way we can have the action camera triggered specifically for the action we want.
        {
            
            case ShootAction shootAction:
                HideActionCamera();

                break;
        }
    }
}

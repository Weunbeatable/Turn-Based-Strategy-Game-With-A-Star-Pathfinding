using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } 

    private void Awake()
    {
        // error check for more than one instance, hence singleton, we'll delete the extra
        if (Instance != null)
        {
            Debug.Log("There's more than one UnitActionSystem! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition; 
    }
    public bool IsMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public Vector2 GetCameraMoveVector()
    {
        Vector3 inputMoveDir = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }
        return inputMoveDir;
    }

    public float GetCameraRotateAmount()
    {
        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
    }

    public float GetCameraZoomAmount()
    {
        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = 1f;
        }

        return zoomAmount;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    // we'll use singletons for the vsual effect
    public static UnitActionSystem Instance { get; private set; } // we only need 1 field for the class so its static

    public event EventHandler OnSelectedUnitChange;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    private int counter = 0;

    private bool isBusy; //check if we're in a state
    private void Awake()
    {
        // error check for more than one instance, hence singleton, we'll delete the extra
        if(Instance != null)
        {
            Debug.Log("There's more than one UnitActionSystem! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {

        if (isBusy)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) // call our unit moving function
        {
           
            if (TryHandleUnitSelection())
            {
                counter = 1; // counter added for playing ready anim
                if (counter == 1)
                {
                    selectedUnit.ReadyAnim();
                }
                return;
            }

            counter = 0;

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            // verify that you can actually move here using the grid position checks in level grid and grid system
            // If valid then call move action to move

            if (selectedUnit.GetMoveAction().IsValidActionGridPosition(mouseGridPosition))
            {
                SetBusy();
                selectedUnit.GetMoveAction().Move(mouseGridPosition, ClearBusy);
            }
            
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            SetBusy();
            selectedUnit.GetSpinAction().Spin(ClearBusy);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
    }

    private void ClearBusy()
    {
        isBusy = false;
    }
    //Summary handles selecting indivudal units, fires raycast and checks for unit under mouse
    private bool TryHandleUnitSelection()
    {
        
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           if( Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) // 1 << 6 way to change mask with bit shifting without using layermask
            {
                if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                SetSelectedUnit(unit);
                return true;
                }
            }
        counter = 0;
            return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        // check for subs
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    //summary exposing unit selection method above
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}

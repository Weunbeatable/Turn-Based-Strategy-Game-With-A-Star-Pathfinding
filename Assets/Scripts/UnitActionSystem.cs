using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    // we'll use singletons for the vsual effect
    public static UnitActionSystem Instance { get; private set; } // we only need 1 field for the class so its static

    public event EventHandler OnSelectedUnitChange;
    public event EventHandler OnSelectedActionChange;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    private int counter = 0;

    private BaseAction selectedAction;
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

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    private void Update()
    {

        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return; // If enemy's turn don't do anything
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
            if (TryHandleUnitSelection())
            {
                counter = 1; // counter added for playing ready anim
                if (counter == 1) { selectedUnit.ReadyAnim(); }        
                return;
            }
            counter = 0;
            HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
      if(InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            // verify that you can actually move here using the grid position checks in level grid and grid system
            // If valid then call move action to move
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }
            if (!selectedUnit.TrySpendActionPointsToTakeACertainAction(selectedAction))
            {
                return;
            }
              SetBusy();
              selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty); // event triggers when an action starts
        }
    }
    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }
    //Summary handles selecting indivudal units, fires raycast and checks for unit under mouse
    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) // 1 << 6 way to change mask with bit shifting without using layermask
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)
                    {
                        //unit is already selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        // Clicked on the enemy
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        counter = 0;
            return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());

        // check for subs
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
    }

    //summary exposing unit selection method above
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction() => selectedAction;

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) // in the event that player unit dies during enemy turn we dont want to select player unit that died
        // when our turn starts again
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            if (selectedUnit == null) // our player has died or for some reason isn't selected
            {
                if (UnitManager.Instance.GetFriendlyUnitList().Count > 0)
                {
                    SetSelectedUnit(UnitManager.Instance.GetFriendlyUnitList()[0]); // assign a new friendly unit 
                }
                else
                {
                    SetSelectedUnit(null);
                    //TurnSystem.Instance.NextTurn();
                }
            }
        }
    }
}

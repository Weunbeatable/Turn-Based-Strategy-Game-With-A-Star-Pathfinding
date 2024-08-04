using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 3f;

    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;
    private List<GridSystemHex<GridObject>> gridSystemList;

    // these int values will be setup when making connection with level grid so they only need to be defined once
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private int floorAmount;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There's more than one UnitActionSystem! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        gridSystemList = new List<GridSystemHex<GridObject>>();

        for (int floor = 0; floor < floorAmount; floor++)
        {
         GridSystemHex<GridObject> gridSystem = new GridSystemHex<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT,
                (GridSystemHex<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            //  gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

            gridSystemList.Add(gridSystem);
        }
    }

    private void Start()
    {
        // This should be called before we have anything else to do with pathfinding
         Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }
    // function to get a grid system for a given floor
    private GridSystemHex<GridObject> GetGridSystem(int floor)
    {
        return gridSystemList[floor];
    }
       
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        // grab correct grid system to then grab correct grid object. 
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);// return the position of unit ongrid
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }
        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        // clearing the unit is essentially setting it to null
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        // we'll have the grid system visual listen for this event. 
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    // some pass through functions
    // GetGridPosition doesn't take a grid position but a world position, so it needs a helper function.
    // that takes a world position and returns a given floor. 

    public int GetFloor(Vector3 worldPosition)
    {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        int floor = GetFloor(worldPosition);
        return GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    // reference To Grid Systems validation so it can be accessed properly by other scripts without weird class access and a;llowing for proper layers of abstraction
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        // used to assume there was always a grid system for the floor, that assumption wont work for 
        // multi levels so it should check if its valid
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount)
        {
            return false;
        }
        else
        {
          return  GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
        }
    }
    // The assumption is all floors have the same shape so the width & height can be gotten from item 0
    public int GetWidth() => GetGridSystem(0).GetWidth();

    public int GetHeight() => GetGridSystem(0).GetHeight();

    public int GetFloorAmount() => floorAmount;

    // test to see if a grid position already has a unit on it, if it does it shouldn't be considered a valid position
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    // return unit on that grid position.
    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void  SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }
}

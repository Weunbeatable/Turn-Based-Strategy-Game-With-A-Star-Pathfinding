using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    // our object (aka unit) on the grid
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;
   [SerializeField] private Unit unit { get; set; }

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = ""; // start off empty
        foreach (Unit unit in unitList)
        {
            // if multiple units occupying a spot they should display on a new line each one
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString; // log on position and unit
        // if unit it will say theres a unit otherwise nothing
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList() => unitList;

    // Check if there is a unit occupying the space
    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return unitList[0];
        }
        else { return null; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    // We'll use this struct to help s define the color of the grid visual depending on the situation, Like a vector 3 our custom struct allows us for a more modifiable type
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
        // enum is our list of possible grid colors 
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft, 
        Yellow
    };

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;

    private void Awake()
    {
        // error check for more than one instance, hence singleton, we'll delete the extra
        if (Instance != null)
        {
            Debug.Log("There's more than one gridSystemVisual! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        // visual single on every grid position
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
        LevelGrid.Instance.GetWidth(),
        LevelGrid.Instance.GetHeight(),
        LevelGrid.Instance.GetFloorAmount()
        ]; ;
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                {


                    GridPosition gridPosition = new GridPosition(x, z, floor); //TODO modify to show visuals for all floors

                    Transform gridSystemVisualSingleTransofm =

                    Instantiate(gridSystemVisualSinglePrefab,
                    LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                    gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingleTransofm.GetComponent<GridSystemVisualSingle>();
                }
            }
        }
        //instead of the grid visual running every frame we want it running on selection
        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead; // in the event that a unit dies we should update the visual so its not showing us incorrect information on who we can target.

        UpdateGridVisual();
    }
  

 
    public void HideAllGridPosition()
    {
      
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                {
                    gridSystemVisualSingleArray[x, z, floor].Hide();
                }
            }
        }
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionsList = new List<GridPosition>();

        for (int x = -range; x <= range; x++) // get grid position values in x range
        {
            for (int z = -range; z <= range; z++) // get grid position values in z range
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, gridPosition.floor); // pass those into grid position list adding current grid position and all retrieved positions. 

                // if the position is not valid
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

  
                gridPositionsList.Add(testGridPosition);
            }

        }
        ShowGridPositionList(gridPositionsList, gridVisualType); // After retreiving all these positions return the data and show it to the user
    }

    //helper function to show areas of influence when using a targeting skill
    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionsList = new List<GridPosition>();

        for (int x = -range; x <= range; x++) // get grid position values in x range
        {
            for (int z = -range; z <= range; z++) // get grid position values in z range
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, gridPosition.floor); // pass those into grid position list adding current grid position and all retrieved positions. 

                // if the position is not valid
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // checking to see if in circular range or not
                int testDistance = (int) MathF.Abs(x) + (int)MathF.Abs(z); // we only care about positive grid position values, values outside grid should be ignored  so Abs
                if(testDistance > range) // if above the range continue and ignore it
                {
                    continue;
                }
                //if not above range add the grid position
                gridPositionsList.Add(testGridPosition);
            }
            
        }
        ShowGridPositionList(gridPositionsList, gridVisualType); // After retreiving all these positions return the data and show it to the user
    }
    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition grid in gridPositionList)
        {
            gridSystemVisualSingleArray[grid.x, grid.z, grid.floor].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }
    private void UpdateGridVisual() // will update when things happen
    {

        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        // show appropriate grid material depending on action
        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;


                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
        }
        ShowGridPositionList(
        selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError("Could not find GridVisualtypeMaterial for Grid visual type " + gridVisualType);
        return null;
    }
    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
}

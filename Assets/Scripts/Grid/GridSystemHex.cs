using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemHex<TGridObject> 
{
    private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
    private int width; 
    private int height;
    private float cellSize;
    private int floor;
    private float floorHeight;
    private TGridObject[,] gridObjectArray;
    
                                                            // this helps us bypass C# limit on contstraints by passing a delegate which will create our object
    public GridSystemHex(int width, int height, float cellSize, int floor, float floorHeight, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject) 
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;

        gridObjectArray = new TGridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, floor);
                gridObjectArray[x,z] = createGridObject(this, gridPosition); // get around constraints by passing a delegate that will take our object. 
                // can't create object of type because of no New constraint is saying we can't make the type cause the'res no gaurantee the type we make will have to use the new constraint
                // However C# won't allow you to use a paramterless constructor but we can use delegates to recieve a delegate that creates the object instead. Like a func which is a delegate that returns something 
            }
        }
       
    }
    // convert grid position to world pos
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return
            new Vector3(gridPosition.x, 0, 0) * cellSize +
            new Vector3(0, 0, gridPosition.z) * cellSize  * HEX_VERTICAL_OFFSET_MULTIPLIER + 
           (((gridPosition.z % 2) ==1 ) ? new Vector3(1,0,0) * cellSize * 0.5f : Vector3.zero) + 
           new Vector3(0, gridPosition.floor, 0) * floorHeight;
        // terinary operator to allow hexagon grids to line up, if grid postion is odd an offset is added
        // other wise none will be added aka Vector3.zero 
  
    }

    //convert World Pos to Grid Pos
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        GridPosition roughXZ = new GridPosition(
        Mathf.RoundToInt(worldPosition.x / cellSize),
        Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET_MULTIPLIER), // dividng removes the compounding offset mouse position error when moving up the grid. 
        floor
        );

        bool oddRow = roughXZ.z % 2 == 1;
        List<GridPosition> neighbourGridPositionList = new List<GridPosition>
        {
            roughXZ + new GridPosition(-1, 0, floor),
            roughXZ + new GridPosition(+1, 0, floor),

            roughXZ + new GridPosition(0, +1, floor),
            roughXZ + new GridPosition(0, -1, floor),

            roughXZ + new GridPosition(oddRow ? +1 : -1, +1, floor),
            roughXZ + new GridPosition(oddRow ? -1 : +1, -1, floor),
        };

        GridPosition closestGridPosition = roughXZ;

        //test world positions in foreach loop then get the world position for each grid position,
        // compare distance of the neighbour position to the roughxz, if the neighbour is closer than
        // the new closest point is that neighbour position. 
        foreach(GridPosition neighbourgridPosition in neighbourGridPositionList)
        {
          if(  Vector3.Distance(worldPosition, GetWorldPosition(neighbourgridPosition)) <
                Vector3.Distance(worldPosition, GetWorldPosition(closestGridPosition)))
            {
                // closer than the closest
                closestGridPosition = neighbourgridPosition;
            }
        }
        return closestGridPosition;
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {  GridPosition gridPosition = new GridPosition(x, z, floor);

               Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
               GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
             //  Debug.Log(GetGridObject(gridPosition));
               gridDebugObject.SetGridObject(GetGridObject(gridPosition)); // instead of casting as gridObjcet we can now le tit be whatever type it is since  gridobject changed in the GridDebugClass to be of type object
              
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition) => gridObjectArray[gridPosition.x, gridPosition.z];

    //Grid validation to prevent illegal moves
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.z >= 0 &&
               gridPosition.x < width &&
               gridPosition.z < height &&
               gridPosition.floor == floor;
    }

    public int GetWidth() => width;

    public int GetHeight() => height;

    public int GetFloor() => floor;

    public static implicit operator GridSystemHex<TGridObject>(GridSystemHex<PathNode> v)
    {
        throw new NotImplementedException();
    }
}
/*
 * original attempt at closest position check (somewhat right idea, wrong approach) .
 foreach(GridPosition gridPosition in neighbourGridPositionList)
        {
            if(gridPosition.z < roughXZ.z)
            {
                return gridPosition;
            }
        }*/
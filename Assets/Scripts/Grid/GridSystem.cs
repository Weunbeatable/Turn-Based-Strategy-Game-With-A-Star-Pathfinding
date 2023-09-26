using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject> 
{
    private int width; 
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;
                                                            // this helps us bypass C# limit on contstraints by passing a delegate which will create our object
    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject) 
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x,z] = createGridObject(this, gridPosition); // get around constraints by passing a delegate that will take our object. 
                // can't create object of type because of no New constraint is saying we can't make the type cause the'res no gaurantee the type we make will have to use the new constraint
                // However C# won't allow you to use a paramterless constructor but we can use delegates to recieve a delegate that creates the object instead. Like a func which is a delegate that returns something 
            }
        }
       
    }
    // convert grid position to world pos
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
  
    }

    //convert World Pos to Grid Pos
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
        Mathf.RoundToInt(worldPosition.x / cellSize),
        Mathf.RoundToInt(worldPosition.z / cellSize)
        );
            
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {  GridPosition gridPosition = new GridPosition(x, z);
               Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
               GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                Debug.Log(GetGridObject(gridPosition));
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
               gridPosition.z < height;
    }

    public int GetWidth() => width;

    public int GetHeight() => height;
}

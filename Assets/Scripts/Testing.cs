using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;
    void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition startGridPosition = new GridPosition(0, 0);

            List<GridPosition> gridPositionList =  Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition);

            for (int i = 0; i < gridPositionList.Count - 1; i++) // count - 1 to ensure we still have a position, we'll use this for loop to draw our what this pathfinding would look like
            {
                Debug.DrawLine(
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]),
                    Color.red,
                    10f
                    );
                Debug.Log("Starting point is" + LevelGrid.Instance.GetWorldPosition(gridPositionList[i]));
                Debug.Log("end point is " + LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]));
            }
            
        }
    }


}

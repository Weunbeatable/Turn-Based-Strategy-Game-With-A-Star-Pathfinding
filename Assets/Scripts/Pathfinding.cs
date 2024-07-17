using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    //Constants for defining movement costs
    // 10 and 14 becase it qould be 1 and 1.4 if using grid positions directly, to aovid floats we use sqrroot of 1 + 1 which is 1.4
    // so converted to ints 1 = 10 and 1.4 = 14
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14; 

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;


    // these int values will be setup when making connection with level grid so they only need to be defined once
    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem; // store the pathnode here
   
    private void Awake()
    {
        // error check for more than one instance, hence singleton, we'll delete the extra
        if (Instance != null)
        {
            Debug.Log("There's more than one Pathfinding! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("the " + Instance + "has been created");

    }

    private void Start()
    {
      //  Pathfinding.Instance.Setup();
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        // setup open and closed lists
        List<PathNode> openList = new List<PathNode>(); // nodes available for searching
        List<PathNode> closedList = new List<PathNode>(); // items we already searched

        //add start node
        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endtNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        // cycle through nodes and reset states
        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);


                // initalize pathNode object
                pathNode.SetGCost(int.MaxValue); // Reset G cost to infinite then recalculate it when we get to the node, we'll set it to the max value
                pathNode.SetHCost(0); // its our heuristic cost so on initalization it should be set to 0 since we don't know
                pathNode.CalculateFCost(); // calculate cost of the two g and h cost
                pathNode.ResetCameFromPathNode(); // reset our path node
            }
        }
        // start searching for a path
        startNode.SetGCost(0); // moving from start node to node we are testing value of (cost)
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition)); // basic guess for the value
        startNode.CalculateFCost();

        // while the open list has elements to search
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList); // don't want only first position, we want node that has lowest f cost so algorithm calculates quickly

            // check if current node is final node
            if(currentNode == endtNode) // that means we reached the final node
            {
                //calculate path
                pathLength = endtNode.GetFCost();
                return CalculatePath(endtNode);
            }
            //remove node from open list then add to closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode); // allready serached through node

            // search through neighbours of node
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                // check if neighbour is already on closed list, already serached
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                //Check to see if the neighbour node is walkable
                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue; // skip all the unwalkable nodes with this script
                }
                // if not we keep going
                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if(tentativeGCost < neighbourNode.GetGCost()) // that means we found a better path i.e like a diagonal instead of two straight movements ( better than what we were previously testing)
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) // if openlist doesn't contain neighbour node, we'll add it to the open list
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        // no longer any open nodes and we still couldn't reach target
        //no path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB) // calculation will be distance from start to end
    {
        GridPosition gridPositionDistance =  gridPositionA - gridPositionB;
        int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z); // for consideration of diagonal
        // we can move diagonally for as many x's as there are z's i.e 1, 1 is just one diagonal and 1,2 is still one diagonal
        int remaining = (int)MathF.Abs(xDistance - zDistance); // we calculate heuristic cost for distance while keeping into account diagonal adn straight distance. 
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining; // we want a positive value because we aren't going in the negative directino so we grab the ABS value;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i]; // if it happens the value we cycle is lower than whats stored change whats stored to be the cycled value.
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        //Initalize list
        List<PathNode> neighbourList = new List<PathNode>();

        // adding neighbours. 
        GridPosition gridPosition = currentNode.GetGridPosition();
        // We'll include Diangonal movement as well 

        // checks for valid positions
        if(gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            if(gridPosition.z - 1 >= 0) // Make sure value we are checkign are above 0 (Grid does not include negative positions)
            {
                // LeftDown
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }
            if(gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // LeftUp
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
            
        }

        if(gridPosition.x + 1 < gridSystem.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                // RightDown
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // RightUp
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.z - 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.z + 0, gridPosition.z + 1));
        }
                
                
        return neighbourList;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height; ;
        this.cellSize = cellSize;


        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));

        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        //Cycle through whole list see if there is anything in there if there it is unwalkable otherwise walkable. 
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 WorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition); // origin of raycast position have to be careful because if origin is inside a collider it wont work
                float raycastOffsetDistance = 5f; // raycast offset to prevent raycast origin being stuck in a collider at the start
                // multiplying raycast offset distance by 2 will help to prevent this issue, alternatively using queries hit backface in the physics tab in project settings fixes this issue. 
                if(Physics.Raycast(WorldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up,
                    raycastOffsetDistance * 2,
                    obstaclesLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }
    private List<GridPosition> CalculatePath(PathNode endtNode)
    {
        // after creating list, start on final node, check if there is any node connected to it, if there is we add linked node to the list and make it the current node
        // we then cycle through and see if that current node has a link and keep repeating the process until there are no more links being made. 

        // instantiate the list
        List<PathNode> pathNodeList = new List<PathNode>();
        // add the end node on to it
        pathNodeList.Add(endtNode); // this will be start node and start node would be the final node. 
        // Cycle through list (walk backwards)
        PathNode currentNode = endtNode;
        while(currentNode.GetCameFromPathNode() != null) // if not null that means some node is connected to it
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        pathNodeList.Reverse(); // this way the end node goes to the end of the list as it should be while the start node stays at the start. 

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }


        return gridPositionList;
    }

    public void SetsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }
    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
      return  FindPath(startGridPosition, endGridPosition, out int pathLength) != null; // checking if there is a valid path to walk to, helps for obscure obstacles. 
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    //Constants for defining movement costs
    // hex grids have all same costs in all directions so no need for a separate diagonal cost. 
    private const int MOVE_STRAIGHT_COST = 10;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private Transform pathfindingLinkContainer;


    // these int values will be setup when making connection with level grid so they only need to be defined once
    private int width;
    private int height;
    private float cellSize;
    private int floorAmount;
    private List<GridSystemHex<PathNode>> gridSystemList; // store the pathnode here
    private List<PathfindingLink> pathfindingLinkList;
   
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
        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        // cycle through nodes and reset states
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition); // floor check before grid position. 


                    // initalize pathNode object
                    pathNode.SetGCost(int.MaxValue); // Reset G cost to infinite then recalculate it when we get to the node, we'll set it to the max value
                    pathNode.SetHCost(0); // its our heuristic cost so on initalization it should be set to 0 since we don't know
                    pathNode.CalculateFCost(); // calculate cost of the two g and h cost
                    pathNode.ResetCameFromPathNode(); // reset our path node
                }
            }
        }
        // start searching for a path
        startNode.SetGCost(0); // moving from start node to node we are testing value of (cost)
        startNode.SetHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition)); // basic guess for the value
        startNode.CalculateFCost();

        // while the open list has elements to search
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList); // don't want only first position, we want node that has lowest f cost so algorithm calculates quickly

            // check if current node is final node
            if(currentNode == endNode) // that means we reached the final node
            {
                //calculate path
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
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
                int tentativeGCost = currentNode.GetGCost() + MOVE_STRAIGHT_COST;

                if(tentativeGCost < neighbourNode.GetGCost()) // that means we found a better path i.e like a diagonal instead of two straight movements ( better than what we were previously testing)
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateHeuristicDistance(neighbourNode.GetGridPosition(), endGridPosition));
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

    public int CalculateHeuristicDistance(GridPosition gridPositionA, GridPosition gridPositionB) // calculation will be distance from start to end
    {
        return Mathf.RoundToInt( MOVE_STRAIGHT_COST *
            Vector3.Distance(gridSystemList[floorAmount -1].GetWorldPosition(gridPositionA), gridSystemList[floorAmount -1].GetWorldPosition(gridPositionB)));

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

    // helper function 
    private GridSystemHex<PathNode> GetGridSystem(int floor)
    {
        return gridSystemList[floor];
    }
    private PathNode GetNode(int x, int z, int floor)
    {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
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
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
       
            
        }

        if(gridPosition.x + 1 < width)
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
        }

        if (gridPosition.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.z - 0, gridPosition.z - 1, gridPosition.floor));
        }
        if (gridPosition.z + 1 < height)
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.z + 0, gridPosition.z + 1, gridPosition.floor));
        }

        bool oddRow = gridPosition.z % 2 == 1;
        if(oddRow)
        {
            if (gridPosition.x + 1 < width)
            {
                if (gridPosition.z - 1 >= 0)
                {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
                }
                if (gridPosition.z + 1 < height)
                {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
                }
            }
        }
        else
        {
            // even row logic 
            if (gridPosition.x - 1 >= 0)
            {
                if (gridPosition.z - 1 >= 0)
                {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
                }
                if (gridPosition.z + 1 < height)
                {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
                }
            }
        }

        // add neighbours above and below
        List<PathNode> totalNeigbourList = new List<PathNode>();
        totalNeigbourList.AddRange(neighbourList);

        List<GridPosition> pathfindingLinkGridPositionList = GetPathfindingLinkConnectedGridPositionList(gridPosition);

        //nodes will be valid neighbours only if there is a link
        foreach (GridPosition pathfindingLinkGridPosition in pathfindingLinkGridPositionList)
        {
            totalNeigbourList.Add(
                GetNode(pathfindingLinkGridPosition.x,
                pathfindingLinkGridPosition.z,
                pathfindingLinkGridPosition.floor) );
        }

        return totalNeigbourList;
    }

    private List<GridPosition> GetPathfindingLinkConnectedGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        // check to ssee if positions on floors link to each other (check both ways) a list will be returned
        // as there can be multiple links e.g. edge of a building where there are multiple entry positions
        foreach (PathfindingLink pathfindingLink in pathfindingLinkList)
        {
            if(pathfindingLink.gridPositionA == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionB);
            }
            if (pathfindingLink.gridPositionB == gridPosition)
            {
                gridPositionList.Add(pathfindingLink.gridPositionA);
            }
        }

        return gridPositionList;
    }

    //TODO: modify to show several floors. 
    public void Setup(int width, int height, float cellSize, int floorAmount)
    {
        this.width = width;
        this.height = height; ;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;


        gridSystemList = new List<GridSystemHex<PathNode>>();

        for( int floor = 0; floor < floorAmount; floor++ )
        {
           GridSystemHex<PathNode> gridSystem = new GridSystemHex<PathNode>(width, height, cellSize, floor, LevelGrid.FLOOR_HEIGHT,
           (GridSystemHex<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));

            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

            gridSystemList.Add(gridSystem);
        }



        //Cycle through whole list see if there is anything in there if there it is unwalkable otherwise walkable. 
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                for(int floor = 0; floor < floorAmount; floor++ )
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Vector3 WorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition); // origin of raycast position have to be careful because if origin is inside a collider it wont work
                    float raycastOffsetDistance = 1f; // raycast offset to prevent raycast origin being stuck in a collider at the start value changes for multifloors. 
                                                      // multiplying raycast offset distance by 2 will help to prevent this issue, alternatively using queries hit backface in the physics tab in project settings fixes this issue. 
                    
                    GetNode(x, z, floor).SetIsWalkable(false);

                    //vector3.up then Vector3.down for the collider raycast collision as it works weirdly on meshes with colliders like whats on the plane
                    if (Physics.Raycast(WorldPosition + Vector3.up * raycastOffsetDistance,
                        Vector3.down,
                        raycastOffsetDistance * 2,
                        floorLayerMask)) //  raycast to see what nodes are walkable for multifloors
                    {
                        GetNode(x, z, floor).SetIsWalkable(true);
                    }
                    if (Physics.Raycast(WorldPosition + Vector3.down * raycastOffsetDistance,
                      Vector3.up,
                      raycastOffsetDistance * 2,
                      obstaclesLayerMask)) 
                    {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }             
            }
        }

        
        pathfindingLinkList = new List<PathfindingLink>();
        foreach(Transform pathFindingLinkTransform in pathfindingLinkContainer)
        {
            if(pathFindingLinkTransform.TryGetComponent(out PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour))
            {
                pathfindingLinkList.Add(pathfindingLinkMonoBehaviour.GetPathfindingLink());
            }
        }
        pathfindingLinkList.Add(new PathfindingLink
        {
            gridPositionA = new GridPosition(3, 6, 0),
            gridPositionB = new GridPosition(3, 7, 1),

        });
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
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }
    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
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

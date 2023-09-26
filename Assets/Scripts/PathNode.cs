using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    /// <summary>
    /// will implement A star pathfinding
    /// will need our g cost to find price to get from one node to the next
    /// our h cost for our heuristic cost assumin gno walls whats the shortest route
    /// and our f cost to add the two values together as we go along. this will help us with our opened and closed list of values. 
    /// </summary>
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromPathNode; // we need to store the reference of the pathnode we came from in order to reach this path node.  so we have to store this 
    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;

    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }
    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }
    public PathNode GetCameFromPathNode() => cameFromPathNode;
    public GridPosition GetGridPosition() => gridPosition;
}

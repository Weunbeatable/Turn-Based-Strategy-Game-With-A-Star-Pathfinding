using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathFindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;

    private PathNode pathNode;
    public override void SetGridObject(object gridObject) // this function will cast the gridobject down to a path node then store it and run the base class functino that stores the grid object
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
        
    }

    protected override void Update() // override the update function from gridDebugObject that we inhereted from, will to string the values of grid object (from base debug object class) still showing grid pos and then display costs
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
    }
}

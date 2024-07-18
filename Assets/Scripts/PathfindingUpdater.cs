using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{

    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
    }

    private void DestructibleCrate_OnAnyDestroyed(object sender, System.EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        Pathfinding.Instance.SetsWalkableGridPosition(destructibleCrate.GetGridPosition(), true);
    }
}

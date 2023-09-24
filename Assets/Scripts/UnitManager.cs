using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We will edit script execution order to make sure this script runs before other scrits that depend on checking to see if units spawned or not
// otherwise it will create errors of not adding units correctly 
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; } // we only need 1 field for the class so its static

    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake()
    {
        // error check for more than one instance, hence singleton, we'll delete the extra
        if (Instance != null)
        {
            Debug.Log("There's more than one UnitActionSystem! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //initalize lists
        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }
    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    { //Unit will act as sender, we'll check the status of the unit, friend or foe and add them to respective lists, regardless all units will go into the unit list
        Unit unit = sender as Unit;

        Debug.Log("Unit Spawned");

        unitList.Add(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        // Same as adding units above this time we remove
        Unit unit = sender as Unit;

        Debug.Log("Unit Removed");

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList() => unitList;
    public List<Unit> GetFriendlyUnitList() => friendlyUnitList;
    public List<Unit> GetEnemyUnitList() => enemyUnitList;


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    public static ArrayList PlayerUnits = new ArrayList();
    public static ArrayList EnemyUnits = new ArrayList();
    private static UnitClass selectedUnit;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            print("Enemys");
            foreach (UnitClass enemy in EnemyUnits)
                print(enemy.unitClassName);
            print("PlayerUnits");
            foreach (UnitClass unit in PlayerUnits)
                print(unit.unitClassName);
        }
    }

    public static void newTurn()
    {
        foreach (UnitClass unit in PlayerUnits)
            unit.newTurn();
    }

    public static void aiTurn()
    {
        foreach (UnitClass unit in EnemyUnits)
            unit.newTurn();
    }

    public static void setSelectedUnit(UnitClass unit)
    {
        selectedUnit = unit;
    }

    public static UnitClass getSelectedUnit()
    {
        return selectedUnit;
    }
}

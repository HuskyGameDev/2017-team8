using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour
{

    public ArrayList PlayerUnits = new ArrayList();
    public ArrayList EnemyUnits = new ArrayList();
    private UnitClass selectedUnit;
    public int hasWon = 0;
    private AI ai;

    // Use this for initialization
    void Start()
    {
        ai = GameObject.Find("AI").GetComponent<AI>();
    }

    void Awake()
    {
        ai = GameObject.Find("AI").GetComponent<AI>();
    }

    // Update is called once per frame
    void Update()
    {
		if (hasWon == 0)
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				print("Enemys");
				foreach (UnitClass enemy in EnemyUnits)
					print(enemy.unitClassName);
				print("PlayerUnits");
				foreach (UnitClass unit in PlayerUnits)
					print(unit.unitClassName);
			}
			// Win/Lose condition checking
			if (hasWon == 0)
			{
				if (PlayerUnits.Count == 0)
				{
					print("You Lose!");
					hasWon = 1;
				}
				if (EnemyUnits.Count == 0)
				{
					print("You Win!");
					try
					{
						Transform unit = this.transform;
						for (int i = 0; i < unit.childCount; i++)
						{
							GameObject indicator = unit.GetChild(i).gameObject;
							if (indicator != null && indicator.name == "winIndicator") indicator.SetActive(true);
						}

					}
					catch (Exception e)
					{

					}
					hasWon = 1;
				}
			}
		}
    }

    public void newTurn()
    {
        foreach (UnitClass unit in PlayerUnits)
            unit.newTurn();
    }

    public void aiTurn()
    {
        foreach (UnitClass unit in EnemyUnits)
            unit.newTurn();
    }
    public void unitKilled(UnitClass unit)
    {
        if (unit.tag == "PlayerUnit")
        {
            PlayerUnits.Remove(unit);
            ai.remove(unit);
        }
        else
        {
            EnemyUnits.Remove(unit);
        }
    }

    public void setSelectedUnit(UnitClass unit)
    {
        selectedUnit = unit;
    }

    public UnitClass getSelectedUnit()
    {
        return selectedUnit;
    }
    public bool unitsAssignedTiles()
    {
        foreach (UnitClass unit in PlayerUnits)
        {
            if(unit.currentTile == null)
            {
                return false;
            }
        }
        foreach (UnitClass unit in EnemyUnits)
        {
            if(unit.currentTile == null)
            {
                return false;
            }
        }
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

	public static ArrayList PlayerUnits = new ArrayList();
	public static ArrayList EnemyUnits = new ArrayList();
	private static UnitClass selectedUnit;
	public int hasWon = 0;

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
		// Win/Lose condition checking
		if( hasWon == 0)
		{
			if(PlayerUnits.Count == 0){
				print("You Lose!");
				hasWon = 1;
			}
			if(EnemyUnits.Count == 0){
				print("You Win!");
				hasWon = 1;
			}
		}
	}

	public static void newTurn()
	{
		foreach (UnitClass unit in PlayerUnits)
		{
			unit.newTurn();
			
		}
	}

	public static void aiTurn()
	{
		foreach (UnitClass unit in EnemyUnits)
			unit.newTurn();
	}
	public static void unitKilled(UnitClass unit)
	{
		print("this should be called");
		if (unit.tag == "PlayerUnit")
		{
			PlayerUnits.Remove(unit);
		} else
		{
			EnemyUnits.Remove(unit);
		}
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

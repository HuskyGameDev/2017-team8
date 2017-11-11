using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : UnitClass {

	public RangedUnit(){
		UnitClassName = "Ranged";
		//Sprite = ""
		Health = 3;
		Speed = 2;
		Range = 3;
		Damage = 4;
		Cost = 175;
		//CurrentTile = Tile;
	}

	void Start(){
		//RangedUnit();
		RangedUnit range = new RangedUnit();
		//print("Hello");
		//base.currentx = 2;
		GameObject enemy;
		string eName;
		enemy = base.EnemyInRange(range.Range);
		enemy = GameObject.GetComponent(MeleeUnit);
		
		//print("Enemy " + enemy.UnitClassName.get() + " found");
		//string eName = enemy.base.;
		print("I am a " + range.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);
	}
}

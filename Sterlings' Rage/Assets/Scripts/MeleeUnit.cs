using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : UnitClass {

	public MeleeUnit(){
		UnitClassName = "Melee";
		//Sprite = ""
		Health = 5;
		Speed = 3;
		Range = 1;
		Damage = 2;
		Cost = 50;
	}

	void Start(){
		MeleeUnit melee = new MeleeUnit();
		print("I am a " + melee.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);
	}
}

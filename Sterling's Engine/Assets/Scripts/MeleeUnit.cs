using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : UnitClass {

	public MeleeUnit(){
		unitClassName = "Melee";
		//Sprite = ""
		Health = 5;
		Speed = 3;
		Range = 1;
		Damage = 2;
		Cost = 50;
	}
}

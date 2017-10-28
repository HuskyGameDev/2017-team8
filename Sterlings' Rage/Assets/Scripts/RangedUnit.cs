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
	}
}

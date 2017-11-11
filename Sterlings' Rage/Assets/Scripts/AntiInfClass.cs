using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiInfClass : UnitClass {

	public AntiInfClass(){
		UnitClassName = "Anti-Infantry";
		//Sprite = ""
		Health = 3;
		Speed = 3;
		Range = 1;
		//If not Infantry Damage = 2
		//Else
		Damage = 4;
		Cost = 85;
	}

	void Start(){
		AntiInfClass anti = new AntiInfClass();
		print("I am an " + anti.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);
	}
}

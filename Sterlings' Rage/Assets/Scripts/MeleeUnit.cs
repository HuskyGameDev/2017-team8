using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : UnitClass {

	public MeleeUnit(){
		UnitClassName = "MeleeUnit";
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
        if (TileManager.mapTiles != null && TileManager.checkIfFull())
        {
            currentTile = TileManager.mapTiles[(int)transform.position.x, (int)transform.position.y];
            print("currentTile " + currentTile.getXPosition() + "," + currentTile.getYPosition());
            currentTile.currentUnit = this;
        }
    }

    void Update(){
    	base.Update();
    	if(Health <= 0){
    		print("Melee is Dead!");
    		Destroy(gameObject);
    	}
    }
}

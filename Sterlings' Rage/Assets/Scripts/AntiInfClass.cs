using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiInfClass : UnitClass {

	public AntiInfClass(){
		UnitClassName = "AntiInfantry";
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

        // Need update the units current tile as well is indicate the tile that there is now a unit on it
        if (TileManager.mapTiles != null && TileManager.checkIfFull())
        {
            currentTile = TileManager.mapTiles[(int)transform.position.x, (int)transform.position.y];
            print("currentTile " + currentTile.getXPosition() + "," + currentTile.getYPosition());
            currentTile.currentUnit = this;
        }
        if (gameObject.tag == "PlayerUnit")
            UnitManager.PlayerUnits.Add(this);
        else
            UnitManager.EnemyUnits.Add(this);
        AntiInfClass anti = new AntiInfClass();
		print("I am an " + anti.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);
	}

	void Update(){
		base.Update();
    	if(Health <= 0){
    		print("Anti is Dead!");
    		Destroy(gameObject);
    	}
    }
}

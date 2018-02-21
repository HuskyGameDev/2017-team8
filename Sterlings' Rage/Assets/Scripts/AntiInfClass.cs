using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiInfClass : UnitClass {

	public AntiInfClass(){
		UnitClassName = "AntiInfantry";
		UnitType = "Infantry";
		//Sprite = ""
		health = 3;
        checkHealth = 3;
		Speed = 2;
		Range = 5;
		//If not Infantry Damage = 2
		//Else
		Damage = 3;
		Cost = 175;

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

	}

	void Update(){
		base.Update();
    	if(health <= 0){
    		print("Anti is Dead!");
            UnitManager.unitKilled(this);
            checkHealth = 0;
    	}
    }
}

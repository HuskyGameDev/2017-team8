using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : UnitClass {

	public RangedUnit(){
		UnitClassName = "RangedUnit";
		UnitType = "Infantry";
		//Sprite = ""
		Health = 3;
		Speed = 2;
		Range = 3;
		//  If attacking infantry damage is 7
		Damage = 3;
		Cost = 175;
		//CurrentTile = Tile;
	}

	void Start(){
        newTurn();

        // Need update the units current tile as well is indicate the tile that there is now a unit on it
        if(TileManager.mapTiles != null && TileManager.checkIfFull())
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
    	if(Health <= 0){
    		print("Ranged is Dead!");
            UnitManager.unitKilled(this);
            Destroy(gameObject);
    	}
    }
}

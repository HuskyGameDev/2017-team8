using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameUnit : UnitClass {

	public FlameUnit(){
		UnitClassName = "FlameUnit";
		UnitType = "Infantry";
		//Sprite = ""
		Health = 7;//7
		Speed = 3;//3
		Range = 1;//1
		//  If attacking infantry damage is 7
		Damage = 5;//5
		Cost = 85;//85
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

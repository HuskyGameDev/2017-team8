using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : UnitClass {

	public MeleeUnit(){
		UnitClassName = "MeleeUnit";
		UnitType = "Infantry";
		//Sprite = ""
		Health = 5;
		Speed = 3;
		Range = 1;
		Damage = 3;
		Cost = 50;
	}

	void Start(){
        
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
    	if(Health <= 0){
    		print("Melee is Dead!");
            UnitManager.unitKilled(this);
            Destroy(gameObject);
    	}
    }
}

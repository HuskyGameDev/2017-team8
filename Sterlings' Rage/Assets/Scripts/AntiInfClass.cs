using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiInfClass : UnitClass {

    private TileManager tileManager;
    private UnitManager unitManager;

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
        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
        // Need update the units current tile as well is indicate the tile that there is now a unit on it
        if (tileManager.mapTiles != null && tileManager.checkIfFull())
        {
            currentTile = tileManager.mapTiles[(int)transform.position.x, (int)transform.position.y];
            print("currentTile " + currentTile.getXPosition() + "," + currentTile.getYPosition());
            currentTile.currentUnit = this;
        }
        if (gameObject.tag == "PlayerUnit")
            unitManager.PlayerUnits.Add(this);
        else

            unitManager.EnemyUnits.Add(this);
        AntiInfClass anti = new AntiInfClass();
		print("I am an " + anti.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);
    }

	void Update(){
		base.Update();
    	if(health <= 0){
    		print("Anti is Dead!");
            UnitManager.unitKilled(this);

    	}
    }
}

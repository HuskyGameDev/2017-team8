using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierUnit : UnitClass {

    private TileManager tileManager;
    private UnitManager unitManager;

	public GrenadierUnit(){
		UnitClassName = "GrenadierUnit";
		UnitType = "Infantry";
		Health = 5;//7
		Speed = 4;//3
		Range = 3;//1
		Damage = 5;//5
		Cost = 100;//85
		
	}

	void Start()
    {
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
        
	}

	void Update(){
		base.Update();
    	if(Health <= 0){
    		print("Ranged is Dead!");
            unitManager.unitKilled(this);
            Destroy(gameObject);
    	}
    }
}

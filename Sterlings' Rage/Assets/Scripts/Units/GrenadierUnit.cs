using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierUnit : UnitClass {

    private TileManager tileManager;
    private UnitManager unitManager;

	public GrenadierUnit(){
		UnitClassName = "GrenadierUnit";
		UnitType = "Infantry";
		Health = 10;//7
		Speed = 4;//3
		Range = 4;//1
		Damage = 3;//5
		Cost = 100;//85
		
	}

void Start(){

        base.Start();
        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
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
    		print("Melee is Dead!");
            unitManager.unitKilled(this);
            Destroy(gameObject);
    	}
        //damaged tick
        if (damageCount != 0)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}

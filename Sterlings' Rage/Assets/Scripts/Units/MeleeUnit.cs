﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : UnitClass {
    private TileManager tileManager;
    private UnitManager unitManager;

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
            damageCount--;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}

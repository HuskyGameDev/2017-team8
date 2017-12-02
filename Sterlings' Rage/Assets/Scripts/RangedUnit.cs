﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : UnitClass {

	public RangedUnit(){
		UnitClassName = "RangedUnit";
		//Sprite = ""
		Health = 3;
		Speed = 2;
		Range = 3;
		Damage = 4;
		Cost = 175;
		//CurrentTile = Tile;
	}

	void Start(){

        // Need update the units current tile as well is indicate the tile that there is now a unit on it
        if(TileManager.mapTiles != null && TileManager.checkIfFull())
        {
            currentTile = TileManager.mapTiles[(int)transform.position.x, (int)transform.position.y];
            print("currentTile " + currentTile.getXPosition() + "," + currentTile.getYPosition());
            currentTile.currentUnit = this;
        }
        //RangedUnit();
        RangedUnit range = new RangedUnit();
		//print("Hello");
		//base.currentx = 2;
		GameObject enemy;
		//string eName;
		enemy = base.EnemyInRange(range.Range);
		//eName = enemy.GetComponent(base.Melee).get();
		
		//print("Enemy " + enemy.UnitClassName.get() + " found");
		//string eName = enemy.base.;
		print("I am a " + range.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);
	}
}

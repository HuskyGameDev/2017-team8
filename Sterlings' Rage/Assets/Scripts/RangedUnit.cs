using System.Collections;
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
        RangedUnit range = new RangedUnit();
        int curHealth =  range.Health;
        print("Health is " + curHealth);
		GameObject enemy;
		enemy = base.EnemyInRange(range.Range);
		print("Enemy is " + enemy);
		
		int inRange = 0;
		if(enemy != null){
			print("Made it");
			inRange = 1;
		}
		print("inRange is " + inRange);
		print("I am a " + range.UnitClassName + " unit. I am located at " + transform.position.x + "," + transform.position.y);

		if(inRange == 1){
			print("Enemy Health " + enemy.GetComponent<MeleeUnit>().Health);
			enemy.GetComponent<MeleeUnit>().Health -= range.Damage;
			print("Enemy Health " + enemy.GetComponent<MeleeUnit>().Health);
			//enemy.GetComponent<curHealth>()
		}
	}

	void Update(){
        base.Update();
			if(Input.GetMouseButtonDown(0)){
				print("Pressed Left click");
			}
	}
}

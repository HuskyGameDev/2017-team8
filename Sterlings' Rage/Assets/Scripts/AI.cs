using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    private bool aiTurn = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!TurnManager.playerTurn && !aiTurn)
        {
            aiTurn = true;
            takeTurn();
        }
		
	}

    private void takeTurn()
    {
        foreach (UnitClass unit in UnitManager.EnemyUnits)
        {
            UnitClass playerUnit = findNearestPlayerUnit(unit);
            print("AI unit at " + unit.gameObject.transform.position.x + "," + unit.gameObject.transform.position.y+ "  found closest to be " 
                + playerUnit.gameObject.transform.position.x + ", " + playerUnit.gameObject.transform.position.y);
            if (playerUnit != null)
            {
                if (tilesTo(unit.gameObject, playerUnit.gameObject) > unit.range)
                    move(unit, playerUnit);
                if (tilesTo(unit.currentTile.gameObject, playerUnit.gameObject) <= unit.range)
                    attack(unit, playerUnit);
            }
        }
        TurnManager.newTurn();
        aiTurn = false;

    }

    /**
     * Moves the aiUnit towards the playerUnit
     */
    private void move (UnitClass aiUnit, UnitClass playerUnit)
    {
        aiUnit.displayMovementPath();
        int distance = int.MaxValue;
        MapTile targetTile = null;
        foreach (MapTile tile in TileManager.getPathList())
        {
            if (tilesTo(playerUnit.gameObject, tile.gameObject) < distance && tile.currentUnit == null)
            {
                distance = tilesTo(playerUnit.gameObject, tile.gameObject);
                targetTile = tile;
            }
        }

        ArrayList movementPath = targetTile.highlightRoute(false);
        aiUnit.MoveTo(movementPath, targetTile);
        targetTile.currentUnit = aiUnit;
        TileManager.resetAllTiles();
    }

    private void attack (UnitClass aiUnit, UnitClass playerUnit)
    {
        print("yeah the computers killing things");
        playerUnit.currentTile.attack(aiUnit, playerUnit, aiUnit.currentTile, playerUnit.currentTile);
        TileManager.resetAllTiles();
    }

    private int tilesTo (GameObject unit, GameObject otherUnit)
    {
        return (int)(Mathf.Abs((unit.transform.position.x - otherUnit.transform.position.x)) + Mathf.Abs((unit.transform.position.y - otherUnit.transform.position.y)));
    }

    private UnitClass findNearestPlayerUnit(UnitClass aiUnit)
    {
        float minDist = float.MaxValue;
        UnitClass nearestUnit = null;
        foreach (UnitClass playerUnit in  UnitManager.PlayerUnits)
        {

            float curDist = distanceBetween(playerUnit.gameObject, aiUnit.gameObject);
            print("playerUnit at " + playerUnit.transform.position.x + "," + playerUnit.transform.position.y + " and " + curDist);
            if (curDist < minDist)
            {
                nearestUnit = playerUnit;
                minDist = curDist;
            }
        }
        return nearestUnit;
    }

    private float distanceBetween(GameObject unit, GameObject otherUnit)
    {
        return Mathf.Sqrt(Mathf.Pow((unit.transform.position.x - otherUnit.transform.position.x),2f) 
            + Mathf.Pow((unit.transform.position.y - otherUnit.transform.position.y),2f));
    }
}

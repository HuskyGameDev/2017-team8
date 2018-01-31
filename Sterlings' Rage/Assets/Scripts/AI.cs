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
            // Need coroutine so that the AI can wait for units to move before attacking
            StartCoroutine(takeTurn());
        }
		
	}

    private IEnumerator takeTurn()
    {
        // Makes a copy of the AI's current units because if one happens to die it will change the list causing the following loop to crash
        ArrayList temp = new ArrayList();
        foreach (UnitClass unit in UnitManager.EnemyUnits)
            temp.Add(unit);
        
        foreach (UnitClass unit in temp)
        {
            UnitClass playerUnit = findNearestPlayerUnit(unit);
            if (playerUnit != null)
            {
                if (tilesTo(unit.gameObject, playerUnit.gameObject) > unit.range)
                    yield return StartCoroutine(move(unit, playerUnit));
                // Currently possible for a different unit to kill the inteded target before reaching this point so
                // null check makes sure it is still alive
                if (playerUnit != null && tilesTo(unit.currentTile.gameObject, playerUnit.gameObject) <= unit.range)
                    yield return StartCoroutine(attack(unit, playerUnit));
            }
        }
        TurnManager.newTurn();
        aiTurn = false;

    }

    /**
     * Moves the aiUnit towards the playerUnit
     */
    private IEnumerator move (UnitClass aiUnit, UnitClass playerUnit)
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
        return new WaitUntil(()=> aiUnit.moving == false);
    }

    private IEnumerator attack (UnitClass aiUnit, UnitClass playerUnit)

    {
        print("yeah the computers killing things");
        playerUnit.currentTile.attack(aiUnit, playerUnit, aiUnit.currentTile, playerUnit.currentTile);
        //play animation
        aiUnit.gameObject.GetComponent<Animator>().Play("Attack");
        TileManager.resetAllTiles();
        // Need to return some sort of Enumerator to work as intended
        return new WaitUntil(()=> 1==1);
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

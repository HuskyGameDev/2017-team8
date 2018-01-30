using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    public static bool playerTurn = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void endTurn()
    {
        playerTurn = false;
        UnitManager.aiTurn();
    }

    public static void newTurn()
    {
        UnitManager.newTurn();
        TileManager.resetAllTiles();
        playerTurn = true;
    }
}

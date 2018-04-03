using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    public bool playerTurn = true;
    private TileManager tileManager;
    private UnitManager unitManager;

	// Use this for initialization
	void Start ()
    {
        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();

    }
	
	// Update is called once per frame
	void Update () {
        if (unitManager == null || tileManager == null)
        {
            unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
            tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
        }
		
	}

    public void endTurn()
    {
        playerTurn = false;
        unitManager.aiTurn();
    }

    public void newTurn()
    {
        unitManager.newTurn();
        tileManager.resetAllTiles();
        playerTurn = true;
        //tileManager.GameStateUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour {

    void OnMouseDown()
    {
        UnitManager.newTurn();
    }

    private void OnMouseOver()
    {
        print("the mouse is over this");
    }

    // Use this for initialization
    void Start () {
        print("something has to happen");
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TileManager.resetAllTiles();
            UnitManager.newTurn();
            print("Ending turn");
        }
    }
}

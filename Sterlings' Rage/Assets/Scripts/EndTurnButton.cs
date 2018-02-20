using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour {
    private TurnManager turnManager;

    public void OnMouseDown()
    {
        endTurn();
    }

    private void endTurn()
    {
        turnManager.endTurn();
    }

    // Use this for initialization
    void Start () {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (turnManager == null)
            turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        if (Input.GetKeyDown(KeyCode.E))
        {
            endTurn();
        }
    }
}

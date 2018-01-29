using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour {

    public void OnMouseDown()
    {
        print("This was clicked");
        endTurn();
    }

    private void endTurn()
    {
        TurnManager.endTurn();
        print("Ending turn");
    }

    // Use this for initialization
    void Start () {
        print("something has to happen " + gameObject.transform.position.x);
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            endTurn();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

    private bool up = true;
    private bool down = true;
    private bool right = true;
    private bool left = true;
	private bool hasUnit = false;
	private bool mouseOverTest = true;

    // Use this for initialization
    void Start () {
		TileManager.addTile(this,(int)gameObject.transform.position.x, (int)gameObject.transform.position.y);

    }
	
	// Update is called once per frame
	void Update () {
        

    }

    private void OnMouseOver()
    {
		if (mouseOverTest) {
			print ("at" + gameObject.transform.position.x + ", " + gameObject.transform.position.y);
			mouseOverTest = false;
		}
    }

	public void setHasUnit(bool value) {
		hasUnit = value;
	}

}

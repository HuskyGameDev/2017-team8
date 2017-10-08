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
	private bool addedTile = false;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if (!addedTile && TileManager.mapTiles != null) {
			TileManager.addTile (this, (int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
			addedTile = true;
		}
        

    }

    private void OnMouseOver()
    {
		if (mouseOverTest) {
			int x = (int)gameObject.transform.position.x;
			int y = (int)gameObject.transform.position.y;
			print ("at" + x + ", " + y);
			print ("right: " + TileManager.mapTiles[x+1,y]);
			print ("up: "+ TileManager.mapTiles[x,y+1]);
			mouseOverTest = false;
		}
    }

	public void setHasUnit(bool value) {
		hasUnit = value;
	}

}

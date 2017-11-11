using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoToUI : MonoBehaviour {

    public MapTile holding;
    public Text SelectedInfo;
    bool hadRecent = false;
	// Use this for initialization
	void Start () {
		//should only be called once per use, so don't need to use update
        //grab the image of the tile, and send along the pertinant info when the mouse enters this tile

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RecentTile(MapTile recent)
    {
        //This will eventually lead to some errors if we make deformable or changing terrain.
        if (hadRecent)
        {
            ClearRecent();
        }
        else
        {
            hadRecent = true;
        }
        holding = recent;
        Color selected = holding.GetComponent<SpriteRenderer>().color;
        selected.g = 0f;
        holding.GetComponent<SpriteRenderer>().color = selected;
        //getting here means you clicked on a tile, so update the info
		GetComponent<Image>().sprite = holding.GetComponent<SpriteRenderer>().sprite;
        SelectedInfo.text = holding.tileType;
    }
    public void ClearRecent()
    {
        //Add in a case where Recent is null (I want to set it to null when not in use)
        //for now only clears the color selection
        Color selected = holding.GetComponent<SpriteRenderer>().color;
        selected.g = 255f;
        holding.GetComponent<SpriteRenderer>().color = selected;
    }
}

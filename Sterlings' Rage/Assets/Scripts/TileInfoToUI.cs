﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoToUI : MonoBehaviour {

    public MapTile holding;
    public Text TileType;
    public Text UnitType;
    public Text UnitHealth;
    public Text UnitSpeed;
    public Text UnitRange;
    public Text UnitDamage;

    bool hadRecent = false;
	private TileInfoToUI UIInfo;
	private GameObject UIRightClick;
	private GameObject UICanvas;
    private GameObject UIIcon;
    private GameObject UIUnit;
	// Use this for initialization
	void Start () {
		//should only be called once per use, so don't need to use update
        //grab the image of the tile, and send along the pertinant info when the mouse enters this tile
		UICanvas = GameObject.Find("Canvas");
		UIInfo = GameObject.Find("Canvas/TilePanel").GetComponent<TileInfoToUI>();
		UIRightClick = GameObject.Find("Canvas/RightClickPanel");
        UIIcon = GameObject.Find("Canvas/TilePanel/TileImage");
        UIUnit = GameObject.Find("Canvas/TilePanel/UnitImage");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RightClick(){
		//should already have recent, if not, exit with an error
		//this is oh so messy, figure out a better way to get valid coordinates
		//print("MouseX: " + Input.mousePosition.x + " MouseY: " + Input.mousePosition.y + "Screen Width: " + Screen.width + " Screen hiehgt: " + Screen.height + " New X: " + (Input.mousePosition.x-Screen.width/2) + " New Y: " + (Input.mousePosition.y-Screen.height/2));
		Vector3 temp = new Vector3((Input.mousePosition.x/Screen.width)*UICanvas.GetComponent<RectTransform>().rect.width - UICanvas.GetComponent<RectTransform>().rect.width/2 + 35, (Input.mousePosition.y/Screen.height)*UICanvas.GetComponent<RectTransform>().rect.height - UICanvas.GetComponent<RectTransform>().rect.height/2 - 80, 0);
		UIRightClick.transform.localPosition = temp;
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
       
        //getting here means you clicked on a tile, so update the info
		UIIcon.GetComponent<Image>().sprite = holding.GetComponent<SpriteRenderer>().sprite;
        if(holding.GetComponent<MapTile>().currentUnit != null ){
            UIUnit.GetComponent<Image>().sprite = holding.GetComponent<MapTile>().currentUnit.GetComponent<SpriteRenderer>().sprite;
            UIUnit.GetComponent<Image>().enabled = true;
        }
        else
        {
            UIUnit.GetComponent<Image>().enabled = false;
        }
        TileType.text = holding.tileType;
        if (holding.GetComponent<MapTile>().currentUnit != null)
        {
            UnitType.text = holding.currentUnit.UnitClassName.ToString();
            UnitHealth.text = holding.currentUnit.health.ToString();
            UnitSpeed.text = holding.currentUnit.speed.ToString();
            UnitRange.text = holding.currentUnit.range.ToString();
            UnitDamage.text = holding.currentUnit.damage.ToString();
        }
        else
        {
            UnitType.text = "";
            UnitHealth.text = "";
            UnitSpeed.text = "";
            UnitRange.text = "";
            UnitDamage.text = "";
        }
        //UIInfo.GetComponent<Text>().text = SelectedInfo;
    }
    public void ClearRecent()
    {
        //Add in a case where Recent is null (I want to set it to null when not in use)
        //for now only clears the color selection
        if (holding.visible == 1)
        {
            holding.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            holding.GetComponent<SpriteRenderer>().color = Color.grey;
        }
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{

    private MapTile up;
    private MapTile down;
    private MapTile right;
    private MapTile left;
    public string tileType;
    private int movementWeight;
    private UnitClass currentUnit;
    private bool mouseOverTest = true;
    private TileInfoToUI UIInfo;
    private GameObject UIRightClick;
	private GameObject UICanvas;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(initializeGrid());
        movementWeight = determineMovementWeight(tileType);
        //For sending info to the UI, needs to be improved
		//For sending info to the UI, needs to be improved
		UICanvas = GameObject.Find("Canvas");
		UIInfo = GameObject.Find("Canvas/TilePanel").GetComponent<TileInfoToUI>();
		UIRightClick = GameObject.Find("Canvas/RightClickPanel");
        
    }

    private IEnumerator initializeGrid()
    {
        yield return new WaitWhile(() => TileManager.mapTiles == null);
        TileManager.addTile(this, (int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        if (mouseOverTest)
        {
            /*
            int x = (int)gameObject.transform.position.x;
            int y = (int)gameObject.transform.position.y;
            print("at" + x + ", " + y + " with weight" + movementWeight);
            print("right: " + right);
            print("up: " + up);
            print("down: " + down);
            print("left: " + left);
            mouseOverTest = false;
            */
        }

        //This section for highlighting hovered Tile, on entry
        Color hovered = GetComponent<SpriteRenderer>().color;
        hovered.b = 0f;
        GetComponent<SpriteRenderer>().color = hovered;

        //since onMousedown is only looking at left click
        if (Input.GetMouseButtonDown(1))
        {
            OnMouseDown();
            //UIRightClick.transform.position = Input.mousePosition;
        }
    }
    void OnMouseDown()
    {
        
        //Send this tile to the UI
        print("Got Here");
        UIInfo.RecentTile(this);

        //if it was a right-click, change the location of the right-click menu. could probably just change the draw layer/visibility later on
        //probably will need to change all this to be raycasting later on
        if (Input.GetMouseButtonDown(1))
        {
			//this is oh so messy, figure out a better way to get valid coordinates
			print("MouseX: " + Input.mousePosition.x + " MouseY: " + Input.mousePosition.y + "Screen Width: " + Screen.width + " Screen hiehgt: " + Screen.height + " New X: " + (Input.mousePosition.x-Screen.width/2) + " New Y: " + (Input.mousePosition.y-Screen.height/2));
			Vector3 temp = new Vector3((Input.mousePosition.x/Screen.width)*UICanvas.GetComponent<RectTransform>().rect.width - UICanvas.GetComponent<RectTransform>().rect.width/2 + 35, (Input.mousePosition.y/Screen.height)*UICanvas.GetComponent<RectTransform>().rect.height - UICanvas.GetComponent<RectTransform>().rect.height/2 - 80, 0);
			UIRightClick.transform.localPosition = temp;
        }
    }

    private void OnMouseExit()
    {
        //set it back to normal
        Color selected = GetComponent<SpriteRenderer>().color;
        selected.b = 255f;
        GetComponent<SpriteRenderer>().color = selected;
    }
    /**
     * Determines the movement weight based on the tile type
     */
    private int determineMovementWeight(string type)
    {
        switch (type)
        {
            case "Building":
                return int.MaxValue;
            case "Road":
                return 1;
            default:
                return 1;
        }

    }

    public bool hasUnit(bool value)
    {
        return currentUnit != null;
    }

    public UnitClass getCurrentUnit()
    {
        return currentUnit;
    }

    public void setRight(MapTile tile)
    {
        right = tile;
    }

    public MapTile getRight()
    {
        return right;
    }

    public void setUp(MapTile tile)
    {
        up = tile;
    }

    public MapTile getUp()
    {
        return up;
    }

    public void setDown(MapTile tile)
    {
        down = tile;
    }

    public MapTile getDown()
    {
        return down;
    }

    public void setLeft(MapTile tile)
    {
        left = tile;
    }

    public MapTile getLeft()
    {
        return left;
    }

    public int getMovementWeight()
    {
        return movementWeight;
    }

}

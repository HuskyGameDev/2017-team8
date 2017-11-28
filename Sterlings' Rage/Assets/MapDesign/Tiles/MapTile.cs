using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{

    private MapTile up;
    private MapTile down;
    private MapTile right;
    private MapTile left;
    private bool possibleMove;
    public string tileType;
    private int movementWeight;
    public Contraband contraband;
    public UnitClass currentUnit;
    private Color originalColor;
    public MapTile previousTile;
    private ArrayList movementPath;
    private bool firstHighlight;
    private int storedSpeed;
    private int xPosition;
    private int yPosition;
    

    // Use this for initialization
    void Start()
    {
        xPosition = (int)gameObject.transform.position.x;
        yPosition = (int)gameObject.transform.position.y;
        StartCoroutine(initializeGrid());
        movementWeight = determineMovementWeight(tileType);
        movementPath = new ArrayList();
        firstHighlight = true;
        originalColor = GetComponent<SpriteRenderer>().color;
        //For sending info to the UI, needs to be improved
        //For sending info to the UI, needs to be improved
    }

    /**
     * This waits for the array to be initialized before adding the tile to it
     */
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

        if (previousTile != null)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
            
            highlightRoute(false);
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
        print("Got Here " + xPosition + ","+ yPosition);
        if(currentUnit != null)
        {
            currentUnit.displayMovementPath();
        } else if (possibleMove)
        {
            TileManager.getSelectedUnit().MoveTo(movementPath,this);
            currentUnit = TileManager.getSelectedUnit();
            TileManager.resetTiles();
        } else if (tileType != "Building")
        {
            // Will change later to spawn different types but for now just spawing a melee unit
            spawnUnit("MeleeUnit");
        }
        //UIInfo.RecentTile(this);

        //if it was a right-click, change the location of the right-click menu. could probably just change the draw layer/visibility later on
        //probably will need to change all this to be raycasting later on
        if (Input.GetMouseButtonDown(1))
        {
			//this is oh so messy, figure out a better way to get valid coordinates
			print("MouseX: " + Input.mousePosition.x + " MouseY: " + Input.mousePosition.y + "Screen Width: " + Screen.width + " Screen hiehgt: " + Screen.height + " New X: " + (Input.mousePosition.x-Screen.width/2) + " New Y: " + (Input.mousePosition.y-Screen.height/2));
			//Vector3 temp = new Vector3((Input.mousePosition.x/Screen.width)*UICanvas.GetComponent<RectTransform>().rect.width - UICanvas.GetComponent<RectTransform>().rect.width/2 + 35, (Input.mousePosition.y/Screen.height)*UICanvas.GetComponent<RectTransform>().rect.height - UICanvas.GetComponent<RectTransform>().rect.height/2 - 80, 0);
			//UIRightClick.transform.localPosition = temp;
        }
    }

    private void OnMouseExit()
    {
        if (previousTile != null)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            highlightRoute(true);
        }
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

    private void spawnUnit(string unitType)
    {
        UnitClass unit;
        switch (unitType)
        {
            case "MeleeUnit":
                unit = new MeleeUnit();
                break;
            case "RangedUnit":
                unit = new RangedUnit();
                break;
            case "AntiInfantry":
                unit = new AntiInfClass();
                break;
            default :
                print("This shouldn't happen...");
                unit = new UnitClass();
                break;
                
        }
        if (ResourceManager.contraband >= unit.Cost)
        {
            print(unit.unitClassName);
            GameObject newUnit = (GameObject)Instantiate(Resources.Load(unit.unitClassName), new Vector2(transform.position.x, transform.position.y), transform.rotation);
            currentUnit = unit;
            unit.CurrentTile = this;
            ResourceManager.Subtract(unit.cost);
            print("Now have contraband: " + ResourceManager.contraband +" and spawned: " +currentUnit.unitClassName);
        } else
            print("Not enough contraband!");
    }

    private void highlightRoute(bool clear)
    {
        MapTile temp = previousTile;
        while (temp != null && TileManager.getSelectedUnit() != temp.currentUnit)
        {
            if (clear)
            {
                temp.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                temp.GetComponent<SpriteRenderer>().color = Color.blue;
                if (firstHighlight && !movementPath.Contains(temp))
                {
                    movementPath.Add(temp);
                }
            }
            temp = temp.previousTile;
        }
        if (!clear)
            firstHighlight = false;
    }

    /** 
     * resets to default information, needs to be here so it can be called by the tile manager
     */
    public void resetTile()
    {
        firstHighlight = true;
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
        movementPath.Clear();
        setPossibleMove(false);
        setStoredSpeed(-1);
        setPreviousTile(null);
    }

    public bool isPossibleMove()
    {
        return possibleMove;
    }

    public void setPossibleMove(bool val)
    {
        if (val)
        {
            possibleMove = val;
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        } else
        {
            possibleMove = val;
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;
        }
    }

    public void setStoredSpeed(int speed)
    {
        storedSpeed = speed;
    }

    public int getStoredSpeed()
    {
        return storedSpeed;
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

    public void setPreviousTile(MapTile tile)
    {
        previousTile = tile;
    }

    public MapTile getPreviousTile()
    {
        return previousTile;
    }

    public int getXPosition()
    {
        return xPosition;
    }

    public int getYPosition()
    {
        return yPosition;
    }

    public bool Equals(MapTile tile)
    {
        if (tile != null && xPosition == tile.getXPosition() && yPosition == tile.getYPosition())
            return true;
        return false;
    }

}

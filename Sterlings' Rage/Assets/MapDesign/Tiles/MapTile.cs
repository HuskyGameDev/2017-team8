using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    private static Color possibleMovementColor = Color.cyan;
    private static Color highlightedMovementRouteColor = Color.blue;
    private static Color attackRangeHighlightColor = Color.red;

    // Used for spawning new Units
    private static MeleeUnit meleeUnit = new MeleeUnit();
    private static AntiInfClass antiInf = new AntiInfClass();
    private static RangedUnit rangedUnit = new RangedUnit();

    public MapTile up;
    public MapTile down;
    public MapTile right;
    public MapTile left;
    private bool possibleMove;
    private int storedSpeed;
    public bool possibleAttack;
    private int storedRange;
    public string tileType;
    private int movementWeight;
    public Contraband contraband;
    public UnitClass currentUnit;
    private Color originalColor;
    public MapTile previousTile;
    private ArrayList movementPath;
    private bool firstHighlight;
    private int xPosition;
    private int yPosition;
    private TileInfoToUI UIInfo;
    private GameObject UIRightClick;
    private Canvas UICanvas;


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
        UIInfo = GameObject.Find("TilePanel").GetComponent<TileInfoToUI>();
        UIRightClick = GameObject.Find("RightClickPanel");
        UICanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    void attack(UnitClass Attacker, UnitClass Defender, MapTile AtckTile, MapTile DefTile)
    {
       /*print("Attacker is " + Attacker);
        print("Defender is " + Defender);
        print("Attacker located at " + AtckTile);
        print("Defender located at " + DefTile);*/
        float aX = AtckTile.xPosition; float aY = AtckTile.yPosition;
        float dX = DefTile.xPosition; float dY = DefTile.yPosition;
        float difX = Mathf.Pow(aX - dX, 2); float difY = Mathf.Pow(aY - dY, 2);
        float dist = Mathf.Sqrt(difX + difY);
        //print("Distance is " + dist);
        if( Attacker.UnitClassName == "RangedUnit" && Defender.UnitType == "Infantry"){
            Defender.Health -= 7;
        }
        else{
            Defender.Health -= Attacker.Damage;

            if(Defender.Health != 0){
                if (dist <= Defender.range)
                {
                    Attacker.Health -= Defender.Damage;
                }
                else
                {
                    print("Out of range!");
                }
            } 
        }
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
            if (currentUnit == null)
                GetComponent<SpriteRenderer>().color = highlightedMovementRouteColor;

            highlightRoute(false);
        }
        else if (!possibleAttack && !GameObject.ReferenceEquals(UIInfo.holding, this))
        {
            GetComponent<SpriteRenderer>().color = Color.grey;
        }
        //since onMousedown is only looking at left click
        //Leave this in here, just needs to be commented out for Thursday
       /* if (Input.GetMouseButtonDown(1))
        {
            print("MouseX: " + Input.mousePosition.x + " MouseY: " + Input.mousePosition.y + "Screen Width: " + Screen.width + " Screen hiehgt: " + Screen.height + " New X: " + (Input.mousePosition.x - Screen.width / 2) + " New Y: " + (Input.mousePosition.y - Screen.height / 2));
            Vector3 temp = new Vector3((Input.mousePosition.x / Screen.width) * UICanvas.GetComponent<RectTransform>().rect.width - UICanvas.GetComponent<RectTransform>().rect.width / 2 + 35, (Input.mousePosition.y / Screen.height) * UICanvas.GetComponent<RectTransform>().rect.height - UICanvas.GetComponent<RectTransform>().rect.height / 2 - 80, 0);
            UIRightClick.transform.localPosition = temp;
        }*/
    }
    void OnMouseDown()
    {

        
        if (currentUnit != null && currentUnit.gameObject.tag != "Enemy")
        {
            TileManager.resetAllTiles();
            print("this should do things");
            currentUnit.displayMovementPath();
        }
        else if (possibleMove && currentUnit == null)
        {
            UnitManager.getSelectedUnit().MoveTo(movementPath, this);
            currentUnit = UnitManager.getSelectedUnit();
            TileManager.resetAllTiles();
        }
        else if (possibleAttack && currentUnit != null && currentUnit != UnitManager.getSelectedUnit() && currentUnit.gameObject.tag == "Enemy")
        {
            print("Attacking!");
            attack(UnitManager.getSelectedUnit(), currentUnit, UnitManager.getSelectedUnit().currentTile, currentUnit.currentTile);
            UnitManager.getSelectedUnit().alreadyAttacked = true;
            TileManager.resetAllTiles();
        }
        else if (tileType != "Building" && currentUnit == null && !possibleAttack)
        {
            // Will change later to spawn different types but for now just spawing a specific unit
            spawnUnit("AntiInfantry");
            TileManager.resetAllTiles();
        }
        if (currentUnit == null && UnitManager.getSelectedUnit() != null)
        {
            TileManager.resetAllTiles();
        }

        UIInfo.RecentTile(this);

        GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void OnMouseExit()
    {
        if (previousTile != null)
        {
            if (!possibleAttack)
            {
                if (currentUnit == null)
                    GetComponent<SpriteRenderer>().color = possibleMovementColor;
                else
                    GetComponent<SpriteRenderer>().color = originalColor;
            }
            else
                GetComponent<SpriteRenderer>().color = attackRangeHighlightColor;
            highlightRoute(true);
        }
        else if(!possibleAttack && !GameObject.ReferenceEquals(UIInfo.holding, this))
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
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
                unit = meleeUnit;
                break;
            case "RangedUnit":
                unit = rangedUnit;
                break;
            case "AntiInfantry":
                unit = antiInf;
                break;
            default:
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
            print("Now have contraband: " + ResourceManager.contraband + " and spawned: " + currentUnit.unitClassName);
        }
        else
            print("Not enough contraband! have :" + ResourceManager.contraband);
    }

    /**
     * Highlights the current route the unit would take to reach a tile.
     * @param clear Indicates to clear the route, set the color back to what it was before, because the mouse is
     *              no longer hovering over the tile
     */              
    public ArrayList highlightRoute(bool clear)
    {
        MapTile temp = previousTile;
        while (temp != null && UnitManager.getSelectedUnit() != temp.currentUnit)
        {
            if (clear)
            {
                if (temp.isPossibleAttack())
                {
                    temp.GetComponent<SpriteRenderer>().color = attackRangeHighlightColor;
                }
                else if(temp.currentUnit == null)
                {
                    temp.GetComponent<SpriteRenderer>().color = possibleMovementColor;
                }
                else
                {
                    temp.GetComponent<SpriteRenderer>().color = originalColor;
                }
            }
            else
            {
              //  if (temp.currentUnit == null)
             //   {
                    temp.GetComponent<SpriteRenderer>().color = highlightedMovementRouteColor;
               // }
                // Only adds the tile to the list if it was the first time being highlighted and it isn't in the list already
                if (firstHighlight && !movementPath.Contains(temp))
                {
                    movementPath.Add(temp);
                }
            }
            temp = temp.previousTile;
        }
        if (!clear)
            firstHighlight = false;
        return movementPath;
    }

    /** 
     * resets to default information, needs to be public so it can be called by the tile manager
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

    public void resetAttackTile()
    {
        possibleAttack = false;
        storedRange = -1;
        if (possibleMove)
            gameObject.GetComponent<SpriteRenderer>().color = possibleMovementColor;
        else
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }

    public bool isPossibleAttack()
    {
        return possibleAttack;
    }

    public void setPossibleAttack(bool val)
    {
        possibleAttack = val;
        if (val)
            gameObject.GetComponent<SpriteRenderer>().color = attackRangeHighlightColor;
    }

    public bool isPossibleMove()
    {
        return possibleMove;
    }

    public void setPossibleMove(bool val)
    {
        possibleMove = val;
        if (val)
        {
            if(currentUnit == null)
                gameObject.GetComponent<SpriteRenderer>().color = possibleMovementColor;
        }
    }

    public void setStoredRange(int range)
    {
        storedRange = range;
    }

    public int getStoredRange()
    {
        return storedRange;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapTile : MonoBehaviour
{
    private static Color possibleMovementColor = Color.cyan;
    private static Color highlightedMovementRouteColor = Color.blue;
    private static Color attackRangeHighlightColor = Color.red;

    // Used for spawning new Units
    private static MeleeUnit meleeUnit = new MeleeUnit();
    private static AntiInfClass antiInf = new AntiInfClass();
    private static RangedUnit rangedUnit = new RangedUnit();

    public TileManager tileManager;
    private TurnManager turnManager;
    private UnitManager unitManager;
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
    public int xPosition;
    public int yPosition;
    private TileInfoToUI UIInfo;
    private GameObject UIRightClick;
    private Canvas UICanvas;
    private bool inAttackRange = false;
    private bool inPossibleAttackRange = false;
    ArrayList inAttackRangeUnits;
    ArrayList inPossibleAttackRangeUnits;

    public int visible;

    //this is only used for tagging when using AoE based checks.
    public int flag;

    // Use this for initialization
    void Start()
    {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
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

        inPossibleAttackRangeUnits = new ArrayList();
        inAttackRangeUnits = new ArrayList();

        this.GetComponent<SpriteRenderer>().color = Color.grey;
        flag = 0;
        visible = 0;

    }
    public void AOE(float xpos, float ypos, UnitClass Attacker, int damage){
        GameObject[] PlayerUnits;
        GameObject[] EnemyUnits;
        GameObject Unit = null;
        Vector3 position = new Vector3(xpos,ypos,0);
        EnemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
        PlayerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        //print(PlayerUnits.Length + " objects in PlayerUnits");
        //print(EnemyUnits.Length + " objects in EnemyUnits");
        //print( "! "  + Attacker.gameObject.tag + " !");
        if(Attacker.gameObject.tag == "Enemy")
        {
            foreach(GameObject found in PlayerUnits){
                if(found.transform.position == position){
                    Unit = found;
                    if(Unit.GetComponent<AntiInfClass>() != null){
                        print("Found Anti-Infantry");
                        Unit.GetComponent<AntiInfClass>().Health -= damage;
                    }
                    else if(Unit.GetComponent<MeleeUnit>() != null){
                        print("Found Melee");
                        Unit.GetComponent<MeleeUnit>().Health -= damage;
                    }
                    else if(Unit.GetComponent<FlameUnit>() != null){
                        print("Found Flame");
                        Unit.GetComponent<FlameUnit>().Health -= damage;
                    }
                }
            }
        }
        else{
            foreach(GameObject found in EnemyUnits){
                if(found.transform.position == position){
                    print("Position is: " + position);
                    Unit = found;
                    if(Unit.GetComponent<AntiInfClass>() != null){
                        print("Found Anti-Infantry");
                        Unit.GetComponent<AntiInfClass>().Health -= damage;
                    }
                    else if(Unit.GetComponent<MeleeUnit>() != null){
                        print("Found Melee");
                        Unit.GetComponent<MeleeUnit>().Health -= damage;
                    }
                    else if(Unit.GetComponent<FlameUnit>() != null){
                        print("Found Flame");
                        Unit.GetComponent<FlameUnit>().Health -= damage;
                    }
                }
            }
        }
    }
    // Calculates damage for AI
    public int calculateAttackDamage(UnitClass Attacker, UnitClass Defender)
    {
        float aX = Attacker.currentTile.xPosition; float aY = Attacker.currentTile.yPosition;
        float dX = Defender.currentTile.xPosition; float dY = Defender.currentTile.yPosition;
        float difX = Mathf.Pow(aX - dX, 2); float difY = Mathf.Pow(aY - dY, 2);
        float dist = Mathf.Sqrt(difX + difY);
        //print("Distance is " + dist);
        //AOE(dX,dY,Attacker, 3);
        if (Attacker.UnitClassName == "FlameUnit")
        {
            Defender.Health -= Attacker.Damage;
            AOE(dX + 1, dY, Attacker, Attacker.Damage);
            AOE(dX + 1, dY + 1, Attacker, Attacker.Damage);
            AOE(dX + 1, dY - 1, Attacker, Attacker.Damage);
        }
        if (Attacker.UnitClassName == "GrenadierUnit")
        {
            Defender.Health -= Attacker.Damage;
            AOE(dX + 1, dY, Attacker, Attacker.Damage);
            AOE(dX - 1, dY, Attacker, Attacker.Damage);
            AOE(dX, dY + 1, Attacker, Attacker.Damage);
            AOE(dX, dY - 1, Attacker, Attacker.Damage);
        }
        if (Attacker.UnitClassName == "Anti-Infantry" && Defender.UnitType == "Infantry")
        {
            return -7;
        }
        else
        {
            return  -Attacker.Damage;

            if (Defender.Health <= 0)
            {
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
    // Calculates damage for Player Units
    public void attack(UnitClass Attacker, UnitClass Defender, MapTile AtckTile, MapTile DefTile)
    {
        float aX = AtckTile.xPosition; float aY = AtckTile.yPosition;
        float dX = DefTile.xPosition; float dY = DefTile.yPosition;
        float difX = Mathf.Pow(aX - dX, 2); float difY = Mathf.Pow(aY - dY, 2);
        float dist = Mathf.Sqrt(difX + difY);
        print("DefTile : " + dX + "," + dY);
        //print("Distance is " + dist);
        //AOE(dX,dY,Attacker, 3);
        if (Attacker.UnitClassName == "FlameUnit")
        {
            Defender.Health -= Attacker.Damage;
            AOE(dX + 1, dY, Attacker, Attacker.Damage);
            AOE(dX + 1, dY + 1, Attacker, Attacker.Damage);
            AOE(dX + 1, dY - 1, Attacker, Attacker.Damage);
        }
        if (Attacker.UnitClassName == "GrenadierUnit")
        {
            //Defender.Health -= Attacker.Damage;
            AOE(dX + 1, dY, Attacker, Attacker.Damage);
            AOE(dX - 1, dY, Attacker, Attacker.Damage);
            AOE(dX, dY + 1, Attacker, Attacker.Damage);
            AOE(dX, dY - 1, Attacker, Attacker.Damage);
        }
        if (Attacker.UnitClassName == "Anti-Infantry" && Defender.UnitType == "Infantry")
        {
            Defender.Health -= 7;
        }
        else
        {
            Defender.Health -= Attacker.Damage;
            Defender.damageCount = 15;

            if (Defender.Health <= 0)
            {
                if (dist <= Defender.range)
                {
                    Attacker.Health -= Defender.Damage;
                    Attacker.damageCount = 15;
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
        yield return new WaitWhile(() => tileManager == null || tileManager.instantiated == false);
        tileManager.addTile(this, (int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (UIInfo == null)
        {
            GameObject g = GameObject.Find("TilePanel");
            UIInfo = g.GetComponent<TileInfoToUI>();
            //print("did we find it? " + UIInfo);
        }
        if (unitManager == null || tileManager == null || turnManager == null)
        {
            
            unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
            tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
            turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        }
        
    }

    private void OnMouseOver()
    {
        if (UIInfo == null)
            print("I'm confused...");
        if (!EventSystem.current.IsPointerOverGameObject())
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
             if (Input.GetMouseButtonDown(1))
             {
                 //print("MouseX: " + Input.mousePosition.x + " MouseY: " + Input.mousePosition.y + "Screen Width: " + Screen.width + " Screen hiehgt: " + Screen.height + " New X: " + (Input.mousePosition.x - Screen.width / 2) + " New Y: " + (Input.mousePosition.y - Screen.height / 2));
                 Vector3 temp = new Vector3((Input.mousePosition.x / Screen.width) * UICanvas.GetComponent<RectTransform>().rect.width - UICanvas.GetComponent<RectTransform>().rect.width / 2 + 35, (Input.mousePosition.y / Screen.height) * UICanvas.GetComponent<RectTransform>().rect.height - UICanvas.GetComponent<RectTransform>().rect.height / 2 - 80, 0);
                 UIRightClick.transform.localPosition = temp;
                 UIRightClick.GetComponent<DefaultOffScreen>().RightClicked = this;
             }
        }
    }
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (turnManager.playerTurn)
            {   
                // Player Units can't move until you end your turn
                // Can't do any other unit actions after using Medic
                if (currentUnit != null && currentUnit.gameObject.tag != "Enemy")
                {

                    tileManager.resetAllTiles();
                    currentUnit.displayMovementPath();
                    /*if (unitManager.getSelectedUnit().UnitClassName == "MedicUnit")
                    {
                        attack(unitManager.getSelectedUnit(), currentUnit, unitManager.getSelectedUnit().currentTile, currentUnit.currentTile);
                        tileManager.resetAllTiles();
                        currentUnit.displayMovementPath();
                    }
                    else if()
                    {
                        tileManager.resetAllTiles();
                        currentUnit.displayMovementPath();
                    }*/ 
                    

                }
                else if (possibleMove && currentUnit == null)
                {
                    unitManager.getSelectedUnit().MoveTo(movementPath, this);
                    currentUnit = unitManager.getSelectedUnit();
                    tileManager.resetAllTiles();
                }
                else if (possibleAttack && currentUnit != null && currentUnit != unitManager.getSelectedUnit() && currentUnit.gameObject.tag == "Enemy")
                {
                    print("Current: " + currentUnit);
                    print("Selected: " + unitManager.getSelectedUnit());
                    print("Attacking!");
                    attack(unitManager.getSelectedUnit(), currentUnit, unitManager.getSelectedUnit().currentTile, currentUnit.currentTile);
                    unitManager.getSelectedUnit().alreadyAttacked = true;
                    tileManager.resetAllTiles();
                    //play attacking animation
                    unitManager.getSelectedUnit().gameObject.GetComponent<Animator>().Play("Attack");
                }
                
                else if (tileType != "Building" && currentUnit == null && !possibleAttack)
                {
                    // Will change later to spawn different types but for now just spawing a specific unit
                    spawnUnit("AntiInfantry");
                    tileManager.resetAllTiles();
                }
            }
            if (currentUnit == null && unitManager.getSelectedUnit() != null)
            {
                tileManager.resetAllTiles();
            }

            UIInfo.RecentTile(this);

            // Test code for the BresenHam Algorithm (A range of 5)
            for (int rangeX = -5; rangeX <= 5; rangeX++)
            {
                for (int rangeY = -5; rangeY <= 5; rangeY++)
                {
                    if (Mathf.Abs(rangeX) + Mathf.Abs(rangeY) <= 5)
                    {
                        print("X: " + (xPosition + rangeX) + " Y: " + (yPosition + rangeY));
                        Bern(xPosition + rangeX, yPosition + rangeY, new ArrayList());
                    }
                }
            }

            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    /* The Bern method is the initializer to the BresenHam function
     * 
     * It first creates the line from the MapTile it originates from, and from a given point.
     * Then it figures out the Octant, manipulates the err (slope) and calls the BrensenHam function
     * 
     * x : the target coordinates x value
     * y : the target coordinates y value
     * 
     * Returns an list of tiles that the AI will use to determine its next move
     * */
    public ArrayList Bern(int x, int y, ArrayList spottedTiles)
    {
        //print("This is getting called");
        // Test code for the Bernsenham Algorithm
        //Get the values needed (hardcoded to target coordinates of (40,40) for initial testing)
        double deltaX = x - xPosition;
        double deltaY = y - yPosition;
        double err = (deltaY / deltaX);
        int octant = 0;
        // Get the Ocatants
        if (deltaX >= 0)
        {//Right
            if (deltaY < 0)
            {//Down
                if (deltaX == 0)
                {//Straight Down
                    octant = 77;
                    err = 0;
                }
                else
                {//Octant 7 or 8
                    //If the error, or slope, is less than or equal to -1, then it is in Octant 7 (Don't forget that less than -1 is a larger absolute value)
                    if (err <= -1)
                    {
                        //Octant 7
                        //Y axis will decrement by one every time, X axis will increment based on err (run over rise)
                        octant = 7;
                        err = Mathf.Abs((float)(deltaX / deltaY));
                    }
                    else if (err > -1)
                    {
                        //Octant 8
                        //X axis increments each time, with Y decrementing based on err (slope)
                        octant = 8;
                        err = Mathf.Abs((float)err);
                    }
                }
            }
            else
            {//Up or Horizontal (don't have to worry about horizontal, as you can divide 0 by numbers)
                if (deltaX == 0)
                {//Straight Up
                    octant = 33;
                    err = 0;
                }
                else
                {//Octant 1 or 2

                    //If the error, or slope, is less than or equal to 1, then it is in Octant 1
                    if (err <= 1)
                    {
                        //Octant 1
                        //X axis will increment by one every time, Y axis will increment based on err (slope)
                        octant = 1;
                        //err is currently correct, no change needed
                    }
                    else if (err > -1)
                    {
                        //Octant 2
                        //Y axis increments each time, with X incrementing based on err (run over rise)
                        octant = 2;
                        err = (deltaX / deltaY);
                    }
                }
            }
        }
        else if (deltaX < 0)
        {//Left
            if (deltaY <= 0)
            {//Down or Horizontal
             //Octant 5 or 6
             //Negatives cancel, check math in incrementations (using a Vector after all)
             //If the error, or slope, is less than or equal to 1, then it is in Octant 5
                if (err <= 1)
                {
                    //Octant 5
                    //X axis will decrement by one every time, Y axis will decrement based on err (slope)
                    octant = 5;
                    //Should have the same err as Octant 1
                }
                else if (err > 1)
                {
                    //Octant 6
                    //Y axis decrements each time, with X decrementing based on err (run over rise)
                    octant = 6;
                    err = (deltaX / deltaY);
                }
            }
            else
            {//Up
             //Octant 3 or 4

                //If the error, or slope, is less than or equal to -1, then it is in Octant 3
                if (err <= -1)
                {
                    //Octant 3
                    //Y axis will increment by one every time, X axis will decrement based on err (run over rise)
                    octant = 3;
                    err = Mathf.Abs((float)(deltaX / deltaY));
                }
                else if (err > -1)
                {
                    //Octant 4
                    //X axis decrements each time, with Y incrementing based on err (slope)
                    octant = 4;
                    err = Mathf.Abs((float)err);
                }
            }
        }
        return BresenHam(x, y, err, 0f, octant, spottedTiles);
    }

    /*This funciton tracks through the needed MapTiles based on the Octant given.
     * 
     * x2 : Target x value
     * y2 : Target y value
     * err : The error (slope/(run/rise)) associated with an integer coordinate system
     * errCount: The cumulative error that causes the non-dominant direction to increment/decrement
     * octant: The 1/8 of a circle (octant 1 starting at 0 deg) you're traversing
     * objectiveTiles: will be used by the AI to determine if anything specail can be seen such as contraband, player units, etc.
     * */
    public ArrayList BresenHam(int x2, int y2, double err, double errCount, int octant, ArrayList objectiveTiles)
    {
        //Debug
        //print("Error: " + err + "  ErrCount: " + errCount + "  Octant: " + octant);
        //Check for walls
        if(this.tileType == "Building")
        {
            return objectiveTiles;
        }
        //color the square
        //GetComponent<SpriteRenderer>().color = Color.red;
        if (currentUnit != null && currentUnit.playerUnit)
        {
            objectiveTiles.Add(new KeyValuePair<MapTile, object>(this, currentUnit));
           // print("Found unit");
        }
        else if (contraband != null)
            objectiveTiles.Add(new KeyValuePair<MapTile, object>(this, contraband));
        //See if finished, otherwise increment
        if (xPosition == x2 && yPosition == y2)
        {
            return objectiveTiles;
        }
        else
        {
            errCount += err;
            if (errCount > 0.5f)
            {
                errCount--;
                switch (octant)
                {
                    case 1:
                        //print("Got here too");
                        if (right != null && right.up != null)
                            right.up.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 2:
                        if (up != null && up.right != null)
                            up.right.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 3:
                        if (up != null && up.left != null)
                            up.left.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 4:
                        if (left != null && left.up != null)
                            left.up.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 5:
                        if (left != null && left.down != null)
                            left.down.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 6:
                        if (down != null && down.left != null)
                            down.left.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 7:
                        if (down != null && down.right != null)
                            down.right.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 8:
                        if (right != null && right.down != null)
                            right.down.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 33:
                        break;
                    case 77:
                        break;
                }
            }
            else
            {
                switch (octant)
                {
                    case 1:
                        if (right != null)
                            right.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 2:
                        if (up != null)
                            up.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 3:
                        if (up != null)
                            up.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 4:
                        if (left != null)
                            left.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 5:
                        if (left != null)
                            left.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 6:
                        if (down != null)
                            down.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 7:
                        if (down != null)
                            down.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 8:
                        if (right != null)
                            right.BresenHam(x2, y2, err, errCount, octant, objectiveTiles);
                        break;
                    case 33:
                        if (up != null)
                            up.BresenHam(x2, y2, 0, 0, octant, objectiveTiles);
                        break;
                    case 77:
                        if (down != null)
                            down.BresenHam(x2, y2, 0, 0, octant, objectiveTiles);
                        break;
                }
            }
        }
        return objectiveTiles;
    }

    private void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (previousTile != null)
            {
                if (!possibleAttack)
                {
                    if (currentUnit == null)
                        GetComponent<SpriteRenderer>().color = possibleMovementColor;
                    else
                    {
                        if(visible == 1)
                        {
                            GetComponent<SpriteRenderer>().color = Color.white;
                        }
                        else
                        {
                            GetComponent<SpriteRenderer>().color = Color.grey;
                        }
                    }
                }
                else
                    GetComponent<SpriteRenderer>().color = attackRangeHighlightColor;
                highlightRoute(true);
            }
            else if (!possibleAttack && !GameObject.ReferenceEquals(UIInfo.holding, this))
            {
                if (visible == 1)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = Color.grey;
                }
            }
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
            GameObject newUnit = (GameObject)Instantiate(Resources.Load(unit.unitClassName), new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0f, 0f, 0f));
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
        while (temp != null && unitManager.getSelectedUnit() != temp.currentUnit)
        {
            if (clear)
            {
                if (temp.isPossibleAttack())
                {
                    temp.GetComponent<SpriteRenderer>().color = attackRangeHighlightColor;
                }
                else if (temp.currentUnit == null)
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
            if (currentUnit == null)
                gameObject.GetComponent<SpriteRenderer>().color = possibleMovementColor;
        }
    }

    public void resetDangerousTile()
    {
        setInAttackRange(false, null);
        setInPossibleAttackRange(false, null);
    }

    public List<MapTile> getAdjacentTiles()
    {
        List<MapTile> tiles = new List<MapTile>();
        if (up != null)
        {
            tiles.Add(up);
        }
        if (right != null)
        {
            tiles.Add(right);
        }
        if (down != null)
        {
            tiles.Add(down);
        }
        if (left != null)
        {
            tiles.Add(left);
        }

        return tiles;
    }

    public override bool Equals(object other)
    {
        if (other == null || other.GetType() != typeof(MapTile))
            return false;
        MapTile tile = (MapTile)other;
        return xPosition == tile.getXPosition() && yPosition == tile.getYPosition();
    }

    public override string ToString()
    {
        return xPosition + "," + yPosition;
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

    public bool isInAttackRange()
    {
        return inAttackRange;
    }

    public bool isInPossibleAttackRange()
    {
        return inPossibleAttackRange;
    }

    /**
     * Indicates that the tile is in the immedeiate attack range of a unit and stores which unit can attack
     * 
     * unit is the unit that can attack, if resetting to false just pass in null
     **/
    public void setInAttackRange(bool value, UnitClass unit)
    {
        if (value)
        {
            if (!inAttackRange)
                inAttackRangeUnits = new ArrayList();
            if (!inAttackRangeUnits.Contains(unit))
                inAttackRangeUnits.Add(unit);
        } else
        {
            inAttackRangeUnits.Clear();
        }
        inAttackRange = value;
    }

    /**
     * Indicates that the tile is attack range of a unit after it moves and stores which unit can attack
     * 
     * unit is the unit that can attack, if resetting to false just pass in null
     **/
    public void setInPossibleAttackRange(bool value, UnitClass unit)
    {
        if (value)
        {
            if (!inAttackRange)
                inPossibleAttackRangeUnits = new ArrayList();
            if (!inPossibleAttackRangeUnits.Contains(unit))
                inPossibleAttackRangeUnits.Add(unit);
        }
        else
        {
            inPossibleAttackRangeUnits.Clear();
        }
        inPossibleAttackRange = value;
    }

    public ArrayList getInAttackRangeUnits ()
    {
        return inAttackRangeUnits;
    }

    public ArrayList getInPossibleAttackRangeUnits()
    {
        return inPossibleAttackRangeUnits;
    }

    public bool Equals(MapTile tile)
    {
        if (tile != null && xPosition == tile.getXPosition() && yPosition == tile.getYPosition())
            return true;
        return false;
    }

}

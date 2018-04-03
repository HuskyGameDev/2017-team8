using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public MapTile[,] mapTiles;
    public bool instantiated = false;
    public int mapWidthX;
	public int mapHeightY;
    public int width;
	public int height;
    private TurnManager turnManager;
    private UnitManager unitManager;
    private bool full = false;
    private ArrayList pathList;
    private ArrayList attackList;

    // Use this for initialization
    void Start()
    {
        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        // Allows us to set the size from within the editor and then storing in the static variable allows 
        // the size to be used in the static methods
        width = mapWidthX;
		height = mapHeightY;
		mapTiles = new MapTile[width, height];
        instantiated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (unitManager == null || turnManager == null)
        {
            unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
            turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        }
        if (Input.GetKey(KeyCode.B))
        {
            print("full? " + checkIfFull());
        }
        if (Input.GetKeyDown(KeyCode.A) && unitManager.getSelectedUnit() != null)
        {
            unitManager.getSelectedUnit().DisplayAttackRange();
        }
    }

    public void addTile(MapTile mapTile, int x, int y)
    {
        mapTiles[x, y] = mapTile;
        if (checkIfFull())
        {
            fillAdjacentTiles();
            turnManager.newTurn();
        }
    }

    public bool checkIfFull()
    {
        if (!full)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapTiles[x, y] == null)
                    {
                        return false;
                    }
                }
            }
        }
        full = true;
        return true;
    }

    private void fillAdjacentTiles()
    {
		for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
               // print(mapTiles[x, y]);
                if (x + 1 < width)
                {
                    mapTiles[x, y].setRight(mapTiles[x + 1, y]);
                }
				if (y + 1 < height)
                {
                    mapTiles[x, y].setUp(mapTiles[x, y + 1]);
                }
                if (x - 1 >= 0)
                {
                    mapTiles[x, y].setLeft(mapTiles[x - 1, y]);
                }
                if (y - 1 >= 0)
                {
                    mapTiles[x, y].setDown(mapTiles[x, y - 1]);
                }
            }
        }
    }

    public bool containsAttackTile(int range, MapTile tile)
    {
        if (tile.isPossibleAttack() && tile.getStoredRange() > range)
        {
            return true;
        }
        return false;
    }

    /**
     * Checks if the current tile has been a checked for a possible move or if it has it
     * checkes what the speed was when it was chekced
     */
    public bool containsTile(int speed, MapTile tile)
    {
        if (tile.isPossibleMove() && tile.getStoredSpeed() > speed)
        {
            return true;
        }
        return false;
    }

    public void resetMovementTiles()
    {
        if (pathList != null)
        {
            foreach (MapTile tile in pathList)
            {
                tile.resetTile();
            }
        }
        pathList = null;
    }

    public void resetAllTiles()
    {
        if (pathList != null)
        {
            foreach (MapTile tile in pathList)
            {
                tile.resetTile();
            }
        }
        if (attackList != null)
        {
            
             foreach (MapTile tile in attackList)
             {
                tile.resetAttackTile();
             }
        }
        pathList = null;
        //unitManager.setSelectedUnit(null);

        //reset all tiles to unseen
        foreach (MapTile Tile in mapTiles)
        {
            if (Tile.GetComponent<SpriteRenderer>().color == Color.white)
            {
                Tile.GetComponent<SpriteRenderer>().color = Color.grey;
                Tile.visible = 0;
            }
        }
        //call the Vision Bernsenham
        foreach (UnitClass unit in unitManager.PlayerUnits)
        {
            BernStart(unit.CurrentTile, 15, 1);
        }
        //integrate the superfluous vision reset from Gamestateupdate here
        GameObject.Find("SelectionIcon").transform.localPosition = new Vector3(-10, -10, 0);
    }

    public void setPathList(ArrayList path)
    {
        pathList = path;
    }

    public ArrayList getPathList()
    {
        return pathList;
    }

    public ArrayList getAttackList()
    {
        return attackList;
    }

    public void setAttackList(ArrayList list)
    {
        attackList = list;
    }

    /*
     * range indicates the max sum of x and y on travel
     * modifier indicates what should happen upon each tile being reached.
     * */
    public void BernStart(MapTile Tile, int range, int modifier)
    {
        for (int rangeX = -range; rangeX <= range; rangeX++)
        {
            for (int rangeY = -range; rangeY <= range; rangeY++)
            {
                //First do the most by checking the outside edges first, a flag will be placed on each 
                //visited tile. Another run will then go through all tiles again, but only run on
                //unflagged Tiles. After a single run, all Tiles will then be unflagged using a simple
                //radius. The used flag should never last after an operation, so setting all flags in a
                //radius shoudn't be a problem.
                if (Mathf.Abs(rangeX) + Mathf.Abs(rangeY) <= range)
                {
                    //need to make sure that no tile is chosen if the x,y coords are out of bounds
                    BernSetup(Tile, Tile.xPosition + rangeX, Tile.yPosition + rangeY, modifier);
                }
            }
        }
        /*for (int rangeX = -range; rangeX <= range; rangeX++)
        {
            for (int rangeY = -range; rangeY <= range; rangeY++)
            {
                //constrain this to only valid tiles
                if(Tile.xPosition + rangeX >= 0 && Tile.xPosition + rangeX < width && Tile.yPosition + rangeY >= 0 && Tile.yPosition + rangeY < height)
                {
                    if (Mathf.Abs(rangeX) + Mathf.Abs(rangeY) <= range && mapTiles[Tile.xPosition + rangeX, Tile.yPosition + rangeY].flag == 0)
                    {
                        //Now check every tile's flag, calling the BernSetup only on those that are unflagged.
                        BernSetup(Tile, Tile.xPosition + rangeX, Tile.yPosition + rangeY, modifier);
                    }
                }
                
            }
        }*/
        /*for (int rangeX = -range; rangeX <= range; rangeX++)
        {
            for (int rangeY = -range; rangeY <= range; rangeY++)
            {
                //Unflagg all tiles
                if (Mathf.Abs(rangeX) + Mathf.Abs(rangeY) <= range)
                {
                    Tile.flag = 0;
                }
            }
        }*/
    }
    /* The Bern method is the initializer to the BresenHam function
     * 
     * It first creates the line from the MapTile it originates from, and from a given point.
     * Then it figures out the Octant, manipulates the err (slope) and calls the BrensenHam function
     * 
     * x : the target coordinates x value
     * y : the target coordinates y value
     * 
     * */
    public void BernSetup(MapTile Tile, int x, int y, int modifier)
    {
        // Test code for the Bernsenham Algorithm
        //Get the values needed (hardcoded to target coordinates of (40,40) for initial testing)
        double deltaX = x - Tile.xPosition;
        double deltaY = y - Tile.yPosition;
        double err = (deltaY / deltaX);
        int octant = 0;
        // Get the Ocatants
        //print("Got Here");
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
        BresenHam(Tile, x, y, err, 0f, octant, modifier);
    }

    /*This funciton tracks through the needed MapTiles based on the Octant given.
     * 
     * x2 : Target x value
     * y2 : Target y value
     * err : The error (slope/(run/rise)) associated with an integer coordinate system
     * errCount: The cumulative error that causes the non-dominant direction to increment/decrement
     * octant: The 1/8 of a circle (octant 1 starting at 0 deg) you're traversing
     * */
    public void BresenHam(MapTile Tile, int x2, int y2, double err, double errCount, int octant, int modifier)
    {
        //Debug
        //print("Error: " + err + "  ErrCount: " + errCount + "  Octant: " + octant);
        //print("X: " + (Tile.xPosition) + " Y: " + (Tile.yPosition));
        
        
        
        //perform the modifier action
        if (modifier == 1)
        {//This is the standard Vision modifier, just going along, stopping at walls
            Tile.GetComponent<SpriteRenderer>().color = Color.white;
            //mark it as visible
            Tile.visible = 1;
            if (Tile.tileType == "Building")
            {
                //no more vision past this, but still want to mark all preceding as flags so they aren't re-checked
                modifier = 2;
            }
        }
        else if(modifier == 0)
        {
            Tile.GetComponent<SpriteRenderer>().color = Color.red;
        }else if(modifier == 2)
        {
            //case where it hits a wall, but everything still needs to be flagged as touched.
        }

        //flag the Tile as visited
        Tile.flag = 1;

        //See if finished, otherwise increment
        if (Tile.xPosition == x2 && Tile.yPosition == y2)
        {
            //print("X: " + (Tile.xPosition) + " Y: " + (Tile.yPosition));
            return;
        }
        else
        {
            errCount += err;
            if (errCount <= 0.5f)
            {
                switch (octant)
                {
                    case 1:
                        if (Tile.right == null) { return; }
                        BresenHam(Tile.right, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 2:
                        if (Tile.up == null) { return; }
                        BresenHam(Tile.up, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 3:
                        if (Tile.up == null) { return; }
                        BresenHam(Tile.up, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 4:
                        if (Tile.left == null) { return; }
                        BresenHam(Tile.left, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 5:
                        if (Tile.left == null) { return; }
                        BresenHam(Tile.left, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 6:
                        if (Tile.down == null) { return; }
                        BresenHam(Tile.down, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 7:
                        if (Tile.down == null) { return; }
                        BresenHam(Tile.down, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 8:
                        if (Tile.right == null) { return; }
                        BresenHam(Tile.right, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 33:
                        if (Tile.up == null) { return; }
                        BresenHam(Tile.up, x2, y2, 0, 0, octant, modifier);
                        break;
                    case 77:
                        if (Tile.down == null) { return; }
                        BresenHam(Tile.down, x2, y2, 0, 0, octant, modifier);
                        break;
                }
                
            }
            else
            {
                errCount--;
                switch (octant)
                {
                    case 1:
                        //print("Got here too");
                        if (Tile.right == null) { return; }
                        if (Tile.right.up == null) { return; }
                        BresenHam(Tile.right.up, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 2:
                        if (Tile.up == null) { return; }
                        if (Tile.up.right == null) { return; }
                        BresenHam(Tile.up.right, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 3:
                        if (Tile.up == null) { return; }
                        if (Tile.up.left == null) { return; }
                        BresenHam(Tile.up.left, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 4:
                        if (Tile.left == null) { return; }
                        if (Tile.left.up == null) { return; }
                        BresenHam(Tile.left.up, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 5:
                        if (Tile.left == null) { return; }
                        if (Tile.left.down == null) { return; }
                        BresenHam(Tile.left.down, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 6:
                        if (Tile.down == null) { return; }
                        if (Tile.down.left == null) { return; }
                        BresenHam(Tile.down.left, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 7:
                        if (Tile.down == null) { return; }
                        if (Tile.down.right == null) { return; }
                        BresenHam(Tile.down.right, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 8:
                        if (Tile.right == null) { return; }
                        if (Tile.right.down == null) { return; }
                        BresenHam(Tile.right.down, x2, y2, err, errCount, octant, modifier);
                        break;
                    case 33:
                        break;
                    case 77:
                        break;
                }
            }
        }
    }
}

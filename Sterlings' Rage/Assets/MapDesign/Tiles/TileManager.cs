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
}

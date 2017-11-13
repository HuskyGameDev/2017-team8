using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public static MapTile[,] mapTiles;
    public int mapWidthX;
	public int mapHeightY;
    public static int width;
	public static int height;
    private static UnitClass selectedUnit;
    private static ArrayList pathList;

    // Use this for initialization
    void Start()
    {
        // Allows us to set the size from within the editor and then storing in the static variable allows 
        // the size to be used in the static methods
		width = mapWidthX;
		height = mapHeightY;
		mapTiles = new MapTile[width, height];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void addTile(MapTile mapTile, int x, int y)
    {
        mapTiles[x, y] = mapTile;
        if (checkIfFull())
        {
            fillAdjacentTiles();
        }
    }

    private static bool checkIfFull()
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
        return true;
    }

    private static void fillAdjacentTiles()
    {
		for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
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

    public static void resetTiles()
    {
        foreach (MapTile tile in pathList)
        {
            tile.resetColor();
            tile.setPossibleMove(false);
        }
        pathList = null;
        selectedUnit = null;
    }

    public static void setSelectedUnit(UnitClass unit)
    {
        selectedUnit = unit;
    }

    public static UnitClass getSelectedUnit()
    {
        return selectedUnit;
    }

    public static void setPathList(ArrayList path)
    {
        pathList = path;
    }

    public static ArrayList getPathList()
    {
        return pathList;
    }
}

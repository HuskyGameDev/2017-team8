using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public static MapTile[,] mapTiles;
    public int mapSize;
    private static int size;

    // Use this for initialization
    void Start()
    {
        // Allows us to set the size from within the editor and then storing in the static variable allows 
        // the size to be used in the static methods
        size = mapSize;
        mapTiles = new MapTile[size, size];
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
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (mapTiles[i, j] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static void fillAdjacentTiles()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i + 1 < size)
                {
                    mapTiles[i, j].setRight(mapTiles[i + 1, j]);
                }
                if (j + 1 < size)
                {
                    mapTiles[i, j].setUp(mapTiles[i, j + 1]);
                }
                if (i - 1 >= 0)
                {
                    mapTiles[i, j].setLeft(mapTiles[i - 1, j]);
                }
                if (j - 1 >= 0)
                {
                    mapTiles[i, j].setDown(mapTiles[i, j - 1]);
                }
            }
        }
    }
}

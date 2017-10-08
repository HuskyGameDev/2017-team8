using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public static MapTile[,] mapTiles;

	// Use this for initialization
	void Start () {
        mapTiles = new MapTile[5,5];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void addTile(MapTile mapTile, int x, int y)
    {
        mapTiles[x,y] = mapTile;
    }
}

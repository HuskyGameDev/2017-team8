using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public static MapTile[,] mapTiles;
	public int size;

	// Use this for initialization
	void Start () {
		print ("size = " + size);
        mapTiles = new MapTile[size,size];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void addTile(MapTile mapTile, int x, int y)
    {
		
       	 mapTiles[x,y] = mapTile;
    }
}

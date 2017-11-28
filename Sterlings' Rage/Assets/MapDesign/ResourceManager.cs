using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public int initialContraband;
    public static int contraband;

	// Use this for initialization
	void Start () {
        contraband = initialContraband;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void pickUpContraband(MapTile tile)
    {
        Add(tile.contraband.value);
        tile.contraband.Remove();
        tile.contraband = null;
    }

    public static void Add(int amount)
    {
        contraband = contraband + amount;
    }

    public static void Subtract(int amount)
    {
        contraband = contraband - amount;
    }


}

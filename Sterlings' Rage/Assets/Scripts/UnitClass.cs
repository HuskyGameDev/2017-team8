using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClass {
    private string unitClassName;

    // Test Comment

    // Stats
    private int health;
    private int speed;
    private int range;
    private int damage;
    private string sprite;
    private int cost;
    MapTile currentTile;

    public string UnitClassName {
        get { return unitClassName; }
        set { unitClassName = value; }
    }

    public string Sprite {
        get { return sprite; }
        set { sprite = value; }
    }

    public int Health {
        get { return health; }
        set { health = value; }
    }

    public int Speed {
        get { return speed; }
        set { speed = value; }
    }

    public int Range {
        get { return range; }
        set { range = value; }
    }

    public int Damage {
        get { return damage; }
        set { damage = value; }
    }

    public int Cost {
        get { return cost; }
        set { cost = value; }
    }

    public MapTile CurrentTile{
        get { return currentTile; }
        set { currentTile = value; }
    }

    public bool MoveTo(MapTile dest) {
        // if the destination tile is range, changes unit's current tile to the destination
        // and returns true
        // if out of range, returns false and does nothing
        int destx = (int)dest.transform.position.x;
        int desty = (int)dest.transform.position.y;
        int currentx = (int)currentTile.transform.position.x;
        int currenty = (int)currentTile.transform.position.y;
        if(Mathf.Abs(destx - currentx) + Mathf.Abs(desty - currenty) <= range)
        {
            currentTile = dest;
            return true;
        }
        return false;
    }
	
}

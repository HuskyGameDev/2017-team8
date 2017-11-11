using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour {
    public string unitClassName;

    // Test Comment

    // Stats
    public int health;
    public int speed;
    public float range;
    public int damage;
    public string sprite;
    public int cost;
    public MapTile currentTile;
    private MapTile dest;
    private bool isMovingX;
    private bool isMovingY;

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

    public float Range {
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

    public GameObject EnemyInRange(float range){
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //print("Enemies found " + enemies.Length);
        GameObject closest = null;
        float distance = range;
        //print("targeting range is " + distance);
        Vector3 pos = transform.position;
        //print("Pos is " + pos);
        foreach(GameObject go in enemies){
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if(curDistance <= distance){
                //print("target in range");
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
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
            isMovingX = true;
            this.dest = dest;
            return true;
        }
        return false;
    }

    public void Update()
    {
        if (isMovingX)
        {
            if (dest.transform.position.x < currentTile.transform.position.x)
            {
                transform.Translate(-0.1f, 0, 0);
                if(dest.transform.position.x >= transform.position.x)
                {
                    isMovingX = false;
                    isMovingY = true;
                }
            }
            else
            {
                transform.Translate(0.1f, 0, 0);
                if (dest.transform.position.x <= transform.position.x)
                {
                    isMovingX = false;
                    isMovingY = true;
                }
            }
        }
        else if (isMovingY)
        {
            if(dest.transform.position.y < currentTile.transform.position.y)
            {
                transform.Translate(0, 0.1f, 0);
                if(dest.transform.position.y >= transform.position.y)
                {
                    isMovingY = false;
                    currentTile = dest;
                    dest = null;
                }
            }
            else
            {
                transform.Translate(0, 0.1f, 0);
                if (dest.transform.position.y <= transform.position.y)
                {
                    isMovingY = false;
                    currentTile = dest;
                    dest = null;
                }
            }
        }
    }

    private void Start()
    {
        isMovingY = false;
        isMovingY = false; 
        range = 10;
        dest = null;
    }

}

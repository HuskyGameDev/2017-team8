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

    public void displayMovementPath()
    {
        int x = (int)gameObject.transform.position.x;
        int y = (int)gameObject.transform.position.y;

        print("CUrious at:" +x+","+y);
        TileManager.setSelectedUnit(this);
        ArrayList pathTiles = new ArrayList();

        if (x - 1 >= 0)
        {
            findPossiblePaths(x - 1, y, speed - TileManager.mapTiles[x - 1, y].getMovementWeight(), pathTiles);
        }
        if (y - 1 >= 0)
        {
            findPossiblePaths(x, y - 1, speed - TileManager.mapTiles[x, y - 1].getMovementWeight(), pathTiles);
        }
        if (x + 1 < TileManager.width)
        {
            findPossiblePaths(x + 1, y, speed - TileManager.mapTiles[x + 1, y].getMovementWeight(), pathTiles);
        }
        if (y + 1 < TileManager.height)
        {
            findPossiblePaths(x, y + 1, speed - TileManager.mapTiles[x, y + 1].getMovementWeight(), pathTiles);
        }

        TileManager.setPathList(pathTiles);
    }

    private void findPossiblePaths(int x, int y, int speed, ArrayList pathTiles)
    {
        if(speed < 0)
        {
            return;
        }

        MapTile curTile =TileManager.mapTiles[x, y];
        curTile.setPossibleMove(true);
        pathTiles.Add(curTile);

        if (x - 1 >= 0)
            findPossiblePaths(x - 1, y, speed - TileManager.mapTiles[x - 1, y].getMovementWeight(), pathTiles);
        if (y - 1 >= 0)
            findPossiblePaths(x, y - 1, speed - TileManager.mapTiles[x, y - 1].getMovementWeight(), pathTiles);
        if (x + 1 < TileManager.width)
            findPossiblePaths(x + 1, y, speed - TileManager.mapTiles[x + 1, y].getMovementWeight(), pathTiles);
        if (y + 1 < TileManager.height)
            findPossiblePaths(x, y+1, speed - TileManager.mapTiles[x, y + 1].getMovementWeight(), pathTiles);
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

    public void MoveTo(MapTile dest) {
        
        isMovingX = true;
        this.dest = dest;
        dest.currentUnit = this;
        currentTile.currentUnit = null;
    }

    public void Update()
    {
        if (isMovingX)
        {
            if (dest.transform.position.x < currentTile.transform.position.x)
            {
                if(dest.transform.position.x >= transform.position.x)
                {
                    isMovingX = false;
                    isMovingY = true;
                } else 
                    transform.Translate(-0.1f, 0, 0);
            }
            else
            {
                if (dest.transform.position.x <= transform.position.x)
                {
                    isMovingX = false;
                    isMovingY = true;
                } else
                    transform.Translate(0.1f, 0, 0);
            }
        }
        else if (isMovingY)
        {
            if(dest.transform.position.y < currentTile.transform.position.y)
            {
                if(dest.transform.position.y >= transform.position.y)
                {
                    isMovingY = false;
                    currentTile = dest;
                    // Because of how floats work need to make sure to set the units position to be the same as the tile or
                    // sometimes it will move to far
                    gameObject.transform.position = new Vector2(dest.transform.position.x, dest.transform.position.y);
                    dest = null;
                } else
                    transform.Translate(0, -0.1f, 0);
            }
            else
            {
                if (dest.transform.position.y <= transform.position.y)
                {
                    isMovingY = false;
                    currentTile = dest;
                    gameObject.transform.position = new Vector2(dest.transform.position.x, dest.transform.position.y);
                    dest = null;
                } else
                    transform.Translate(0, 0.1f, 0);
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

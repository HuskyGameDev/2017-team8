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
        // Resets if the user had already selected a different unit
        if (TileManager.getSelectedUnit() != null)
            TileManager.resetTiles();
        TileManager.setSelectedUnit(this);
        
        ArrayList pathTiles = new ArrayList();
        MapTile curTile = TileManager.mapTiles[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y];

        if (curTile.getLeft() != null)
        {
            findPossiblePaths(curTile.getLeft(), speed - curTile.getLeft().getMovementWeight(), pathTiles);
        }
        if (curTile.getDown() != null)
        {
            findPossiblePaths(curTile.getDown(), speed - curTile.getDown().getMovementWeight(), pathTiles);
        }
        if (curTile.getRight() != null)
        {
            findPossiblePaths(curTile.getRight(), speed - curTile.getRight().getMovementWeight(), pathTiles);
        }
        if (curTile.getUp() != null)
        {
            findPossiblePaths(curTile.getUp(), speed - curTile.getUp().getMovementWeight(), pathTiles);
        }
        TileManager.setPathList(pathTiles);
    }

    private void findPossiblePaths(MapTile curTile, int speed, ArrayList pathTiles)
    {
        if (speed < 0)
        {
            return;
        }

        
        if (curTile.currentUnit == null)
        {
            curTile.setPossibleMove(true);
            // Stores the speed that it was found with so that if a faster path is found it will know
            curTile.setStoredSpeed(speed);
            
        }
        pathTiles.Add(curTile);

        if (curTile.getLeft() != null && !TileManager.containsTile(speed - curTile.getLeft().getMovementWeight(), curTile.getLeft()))
            findPossiblePaths(curTile.getLeft(), speed - curTile.getLeft().getMovementWeight(), pathTiles);

        if (curTile.getDown() != null && !TileManager.containsTile(speed - curTile.getDown().getMovementWeight(), curTile.getDown()))
            findPossiblePaths(curTile.getDown(), speed - curTile.getDown().getMovementWeight(), pathTiles);

        if (curTile.getRight() != null && !TileManager.containsTile(speed - curTile.getRight().getMovementWeight(), curTile.getRight()))
            findPossiblePaths(curTile.getRight(), speed - curTile.getRight().getMovementWeight(), pathTiles);

        if (curTile.getUp() != null && !TileManager.containsTile(speed - curTile.getUp().getMovementWeight(), curTile.getUp()))
            findPossiblePaths(curTile.getUp(), speed - curTile.getUp().getMovementWeight(), pathTiles);
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

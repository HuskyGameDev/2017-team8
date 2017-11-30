using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour {
    public string unitClassName;

    // Test Comment

    // Stats
    public int health;
    public int speed;
    public int range;
    public int damage;
    public string sprite;
    public int cost;
    public MapTile currentTile;
    private MapTile dest;
    private ArrayList path;
    private bool isMovingX;
    private bool isMovingY;
    private bool moving;

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

    public void DisplayAttackRange()
    {
        ArrayList attackTiles = new ArrayList();
        TileManager.resetMovementTiles();

        if (currentTile.getLeft() != null)
        {
            findAttackTiles(currentTile.getLeft(), range - 1, attackTiles);
        }
        if (currentTile.getDown() != null)
        {
            findAttackTiles(currentTile.getDown(), range - 1, attackTiles);
        }
        if (currentTile.getRight() != null)
        {
            findAttackTiles(currentTile.getRight(), range - 1, attackTiles);
        }
        if (currentTile.getUp() != null)
        {
            findAttackTiles(currentTile.getUp(), range - 1, attackTiles);
        }
        TileManager.setAttackList(attackTiles);
    }

    public void findAttackTiles(MapTile curTile, int range, ArrayList attackTiles)
    {
        if (range < 0)
        {
            return;
        }
        if (curTile != currentTile)
        {
            curTile.setPossibleAttack(true);
            curTile.setStoredRange(range);
            attackTiles.Add(curTile);
        }


        if (curTile.getLeft() != null && !TileManager.containsAttackTile(range - 1, curTile.getLeft()))
        {
            findAttackTiles(curTile.getLeft(), range - 1, attackTiles);
        }
        if (curTile.getDown() != null && !TileManager.containsAttackTile(range - 1, curTile.getDown()))
        {
            findAttackTiles(curTile.getDown(), range - 1, attackTiles);
        }
        if (curTile.getRight() != null && !TileManager.containsAttackTile(range - 1, curTile.getRight()))
        {
            findAttackTiles(curTile.getRight(), range - 1, attackTiles);
        }
        if (curTile.getUp() != null && !TileManager.containsAttackTile(range - 1, curTile.getUp()))
        {
            findAttackTiles(curTile.getUp(), range - 1, attackTiles);
        }
    }

    public void displayMovementPath()
    {
        // Resets if the user had already selected a different unit
        if (TileManager.getSelectedUnit() != null)
            TileManager.resetMovementTiles();
        TileManager.setSelectedUnit(this);

        ArrayList pathTiles = new ArrayList();

        if (currentTile.getLeft() != null)
        {
            findPossiblePaths(currentTile.getLeft(), speed - currentTile.getLeft().getMovementWeight(), pathTiles, currentTile);
        }
        if (currentTile.getDown() != null)
        {
            findPossiblePaths(currentTile.getDown(), speed - currentTile.getDown().getMovementWeight(), pathTiles, currentTile);
        }
        if (currentTile.getRight() != null)
        {
            findPossiblePaths(currentTile.getRight(), speed - currentTile.getRight().getMovementWeight(), pathTiles, currentTile);
        }
        if (currentTile.getUp() != null)
        {
            findPossiblePaths(currentTile.getUp(), speed - currentTile.getUp().getMovementWeight(), pathTiles, currentTile);
        }
        TileManager.setPathList(pathTiles);
    }

    private void findPossiblePaths(MapTile curTile, int speed, ArrayList pathTiles, MapTile previous)
    {
        if (speed < 0)
        {
            return;
        }
        if (curTile != currentTile)
        {
            curTile.setPossibleMove(true);
            curTile.setPreviousTile(previous);
            // Stores the speed that it was found with so that if a faster path is found it will know
            curTile.setStoredSpeed(speed);
            pathTiles.Add(curTile);
        }

        // Recursive calls to all adjacent tiles that haven't already been checked or that have but had a lower speed when they were reached.
        if (curTile.getLeft() != null && !TileManager.containsTile(speed - curTile.getLeft().getMovementWeight(), curTile.getLeft()))
            findPossiblePaths(curTile.getLeft(), speed - curTile.getLeft().getMovementWeight(), pathTiles, curTile);

        if (curTile.getDown() != null && !TileManager.containsTile(speed - curTile.getDown().getMovementWeight(), curTile.getDown()))
            findPossiblePaths(curTile.getDown(), speed - curTile.getDown().getMovementWeight(), pathTiles, curTile);

        if (curTile.getRight() != null && !TileManager.containsTile(speed - curTile.getRight().getMovementWeight(), curTile.getRight()))
            findPossiblePaths(curTile.getRight(), speed - curTile.getRight().getMovementWeight(), pathTiles, curTile);

        if (curTile.getUp() != null && !TileManager.containsTile(speed - curTile.getUp().getMovementWeight(), curTile.getUp()))
            findPossiblePaths(curTile.getUp(), speed - curTile.getUp().getMovementWeight(), pathTiles, curTile);
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

    public void MoveTo(ArrayList movementPath, MapTile dest)
    {
        if(path == null)
            path = new ArrayList();
        path.Clear();
        // Have to add to the path for this unit because the tile will clear it's movement path 
        // before it is able to travel there
        for (int i = movementPath.Count - 1; i >= 0; i--)
        {
            path.Add(movementPath[i]);
        }
        path.Add(dest);
        this.dest = (MapTile)path[0];
        path.Remove(this.dest);
        currentTile.currentUnit = null;
        currentTile = dest;
        isMovingX = true;
        moving = true;
    }

    public void Update()
    {
        if (moving)
        {
            if (isMovingX)
            {
                if (dest.transform.position.x < transform.position.x)
                {
                    transform.Translate(-0.1f, 0, 0);
                    if (dest.transform.position.x >= transform.position.x)
                    {
                        isMovingY = true;
                        isMovingX = false;
                    }
                }
                else if (dest.transform.position.x > transform.position.x)
                {
                    transform.Translate(0.1f, 0, 0);
                    if (dest.transform.position.x <= transform.position.x)
                    {
                        isMovingY = true;
                        isMovingX = false;
                    }
                }
                else
                {
                    isMovingY = true;
                    isMovingX = false;
                }
            }
            else if (isMovingY)
            {
                if (dest.transform.position.y < transform.position.y)
                {
                    transform.Translate(0, -0.1f, 0);
                    if (dest.transform.position.y >= transform.position.y)
                    {
                        isMovingY = false;
                        // Because of how floats work need to make sure to set the units position to be the same as the tile or
                        // sometimes it will move to far
                        gameObject.transform.position = new Vector2(dest.transform.position.x, dest.transform.position.y);                        
                    }
                }
                else
                {
                    transform.Translate(0, 0.1f, 0);
                    if (dest.transform.position.y <= transform.position.y)
                    {
                        isMovingY = false;
                        gameObject.transform.position = new Vector2(dest.transform.position.x, dest.transform.position.y);
                    }
                }
            }

            if (!isMovingX && !isMovingY)
            {
                if(dest.contraband != null)
                {
                    ResourceManager.pickUpContraband(dest);
                }
                if (path.Count > 0)
                {
                    dest = (MapTile)path[0];
                    path.Remove(dest);
                    isMovingX = true;
                } else
                {
                    moving = false;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    private bool aiTurn = false;
    private bool test = false;
    private TurnManager turnManager;
    private TileManager tileManager;
    private UnitManager unitManager;

    private void testQueue()
    {
        ArrayList visited = new ArrayList();
        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        for (int i = 0; i < 10; i++)
        {
            Node start = new Node(tileManager.mapTiles[0,0], null, UnityEngine.Random.Range(0, 100));
            queue.Push(start);
        }
        test = true;
        queue.Push(new Node(tileManager.mapTiles[1, 1], null, -1));
        test = false;
        while (queue.Size() > 0)
        {
            Node temp = queue.Pop();
            print(temp.value);
        }
    }

    // Use this for initialization
    void Start() {

        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.T))
            test = true;
        if (Input.GetKey(KeyCode.P))
            testQueue();
        // In case it didn't find it the first time
        if (unitManager == null || tileManager == null || turnManager == null)
        {
            unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
            tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
            turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        }
        if (!turnManager.playerTurn && !aiTurn)
        {
            aiTurn = true;
            // Need coroutine so that the AI can wait for units to move before attacking
            StartCoroutine(takeTurn());
        }

    }

    private IEnumerator takeTurn()
    {
        // Makes a copy of the AI's current units because if one happens to die it will change the list causing the following loop to crash
        ArrayList temp = new ArrayList();
        foreach (UnitClass unit in unitManager.EnemyUnits)
            temp.Add(unit);
        try
        {
            foreach (UnitClass unit in temp)
            {
                UnitClass playerUnit = findNearestPlayerUnit(unit);
                if (playerUnit != null)
                {
                    if (ManhattanDistance(unit.gameObject, playerUnit.gameObject) > unit.range)
                        yield return StartCoroutine(move(unit, playerUnit));
                    
                    // Currently possible for a different unit to kill the inteded target before reaching this point so
                    // null check makes sure it is still alive
                    if (playerUnit != null && ManhattanDistance(unit.currentTile.gameObject, playerUnit.gameObject) <= unit.range)
                        yield return StartCoroutine(attack(unit, playerUnit));
                }
            }
        }
        finally
        {
            turnManager.newTurn();
            aiTurn = false;
        }


    }

    /**
     * Moves the aiUnit towards the playerUnit
     */
    private IEnumerator move(UnitClass aiUnit, UnitClass playerUnit)
    {
        aiUnit.displayMovementPath();
        int distance = int.MaxValue;
        MapTile targetTile = findClosestOpenAdjacentTile(aiUnit.currentTile, playerUnit.currentTile);
        if (targetTile == null) {
            MapTile closest = findClosestAdjacentTile(aiUnit.currentTile, playerUnit.currentTile);
            while (targetTile == null)
                {
                targetTile = findClosestOpenAdjacentTile(aiUnit.currentTile, closest);
                    closest = findClosestAdjacentTile(aiUnit.currentTile, closest);
                }
        }
        ArrayList pathToUnit = FindPathToUnit(aiUnit.currentTile, targetTile);
        ArrayList movementPath = new ArrayList();
        for(int i = aiUnit.speed; i > 0; i--)
        {
            if (pathToUnit.Count - i >= 0)
            {
                MapTile tile = (MapTile)pathToUnit[pathToUnit.Count - i];
                //rint(tile.gameObject.transform.position.x + "," + tile.gameObject.transform.position.y);
                movementPath.Add(pathToUnit[pathToUnit.Count - i]);
            }
        }
        targetTile = (MapTile)movementPath[0];
        if (targetTile.currentUnit != null)
        {
            movementPath.Remove(targetTile);
            while (movementPath.Count > 0)
            {
                targetTile = (MapTile)movementPath[0];
                if (targetTile.currentUnit == null)
                    break;
                movementPath.Remove(targetTile);
            }
        }
        aiUnit.MoveTo(movementPath, targetTile);
        targetTile.currentUnit = aiUnit;
        tileManager.resetAllTiles();
        return new WaitUntil(() => aiUnit.moving == false);
    }

    private IEnumerator attack(UnitClass aiUnit, UnitClass playerUnit)

    {
        playerUnit.currentTile.attack(aiUnit, playerUnit, aiUnit.currentTile, playerUnit.currentTile);
        //play animation
        aiUnit.gameObject.GetComponent<Animator>().Play("Attack");
        tileManager.resetAllTiles();
        // Need to return some sort of Enumerator to work as intended
        return new WaitUntil(() => 1 == 1);
    }

    private MapTile findClosestAdjacentTile(MapTile ai, MapTile player)
    {
        
        MapTile closest = null;
        List<MapTile> adjacentTiles = player.getAdjacentTiles();
        
        int min = int.MaxValue;
        foreach (MapTile tile in player.getAdjacentTiles())
        {
            if (ManhattanDistance(ai.gameObject, tile.gameObject) < min)
            {
                min = ManhattanDistance(ai.gameObject, tile.gameObject);
                closest = tile;
            }
        }
        
        return closest;
    }

    private MapTile findClosestOpenAdjacentTile(MapTile ai, MapTile player)
    {

        MapTile closest = null;
        List<MapTile> adjacentTiles = player.getAdjacentTiles();

        int min = int.MaxValue;
        foreach (MapTile tile in player.getAdjacentTiles())
        {
            if (!tile.tileType.Equals("Building"))
            {
                if (ManhattanDistance(ai.gameObject, tile.gameObject) < min && tile.currentUnit == null)
                {
                    min = ManhattanDistance(ai.gameObject, tile.gameObject);
                    closest = tile;
                }
            }
        }

        return closest;
    }

    private int ManhattanDistance(GameObject unit, GameObject otherUnit)
    {
        return (int)(Mathf.Abs((unit.transform.position.x - otherUnit.transform.position.x)) + Mathf.Abs((unit.transform.position.y - otherUnit.transform.position.y)));
    }

    /**
     * A* search to find a path from the AI unit to the player unit
     **/
    private ArrayList FindPathToUnit(MapTile startTile, MapTile endTile)
    {
        ArrayList visited = new ArrayList();
        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        Node start = new Node(startTile, null, 0);
        queue.Push(start);
        int currentPathLength = 0;
        while(true)
        {
            if (queue.Size() == 0)
                print("well theres a problem");
            Node curNode = queue.Pop();
            if (test)
                curNode.tile.GetComponent<SpriteRenderer>().color = Color.blue;
            if (!visited.Contains(curNode.tile))
            {
                visited.Add(curNode.tile);
                if (curNode.tile == endTile)
                {
                    start = curNode;

                    break;
                }
                foreach (MapTile tile in curNode.tile.getAdjacentTiles())
                {
                    if (!tile.tileType.Equals("Building") && !visited.Contains(tile))
                    {
                        Node adj = new Node(tile, curNode,
                            tile.getMovementWeight() + ManhattanDistance(tile.gameObject, endTile.gameObject) + curNode.current + 1);
                        adj.current = curNode.current + 1;
                        queue.Push(adj);

                    }
                }
            }
        }

        ArrayList path = new ArrayList();
        // is currently the tile the player unit is on, can't move directly on top so get the tile before it on the path
        //start = start.parent;
        while(start.tile != startTile)
        {
            //rint(start.tile.gameObject.transform.position.x + "," + start.tile.gameObject.transform.position.y);
            path.Add(start.tile);
            start = start.parent;
            
        }
        
        return path;
    }

    private UnitClass findNearestPlayerUnit(UnitClass aiUnit)
    {
        float minDist = float.MaxValue;
        UnitClass nearestUnit = null;
        foreach (UnitClass playerUnit in unitManager.PlayerUnits)
        {
            //add in a visibility modifier
            if (playerUnit.spotted) {
                float curDist = distanceBetween(playerUnit.gameObject, aiUnit.gameObject);
                if (curDist < minDist)
                {
                    nearestUnit = playerUnit;
                    minDist = curDist;
                }
            }
        }
        return nearestUnit;
    }

    private float distanceBetween(GameObject unit, GameObject otherUnit)
    {
        return Mathf.Sqrt(Mathf.Pow((unit.transform.position.x - otherUnit.transform.position.x), 2f)
            + Mathf.Pow((unit.transform.position.y - otherUnit.transform.position.y), 2f));
    }

    public class PriorityQueue<T> where T : IComparable<T> {

        private List<T> data;

        public PriorityQueue()
        {
            data = new List<T>();
        }

        public void Push(T item)
        {
            data.Add(item);

            int child = data.Count - 1;
            
            while(child > 0)
            {
                int parent = ((child-1) / 2);

                if (data[parent].CompareTo(data[child]) < 0)
                    break;
                T temp = data[parent];
                data[parent] = data[child];
                data[child] = temp;
                child = parent;
            }
        }

        public T Pop ()
        {
            T ret = data[0];
            data[0] = data[data.Count - 1];
            data.RemoveAt(data.Count - 1);
            int index = 0;
            
            int last = data.Count - 1;
            while (true)
            {
                int leftChild = index*2+1;
                int rightChild = leftChild + 1;
                if (leftChild > last)
                    break;
                if (rightChild <= last && data[leftChild].CompareTo(data[rightChild]) > 0)
                    leftChild = rightChild;
                if (data[index].CompareTo(data[leftChild]) > 0)
                {
                    T temp = data[index];
                    data[index] = data[leftChild];
                    data[leftChild] = temp;
                    index = leftChild;
                }
                else
                {
                    break;
                }
            }
            return ret;
        }

        public int Size()
        {
            return data.Count;
        }
    }

    public class Node : IComparable<Node>
    {
        public MapTile tile;
        public Node parent;
        public int value;
        public int current = 0;

        public Node(MapTile t, Node p, int v)
        {
            tile = t;
            parent = p;
            value = v;
        }

        public override string ToString()
        {
            return ""+value;
        }

        public int CompareTo(Node other)
        {
            if (value > other.value)
                return 1;
            else
                return -1;
        }
    }
}

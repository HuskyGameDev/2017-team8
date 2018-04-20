using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    private bool aiTurn = false;
    private bool test = false;
    private bool endThreads = false;
    private ArrayList objectives;
    private Thread objectiveBuilder;
    private Thread aiTurnTaker;
    private TurnManager turnManager;
    private TileManager tileManager;
    private UnitManager unitManager;

    private static int inAttckRangeValue = 6;
    private static int inPossibleAttackRangeValue = 3;
    private static int acquireContrabandValue = -2;
    private static int attackUnitValue = -8;
    private static int killUnitValue = -15;


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
    void OnApplicationQuit()
    {
        endThreads = true;
    }

    // Use this for initialization
    void Start() {
        //objectives = new List<KeyValuePair<MapTile, string>>();
        objectives = new ArrayList();
        objectiveBuilder = new Thread(objectiveListHandler);
        objectiveBuilder.Start();

        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.T))
        {
            foreach (KeyValuePair<MapTile, object> cur in objectives)
            {
                print(cur);
            }
        }
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
            // New thread keeps the game from "freezing" while the AI is determining the correct move
            //aiTurnTaker = new Thread(startTurn); Have to fix a few things before this can be implemented
            StartCoroutine(takeTurn());

        }

    }

    /**
     * This will be it's own thread that will build a list of objectives that the AI can choose from
     **/
    private void objectiveListHandler()
    {
        while(!endThreads)
        {
            try
            {
                //print("still running");
                foreach (UnitClass unit in unitManager.EnemyUnits)
                {
                    ArrayList foundObectives = new ArrayList();
                    for (int rangeX = -5; rangeX <= 5; rangeX++)
                    {
                        for (int rangeY = -5; rangeY <= 5; rangeY++)
                        {
                            if (Mathf.Abs(rangeX) + Mathf.Abs(rangeY) <= 5)
                            {
                                unit.currentTile.Bern(unit.currentTile.xPosition + rangeX, unit.currentTile.yPosition + rangeY, foundObectives);
                            }
                        }
                    }

                    if (foundObectives != null)
                    {
                        foreach (KeyValuePair<MapTile, object> obj in foundObectives)
                        {
                            if (!objectives.Contains(obj))
                            {
                                foreach (KeyValuePair<MapTile, object> cur in objectives)
                                {
                                    if (cur.Value == obj.Value)
                                    {
                                        lock (objectives)
                                        {
                                            objectives.Remove(cur);
                                        }
                                    }
                                }
                                lock (objectives)
                                {
                                    objectives.Add(obj);
                                }
                            }
                        }
                    }
                }
            } catch(Exception e)
            {
                print(e.StackTrace);
            }
            unitManager.getSelectedUnit();
        }
    }

    public void remove(UnitClass unit)
    {
        lock (objectives)
        {
            foreach (KeyValuePair<MapTile, object> obj in objectives)
            {
                if(obj.Value == unit)
                {
                    objectives.Remove(obj);
                    print("removed");
                    break;
                }
            }
        }
    }

    public void remove(Contraband contraband)
    {
        lock (objectives)
        {
            foreach (KeyValuePair<MapTile, object> obj in objectives)
            {
                if (obj.Value == contraband)
                {
                    objectives.Remove(obj);
                    print("removed");
                    break;
                }
            }
        }
    }

    /**
     * Function used as a new thread to take the AI's turn
     **/
    private void startTurn()
    {
        // Need coroutine so that the AI can wait for units to move before attacking
        StartCoroutine(takeTurn());
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
                Objective next = determineMove(unit);
                
                MapTile nextMove = next.tile;
                //UnitClass playerUnit = findNearestPlayerUnit(unit);
                if (nextMove != null)
                {
                    // if (ManhattanDistance(unit.gameObject, playerUnit.gameObject) > unit.range)
                    //     yield return StartCoroutine(move(unit, playerUnit));
                    yield return StartCoroutine(move(unit, nextMove));
                    
                    // Currently possible for a different unit to kill the inteded target before reaching this point so
                    // null check makes sure it is still alive
                    if (next.unit != null && next.attack)
                        yield return StartCoroutine(attack(unit, next.unit));
                }
            }
        }
        finally
        {
            turnManager.newTurn();
            aiTurn = false;
        }


    }

    private Objective determineMove(UnitClass unit)
    {
        // Used to copy the objective list so it isn't locked for too long
        //List<KeyValuePair<MapTile, string>> tempList = new List<KeyValuePair<MapTile, string>>();
        ArrayList tempList = new ArrayList();
        // lock makes sure the objective list isn't being written over as it is being checked
        if (objectives.Count > 0)
        {
            lock (objectives)
            {
                foreach (KeyValuePair<MapTile, object> obj in objectives)
                {
                    tempList.Add(obj);
                }
            }
        }
        setDangerousTiles(tempList);
        unit.displayMovementPath();
        PriorityQueue<Objective> queue = new PriorityQueue<Objective>();
        foreach (KeyValuePair<MapTile, object> obj in tempList)
        {
            Contraband contraband;
            UnitClass objUnit;
            ArrayList pathToObj;
            ArrayList movementPath = new ArrayList();

            try
            {
                print("Made it here");
                objUnit = (UnitClass)obj.Value;
                pathToObj = FindPathToUnit(unit.currentTile, findClosestAdjacentTile(unit.currentTile, objUnit.currentTile));
                int closerToObj = 0;
                for (int i = unit.speed; i > 0; i--) 
                {
                    if (pathToObj.Count - i >= 0)
                    {
                        movementPath.Add(pathToObj[pathToObj.Count - i]);
                    }
                }
                foreach (MapTile curTile in movementPath)
                {
                    queue.Push(DetermineObjectiveValue(curTile, unit, objUnit, closerToObj));
                    closerToObj++;
                }
            } catch (Exception e) {
                print("also it here");
                contraband = (Contraband)obj.Value;
                pathToObj = FindPathToUnit(unit.currentTile, contraband.currentTile);
                int closerToObj = 0;
                for (int i = unit.speed; i > 0; i--)
                {
                    if (pathToObj.Count - i >= 0)
                    {
                        movementPath.Add(pathToObj[pathToObj.Count - i]);
                    }
                }
                foreach (MapTile curTile in movementPath)
                {
                    queue.Push(DetermineObjectiveValue(obj.Key, unit, contraband, closerToObj));
                    closerToObj++;
                }
            }
        }
        foreach(MapTile tile in tileManager.getPathList())
        {
            queue.Push(DetermineObjectiveValue(tile));
        }
        return queue.Pop();
    }

    private Objective DetermineObjectiveValue(MapTile tile, UnitClass aiUnit, UnitClass playerUnit, int closerTo)
    {
        Objective ret = new Objective(tile, 0);
        int value = 0;
        if (tile.isInAttackRange())
            value += inAttckRangeValue;
        else if (tile.isInPossibleAttackRange())
            value += inPossibleAttackRangeValue;
        if (ManhattanDistance(tile.gameObject, playerUnit.gameObject) <= aiUnit.range)
        {
            if (tile.calculateAttackDamage(aiUnit, playerUnit) >= playerUnit.health)
            {
                value += killUnitValue;
            }
            else
                value += attackUnitValue;
            ret.unit = playerUnit;
            ret.attack = true;
        }
        value += closerTo;
        ret.value = value;
        print(ret.tile + ", " + ret.value);
        return ret;
    }

    /** Determines the value if the objective tile has contraband
     **/
    private Objective DetermineObjectiveValue(MapTile tile, UnitClass aiUnit, Contraband contraband, int closerTo)
    {
        int value = 0;
        if (tile.isInAttackRange())
            value += inAttckRangeValue;
        else if (tile.isInPossibleAttackRange())
            value += inPossibleAttackRangeValue;
        if (contraband.currentTile == tile)
            value += acquireContrabandValue;
        value += 2 * closerTo;
        Objective ret = new Objective(tile, value);
        print(ret.tile + ", " + ret.value);
        return ret;
    }

    private Objective DetermineObjectiveValue(MapTile tile)
    {
        int value = 10;
        if (tile.isInAttackRange())
            value += inAttckRangeValue;
        else if (tile.isInPossibleAttackRange())
            value += inPossibleAttackRangeValue;
        //print("Here: " + tile + ", " + value);
        return new Objective(tile, value);
    }

    private void setDangerousTiles(ArrayList objList)
    {
        ArrayList dangerousTiles = new ArrayList();
        foreach (KeyValuePair<MapTile, object> obj in objList)
        {
            if (obj.Value == typeof(UnitClass))
            {
                UnitClass playerUnit = (UnitClass)obj.Value;
                playerUnit.displayMovementPath();
                foreach (MapTile tile in tileManager.getPathList())
                {
                    if (tile == playerUnit.currentTile)
                        determineDangerousTiles(tile, true, playerUnit.range, dangerousTiles);
                    else
                        determineDangerousTiles(tile, false, playerUnit.range, dangerousTiles);
                }
                tileManager.resetMovementTiles();
            }
        }
        tileManager.setDangerousTiles(dangerousTiles);
    }

    /**
     * Will determine which tiles spotted units can attack so the AI will avoid them if necessary
     * 
     * currentTile is the tile the the attack range of the player unit is being determined from
     * playerTile indicates if the AI thinks the player is on this tile. This will be used to know if it is walking into immediate attack
     *            range of the playerUnit or if it is possible for the unit to move and attack the AI's unit. The AI is more likely to avoid
     *            walking into immediate attack range than somewhere that the player could potentailly move to and attack.
     **/
    public void determineDangerousTiles(MapTile currentTile, bool playerTile, int range, ArrayList dangerousTiles)
    {
        ArrayList temp = new ArrayList();
        if (currentTile.getLeft() != null)
        {
            findDangerousTiles(currentTile.getLeft(), range - 1, dangerousTiles, playerTile, temp);
        }
        if (currentTile.getDown() != null)
        {
            findDangerousTiles(currentTile.getDown(), range - 1, dangerousTiles, playerTile, temp);
        }
        if (currentTile.getRight() != null)
        {
            findDangerousTiles(currentTile.getRight(), range - 1, dangerousTiles, playerTile, temp);
        }
        if (currentTile.getUp() != null)
        {
            findDangerousTiles(currentTile.getUp(), range - 1, dangerousTiles, playerTile, temp);
        }

        foreach(MapTile tile in temp)
        {
            tile.resetAttackTile();
        }
    }

    public void findDangerousTiles(MapTile curTile, int range, ArrayList dangerousTiles, bool playerTile, ArrayList temp)
    {
        if (range < 0)
        {
            return;
        }
        if (playerTile)
            curTile.setInAttackRange(true);
        else
            curTile.setInPossibleAttackRange(true);

        temp.Add(curTile);
        curTile.setStoredRange(range);
        //if (!dangerousTiles.Contains(curTile))
        dangerousTiles.Add(curTile);


        if (curTile.getLeft() != null && !tileManager.containsDangerousTile(range - 1, curTile.getLeft(), playerTile))
        {
            findDangerousTiles(curTile.getLeft(), range - 1, dangerousTiles, playerTile, temp);
        }
        if (curTile.getDown() != null && !tileManager.containsDangerousTile(range - 1, curTile.getDown(), playerTile))
        {
            findDangerousTiles(curTile.getDown(), range - 1, dangerousTiles, playerTile, temp);
        }
        if (curTile.getRight() != null && !tileManager.containsDangerousTile(range - 1, curTile.getRight(), playerTile))
        {
            findDangerousTiles(curTile.getRight(), range - 1, dangerousTiles, playerTile, temp);
        }
        if (curTile.getUp() != null && !tileManager.containsDangerousTile(range - 1, curTile.getUp(), playerTile))
        {
            findDangerousTiles(curTile.getUp(), range - 1, dangerousTiles, playerTile, temp);
        }
    }

    /**
     * Moves the aiUnit towards the playerUnit
     */
    private IEnumerator move(UnitClass aiUnit, MapTile dest)
    {
        aiUnit.displayMovementPath();
        int distance = int.MaxValue;
        
        ArrayList pathToUnit = FindPathToUnit(aiUnit.currentTile, dest);
        ArrayList movementPath = new ArrayList();
        for(int i = aiUnit.speed; i > 0; i--)
        {
            if (pathToUnit.Count - i >= 0)
            {
                MapTile tile = (MapTile)pathToUnit[pathToUnit.Count - i];
                movementPath.Add(pathToUnit[pathToUnit.Count - i]);
            }
        }
        dest = (MapTile)movementPath[0];
        if (dest.currentUnit != null)
        {
            movementPath.Remove(dest);
            while (movementPath.Count > 0)
            {
                dest = (MapTile)movementPath[0];
                if (dest.currentUnit == null)
                    break;
                movementPath.Remove(dest);
            }
        }
        aiUnit.MoveTo(movementPath, dest);
        dest.currentUnit = aiUnit;
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

        MapTile targetTile = null;
        List<MapTile> adjacentTiles = player.getAdjacentTiles();

        int min = int.MaxValue;
        foreach (MapTile tile in player.getAdjacentTiles())
        {
            if (!tile.tileType.Equals("Building"))
            {
                if (ManhattanDistance(ai.gameObject, tile.gameObject) < min && tile.currentUnit == null)
                {
                    min = ManhattanDistance(ai.gameObject, tile.gameObject);
                    targetTile = tile;
                }
            }
        }

        if (targetTile == null)
        {
            MapTile closest = findClosestAdjacentTile(ai, player);
            while (targetTile == null)
            {
                targetTile = findClosestOpenAdjacentTile(ai, closest);
                closest = findClosestAdjacentTile(ai, closest);
            }
        }

        return targetTile;
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
    // Method shouldn't be needed anymore
    /*private UnitClass findNearestPlayerUnit(UnitClass aiUnit)
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
    }*/

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

    public class Objective : IComparable<Objective>
    {
        public MapTile tile;
        public int value;
        public bool attack;
        public UnitClass unit;
        public Objective(MapTile t, int v)
        {
            tile = t;
            value = v;
        }

        public int CompareTo(Objective other)
        {
            if (value > other.value)
                return 1;
            else
                return -1;
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

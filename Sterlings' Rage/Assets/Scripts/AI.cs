using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private bool aiTurn = false;
    private bool test = false;
    private bool endThreads = false;
    private ArrayList objectives;
    private List<MapTile> patrolTiles = new List<MapTile>();
    private Thread objectiveBuilder;
    private Thread aiTurnTaker;
    private TurnManager turnManager;
    private TileManager tileManager;
    private UnitManager unitManager;
    System.Random random = new System.Random();


    private static int inAttckRangeValue = 16;
    private static int inPossibleAttackRangeValue = 12;
    private static int acquireContrabandValue = -8;
    private static int attackUnitValue = -32;
    private static int killUnitValue = -60;
    private static int dyingValue = 50;
    private static int unitViewDistance = 10;
    private static int thresholdBetweenUnits = 4;
    private static int safteyInNumbersValue = -3;
    private static int closeThreshold = 5;
    private static int closeObjectiveValue = -5;
    private static int tooFarDist = 15;
    private static int tooFarDistValue = 10;

    private bool debug = false;

    void OnApplicationQuit()
    {
        endThreads = true;
    }

    // Use this for initialization
    void Start()
    {
        //objectives = new List<KeyValuePair<MapTile, string>>();
        objectives = new ArrayList();
        objectiveBuilder = new Thread(objectiveListHandler);
        objectiveBuilder.Start();

        unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
        tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            print(objectives.Count);
            foreach (KeyValuePair<MapTile, object> cur in objectives)
            {
                print(cur);
                if (cur.Value == null)
                    print("I'm confused...");
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            debug = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            debug = false;
        }
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
        while (!endThreads)
        {
            try
            {
                //print("still running");
                foreach (UnitClass unit in unitManager.EnemyUnits)
                {
                    if (unit.currentTile == null)
                        continue;
                    ArrayList foundObectives = new ArrayList();
                    for (int rangeX = -unitViewDistance; rangeX <= unitViewDistance; rangeX++)
                    {
                        for (int rangeY = -unitViewDistance; rangeY <= unitViewDistance; rangeY++)
                        {
                            if (Mathf.Abs(rangeX) + Mathf.Abs(rangeY) <= unitViewDistance)
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
                                    if (obj.Value != null)
                                        objectives.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
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
                if (obj.Value == unit)
                {
                    objectives.Remove(obj);
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
                    break;
                }
            }
        }
    }

    public void remove(KeyValuePair<MapTile, object> obj)
    {
        lock (objectives)
        {
            objectives.Remove(obj);
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
                if (debug)
                    print(next.tile + "  :  " + next.value);
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
        ArrayList tempList = new ArrayList();
        PriorityQueue<Objective> queue = new PriorityQueue<Objective>();
        // lock makes sure the objective list isn't being written over as it is being checked
        if (objectives.Count > 0)
        {
            lock (objectives)
            {
                foreach (KeyValuePair<MapTile, object> obj in objectives)
                {
                    if (obj.Value != null)
                        tempList.Add(obj);
                }
            }
        }
        else
        {
            if (unit.CurrentPatrolTile == null)
                setCurrentPatrolTileClosest(unit);
            if (unit.currentTile == unit.CurrentPatrolTile)
                setNewPatrolTile(unit);
            ArrayList pathToObj = FindPathToUnit(unit.currentTile, unit.CurrentPatrolTile);
            ArrayList movementPath = new ArrayList();
            for (int i = unit.speed; i > 0; i--)
            {
                if (pathToObj.Count - i >= 0)
                {
                    movementPath.Add(pathToObj[pathToObj.Count - i]);
                }
            }
            int count = 0;
            foreach (MapTile tile in movementPath)
            {
                queue.Push(DetermineObjectiveValue(tile, unit, count--));
            }
        }
        setDangerousTiles(tempList);
        unit.displayMovementPath();

        
        
        foreach (KeyValuePair<MapTile, object> obj in tempList)
        {
            Contraband contraband;
            UnitClass objUnit;
            ArrayList pathToObj;
            ArrayList movementPath = new ArrayList();
            bool notUnit = false;
            bool notContraband = false;
            
            // Using the keyValuePairs to store different types made a couple problems so it tries to cast
            // to a unit and if that fails tries casting to contraband. So if both fail should remove the objective
            // from the list if it failed to be removed before
            try
            {
                
                objUnit = (UnitClass)obj.Value;
                
                // Unit is no longer there
                if (ManhattanDistance(unit.gameObject, obj.Key.gameObject) <= 2 && obj.Key.currentUnit == null)
                {
                    notContraband = true;
                    notUnit = true;
                    setCurrentPatrolTileClosest(unit);
                }
                if (!unit.UnitClassName.Equals("MedicUnit"))
                {


                    pathToObj = FindPathToUnit(unit.currentTile, findClosestAdjacentTile(unit.currentTile, obj.Key));
                    queue.Push(DetermineObjectiveValue(unit.currentTile, unit, objUnit, 0));

                    for (int i = unit.speed; i > 0; i--)
                    {
                        if (pathToObj.Count - i >= 0)
                        {
                            movementPath.Add(pathToObj[pathToObj.Count - i]);
                        }
                    }
                    int closerToObj = -movementPath.Count;
                    foreach (MapTile curTile in movementPath)
                    {
                        if (debug)
                        {
                            print("movement: " + curTile + "  :  " + closerToObj);
                        }

                        queue.Push(DetermineObjectiveValue(curTile, unit, objUnit, closerToObj));
                        closerToObj += 1;
                    }
                }

            }
            catch (Exception e)
            {
                print("not unit");
                notUnit = true;
            }
            try
            {
                if (!notContraband)
                {
                    contraband = (Contraband)obj.Value;

                    // Wierd case where the contraband object still exists after the gameObject was destroyed
                    // So calling this throws an expection that is caught so that it will be removed from the objetive list
                    GameObject test = contraband.gameObject;
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
                        closerToObj--;
                    }
                }
            }
            catch (Exception e)
            {
                notContraband = true;
            }
            if (notUnit && notContraband)
            {
                remove(obj);
            }
        }
        if (unit.UnitClassName.Equals("MedicUnit"))
        {
            foreach(UnitClass ally in unitManager.EnemyUnits)
            {
                ArrayList pathToObj = FindPathToUnit(unit.currentTile, findClosestAdjacentTile(unit.currentTile, ally.currentTile));
                ArrayList movementPath = new ArrayList();
                int closerToAlly = 0;
                for (int i = unit.speed; i > 0; i--)
                {
                    if (pathToObj.Count - i >= 0)
                    {
                        movementPath.Add(pathToObj[pathToObj.Count - i]);
                    }
                }

                foreach (MapTile curTile in movementPath)
                {
                    queue.Push(DetermineObjectiveValueForMedic(curTile, unit, ally, closerToAlly));
                    closerToAlly -= 3;
                }
            }
        }

        foreach (MapTile tile in tileManager.getPathList())
        {
            queue.Push(DetermineObjectiveValue(tile, unit, 10));
        }
        Objective ret = queue.Pop();
        if (ret.tile.currentUnit != unit)
        {
            while (ret.tile.currentUnit != null)
            {
                ret = queue.Pop();
            }
        }
        tileManager.resetDangerousTiles();
        return ret;
    }

    /**
     * Determins the value when the AI unit is heading towards an opposing unit
     **/
    private Objective DetermineObjectiveValue(MapTile tile, UnitClass aiUnit, UnitClass playerUnit, int closerTo)
    {
        if (debug)
            print(tile + "  :  " + tile.isInAttackRange() + "," + tile.isInPossibleAttackRange());
        Objective ret = new Objective(tile, 0);
        int value = 0;
        value += determineDangerousTilesValue(tile, aiUnit);
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
        if (ManhattanDistance(playerUnit.gameObject, tile.gameObject) < ManhattanDistance(aiUnit.gameObject, playerUnit.gameObject))
            value += closerTo;
        value += determineMoveTowards(tile.gameObject, playerUnit.gameObject); 
        value += allyUnitsInRange(aiUnit);
        ret.value = value;
        return ret;
    }

    /** Determines the value if the objective tile has contraband
     **/
    private Objective DetermineObjectiveValue(MapTile tile, UnitClass aiUnit, Contraband contraband, int closerTo)
    {
        int value = 0;

        value += determineDangerousTilesValue(tile, aiUnit);
        if (contraband.currentTile == tile)
            value += acquireContrabandValue;
        value += 2 * closerTo;
        value += allyUnitsInRange(aiUnit);
        value += determineMoveTowards(tile.gameObject, contraband.gameObject);
        Objective ret = new Objective(tile, value);
        return ret;
    }

    private Objective DetermineObjectiveValue(MapTile tile, UnitClass aiUnit, int initalValue)
    {
        int value = initalValue;
        value += determineDangerousTilesValue(tile, aiUnit);
        value -= ManhattanDistance(tile.gameObject, aiUnit.gameObject);
        //print("Here: " + tile + ", " + value);
        value += allyUnitsInRange(aiUnit);
        return new Objective(tile, value);
    }

    private Objective DetermineObjectiveValueForMedic(MapTile tile, UnitClass unit, UnitClass ally, int initialValue)
    {
        int value = initialValue;
        value += determineDangerousTilesValue(tile, unit);
        value += allyUnitsInRange(unit);
        if (ManhattanDistance(tile.gameObject, ally.gameObject) <= unit.range)
            value += ((1 / ally.health) * 30);
        Objective ret = new Objective(tile, value);
        ret.attack = true;
        ret.unit = ally;
        return ret;
    }

    private void setDangerousTiles(ArrayList objList)
    {
        ArrayList dangerousTiles = new ArrayList();
        foreach (KeyValuePair<MapTile, object> obj in objList)
        {
            try
            {
                UnitClass playerUnit = (UnitClass)obj.Value;
                playerUnit.displayMovementPathForAI(playerUnit.speed);
                determineDangerousTiles(playerUnit.currentTile, true, playerUnit.range, dangerousTiles, playerUnit);
                foreach (MapTile tile in tileManager.getPathList())
                {
                    if (tile != playerUnit.currentTile)
                        determineDangerousTiles(tile, false, playerUnit.range, dangerousTiles,playerUnit);
                }
                tileManager.resetMovementTiles();
            }
            catch (Exception e) { }
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
    public void determineDangerousTiles(MapTile currentTile, bool playerTile, int range, ArrayList dangerousTiles, UnitClass playerUnit)
    {
        ArrayList temp = new ArrayList();
        if (currentTile.getLeft() != null)
        {
            findDangerousTiles(currentTile.getLeft(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
        if (currentTile.getDown() != null)
        {
            findDangerousTiles(currentTile.getDown(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
        if (currentTile.getRight() != null)
        {
            findDangerousTiles(currentTile.getRight(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
        if (currentTile.getUp() != null)
        {
            findDangerousTiles(currentTile.getUp(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }

        foreach (MapTile tile in temp)
        {
            tile.resetAttackTile();
        }
    }

    public void findDangerousTiles(MapTile curTile, int range, ArrayList dangerousTiles, bool playerTile, ArrayList temp, UnitClass playerUnit)
    {
        if (range < 0)
        {
            return;
        }
        if (playerTile)
            curTile.setInAttackRange(true, playerUnit);
        else
            curTile.setInPossibleAttackRange(true, playerUnit);

        temp.Add(curTile);
        curTile.setStoredRange(range);
        dangerousTiles.Add(curTile);


        if (curTile.getLeft() != null && !tileManager.containsDangerousTile(range - 1, curTile.getLeft(), playerTile, playerUnit))
        {
            findDangerousTiles(curTile.getLeft(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
        if (curTile.getDown() != null && !tileManager.containsDangerousTile(range - 1, curTile.getDown(), playerTile, playerUnit))
        {
            findDangerousTiles(curTile.getDown(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
        if (curTile.getRight() != null && !tileManager.containsDangerousTile(range - 1, curTile.getRight(), playerTile, playerUnit))
        {
            findDangerousTiles(curTile.getRight(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
        if (curTile.getUp() != null && !tileManager.containsDangerousTile(range - 1, curTile.getUp(), playerTile, playerUnit))
        {
            findDangerousTiles(curTile.getUp(), range - 1, dangerousTiles, playerTile, temp, playerUnit);
        }
    }

    /**
     * Finds units within a specified range as the AI units will prefer saftey in numbers
     **/
    private int allyUnitsInRange(UnitClass aiUnit)
    {
        int ret = 0;
        foreach (UnitClass unit in unitManager.EnemyUnits)
        {
            if (aiUnit != unit && ManhattanDistance(aiUnit.gameObject, unit.gameObject) < thresholdBetweenUnits)
                ret -= safteyInNumbersValue;
        }
        return ret;
    }

    /**
     * calculates a value based on how many units can attack the tile
     **/
    private int determineDangerousTilesValue(MapTile tile, UnitClass aiUnit)
    {
        int ret = 0;
        foreach (UnitClass unit in tile.getInAttackRangeUnits())
        {
            print(unit);
            ret += inAttckRangeValue;
            if (unit.damage >= aiUnit.health)
                ret += dyingValue;
        }
        
        foreach (UnitClass unit in tile.getInPossibleAttackRangeUnits())
        {
            ret += inPossibleAttackRangeValue;
            if (unit.damage >= aiUnit.health)
                ret += dyingValue/2;
        }
        return ret;
    }

    private int determineMoveTowards(GameObject tile, GameObject objective)
    {
        int ret = 0;
        if (ManhattanDistance(tile, objective) > tooFarDist)
            ret += tooFarDistValue;
        else if (ManhattanDistance(tile, objective) < closeThreshold)
            ret += closeObjectiveValue;
        return ret;

    }

    /**
     * Moves the aiUnit towards the destination tile
     */
    private IEnumerator move(UnitClass aiUnit, MapTile dest)
    {
        aiUnit.displayMovementPath();
        int distance = int.MaxValue;

        ArrayList pathToUnit = FindPathToUnit(aiUnit.currentTile, dest);
        ArrayList movementPath = new ArrayList();
        for (int i = aiUnit.speed; i > 0; i--)
        {
            if (pathToUnit.Count - i >= 0)
            {
                MapTile tile = (MapTile)pathToUnit[pathToUnit.Count - i];
                movementPath.Add(pathToUnit[pathToUnit.Count - i]);
            }
        }
        dest = (MapTile)movementPath[0];

        /* if (dest.currentUnit != null)
         {
             movementPath.Remove(dest);
             while (movementPath.Count > 0)
             {
                 dest = (MapTile)movementPath[0];
                 if (dest.currentUnit == null)
                     break;
                 movementPath.Remove(dest);
             }
         }*/
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
        if (startTile == endTile)
        {
            visited.Add(startTile);
            return visited;
        }
        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        Node start = new Node(startTile, null, 0);
        queue.Push(start);
        int currentPathLength = 0;
        while (true)
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
                        if (tile == null)
                            print("grrrr");
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
        while (start.tile != startTile)
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

    /**
     * Sets the current patrol tile for the unit to be the closest designated patrol tile
     **/
    private void setCurrentPatrolTileClosest(UnitClass unit)
    {
        int min = int.MaxValue;
        foreach (MapTile tile in patrolTiles)
        {
            int dis = ManhattanDistance(unit.gameObject, tile.gameObject);
            if (dis < min)
            {
                min = dis;
                unit.CurrentPatrolTile = tile;
            }
        }
    }

    /**
     * After the unit reaches their patrol tile it picks a random one as the next
     **/
    private void setNewPatrolTile(UnitClass unit)
    {

        int pos = random.Next(patrolTiles.Count);
        MapTile tile = patrolTiles[pos];
        while (tile == unit.CurrentPatrolTile && patrolTiles.Count > 1)
        {
            pos = random.Next(patrolTiles.Count);
            tile = patrolTiles[pos];
        }
        unit.CurrentPatrolTile = tile;

    }

    private float distanceBetween(GameObject unit, GameObject otherUnit)
    {
        return Mathf.Sqrt(Mathf.Pow((unit.transform.position.x - otherUnit.transform.position.x), 2f)
            + Mathf.Pow((unit.transform.position.y - otherUnit.transform.position.y), 2f));
    }

    public void addToPatrolList(MapTile tile)
    {
        patrolTiles.Add(tile);
    }


    public class PriorityQueue<T> where T : IComparable<T>
    {

        private List<T> data;

        public PriorityQueue()
        {
            data = new List<T>();
        }

        public void Push(T item)
        {
            data.Add(item);

            int child = data.Count - 1;

            while (child > 0)
            {
                int parent = ((child - 1) / 2);

                if (data[parent].CompareTo(data[child]) < 0)
                    break;
                T temp = data[parent];
                data[parent] = data[child];
                data[child] = temp;
                child = parent;
            }
        }

        public T Pop()
        {
            T ret = data[0];
            data[0] = data[data.Count - 1];
            data.RemoveAt(data.Count - 1);
            int index = 0;

            int last = data.Count - 1;
            while (true)
            {
                int leftChild = index * 2 + 1;
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
            return "" + value;
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

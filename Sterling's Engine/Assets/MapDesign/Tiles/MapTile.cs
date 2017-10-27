using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{

    private MapTile up;
    private MapTile down;
    private MapTile right;
    private MapTile left;
    public string tileType;
    private int movementWeight;
    private bool hasUnit = false;
    private bool mouseOverTest = true;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(initializeGrid());
        movementWeight = determineMovementWeight(tileType);
    }

    private IEnumerator initializeGrid()
    {
        yield return new WaitWhile(() => TileManager.mapTiles == null);
        TileManager.addTile(this, (int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        if (mouseOverTest)
        {
            int x = (int)gameObject.transform.position.x;
            int y = (int)gameObject.transform.position.y;
            print("at" + x + ", " + y + " with weight" + movementWeight);
            print("right: " + right);
            print("up: " + up);
            print("down: " + down);
            print("left: " + left);
            mouseOverTest = false;
        }
    }
    /**
     * Determines the movement weight based on the tile type
     */
    private int determineMovementWeight(string type)
    {
        switch (type)
        {
            case "Building":
                return int.MaxValue;
            case "Road":
                return 1;
            default:
                return 1;
        }

    }

    public void setHasUnit(bool value)
    {
        hasUnit = value;
    }

    public void setRight(MapTile tile)
    {
        right = tile;
    }

    public void setUp(MapTile tile)
    {
        up = tile;
    }

    public void setDown(MapTile tile)
    {
        down = tile;
    }

    public void setLeft(MapTile tile)
    {
        left = tile;
    }

}

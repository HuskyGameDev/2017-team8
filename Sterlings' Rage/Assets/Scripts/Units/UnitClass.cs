﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UnitClass : MonoBehaviour
{
	public string unitClassName;
	public string unitType;
	private TileManager tileManager;
	private UnitManager unitManager;
	private MapTile currentPatrolTile;
	private int xPos;
	private int yPos;

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
	private int speedLeft;
	public int checkHealth;
	private int maxHealth;
	public bool alreadyAttacked;
	private bool isMovingX;
	private bool isMovingY;
	public bool moving;
	public bool playerUnit;
	public bool spotted;
	public int damageCount;
	GameObject DMGtext = null;
	public float fade = 3f;

	public string UnitClassName
	{
		get { return unitClassName; }
		set { unitClassName = value; }
	}
	public string UnitType
	{
		get { return unitType; }
		set { unitType = value; }
	}

	public string Sprite
	{
		get { return sprite; }
		set { sprite = value; }
	}

	public int Health
	{
		get { return health; }
		set { health = value; }
	}

	public int Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	public int Range
	{
		get { return range; }
		set { range = value; }
	}

	public int Damage
	{
		get { return damage; }
		set { damage = value; }
	}

	public int Cost
	{
		get { return cost; }
		set { cost = value; }
	}

	public MapTile CurrentTile
	{
		get { return currentTile; }
		set { currentTile = value; }
	}

	public MapTile CurrentPatrolTile
	{
		get { return currentPatrolTile; }
		set { currentPatrolTile = value; }
	}

	public int getSpeedLeft()
	{
		return speedLeft;
	}

	public void newTurn()
	{
		speedLeft = speed;
		alreadyAttacked = false;
		//Make the unit indicate they can now move this turn
		try
		{
			Transform unit = this.transform;
			for (int i = 0; i < unit.childCount; i++)
			{
				GameObject indicator = unit.GetChild(i).gameObject;
				if (indicator != null && indicator.name == "movedIndicator") indicator.SetActive(false);
			}

		}
		catch (Exception e)
		{

		}
	}

	public void DisplayAttackRange()
	{
		if (!alreadyAttacked)
		{
			ArrayList attackTiles = new ArrayList();
			tileManager.resetMovementTiles();

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
			tileManager.setAttackList(attackTiles);
		}
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


		if (curTile.getLeft() != null && !tileManager.containsAttackTile(range - 1, curTile.getLeft()))
		{
			findAttackTiles(curTile.getLeft(), range - 1, attackTiles);
		}
		if (curTile.getDown() != null && !tileManager.containsAttackTile(range - 1, curTile.getDown()))
		{
			findAttackTiles(curTile.getDown(), range - 1, attackTiles);
		}
		if (curTile.getRight() != null && !tileManager.containsAttackTile(range - 1, curTile.getRight()))
		{
			findAttackTiles(curTile.getRight(), range - 1, attackTiles);
		}
		if (curTile.getUp() != null && !tileManager.containsAttackTile(range - 1, curTile.getUp()))
		{
			findAttackTiles(curTile.getUp(), range - 1, attackTiles);
		}
	}

    public void displayMovementPath()
    {
        // Resets if the user had already selected a different unit
        if (unitManager.getSelectedUnit() != null)
            tileManager.resetMovementTiles();
        unitManager.setSelectedUnit(this);

        ArrayList pathTiles = new ArrayList();

        if (currentTile.getLeft() != null)
        {
            findPossiblePaths(currentTile.getLeft(), speedLeft - currentTile.getLeft().getMovementWeight(), pathTiles, currentTile);
        }
        if (currentTile.getDown() != null)
        {
            findPossiblePaths(currentTile.getDown(), speedLeft - currentTile.getDown().getMovementWeight(), pathTiles, currentTile);
        }
        if (currentTile.getRight() != null)
        {
            findPossiblePaths(currentTile.getRight(), speedLeft - currentTile.getRight().getMovementWeight(), pathTiles, currentTile);
        }
        if (currentTile.getUp() != null)
        {
            findPossiblePaths(currentTile.getUp(), speedLeft - currentTile.getUp().getMovementWeight(), pathTiles, currentTile);
        }
        tileManager.setPathList(pathTiles);
    }

    /**
     * Needed for the AI to better calculate the dangerous tiles
     **/
    public void displayMovementPathForAI(int speed)
    {
        // Resets if the user had already selected a different unit
        if (unitManager.getSelectedUnit() != null)
            tileManager.resetMovementTiles();
        unitManager.setSelectedUnit(this);

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
        tileManager.setPathList(pathTiles);
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
        if (curTile.getLeft() != null && !tileManager.containsTile(speed - curTile.getLeft().getMovementWeight(), curTile.getLeft()))
            findPossiblePaths(curTile.getLeft(), speed - curTile.getLeft().getMovementWeight(), pathTiles, curTile);

        if (curTile.getDown() != null && !tileManager.containsTile(speed - curTile.getDown().getMovementWeight(), curTile.getDown()))
            findPossiblePaths(curTile.getDown(), speed - curTile.getDown().getMovementWeight(), pathTiles, curTile);

        if (curTile.getRight() != null && !tileManager.containsTile(speed - curTile.getRight().getMovementWeight(), curTile.getRight()))
            findPossiblePaths(curTile.getRight(), speed - curTile.getRight().getMovementWeight(), pathTiles, curTile);

        if (curTile.getUp() != null && !tileManager.containsTile(speed - curTile.getUp().getMovementWeight(), curTile.getUp()))
            findPossiblePaths(curTile.getUp(), speed - curTile.getUp().getMovementWeight(), pathTiles, curTile);
    }

	public GameObject EnemyInRange(float range)
	{
		GameObject[] enemies;
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		//print("Enemies found " + enemies.Length);
		GameObject closest = null;
		float distance = range;
		//print("targeting range is " + distance);
		Vector3 pos = transform.position;
		//print("Pos is " + pos);
		foreach (GameObject go in enemies)
		{
			Vector3 diff = go.transform.position - pos;
			//print("Diff is " + diff);
			float curDistance = diff.sqrMagnitude - 1;
			//print("curDistance  is " + curDistance);
			if (curDistance <= distance)
			{
				//print("target in range");
				//print("GO is " + go);
				closest = go;
				distance = curDistance;
			}
		}
		//print("Closest is " + closest);
		return closest;
	}

	public void MoveTo(ArrayList movementPath, MapTile dest)
	{
		if (path == null)
			path = new ArrayList();
		path.Clear();
		// Have to add to the path for this unit because the tile will clear it's movement path 
		// before it is able to travel there
		for (int i = movementPath.Count - 1; i >= 0; i--)
		{
			MapTile tile = (MapTile)movementPath[i];
			path.Add(movementPath[i]);
		}
		path.Add(dest);
		this.dest = (MapTile)path[0];
		path.Remove(this.dest);
		currentTile.currentUnit = null;
		currentTile = dest;
		isMovingX = true;
		moving = true;
		speedLeft = speedLeft - this.dest.getMovementWeight();
	}

	public void Update()
	{
		if (DMGtext != null)
		{
			fade -= Time.deltaTime;
			if (fade < 0)
			{
				Text text = DMGtext.GetComponent<Text>();
				text.text = "";
				fade =  3f;
			}
		}

		if (maxHealth == 0 && health != 0)
		{
			checkHealth = health;
			maxHealth = health;
		}
		if (unitManager == null || tileManager == null)
		{
			unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
			tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
		}
		//check if unit should be moving
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
				if (dest.contraband != null)
				{
					ResourceManager.pickUpContraband(dest);
				}
				if (path.Count > 0)
				{
					dest = (MapTile)path[0];
					speedLeft = speedLeft - dest.getMovementWeight();
					path.Remove(dest);
					isMovingX = true;
				}
				else
				{
					moving = false;
					//Make the unit indicate they have moved and cannot move until next turn
					try
					{
						Transform unit = this.transform;
						for (int i = 0; i < unit.childCount; i++)
						{

							GameObject indicator = unit.GetChild(i).gameObject;
							if (indicator != null && indicator.name == "movedIndicator") indicator.SetActive(true);
						}

					}
					catch (Exception e)
					{

					}

				}
			}
		}

		//check if unit took damage
		if (checkHealth != health && maxHealth != 0 && health > 0)
		{

			//calculate width based on health
			float width = 0.95f * ((float)health / maxHealth);

			//find canvas object and resize to reflect current health
			try
			{
				Transform unit = this.transform;
				GameObject theBar = null;
				foreach (Transform child in unit.GetComponentsInChildren<Transform>())
				{
					GameObject indicator = child.gameObject;
					if (indicator != null && indicator.name == "health") theBar = indicator;
				}


				var theBarRectTransform = theBar.transform as RectTransform;

				theBarRectTransform.sizeDelta = new Vector2(width, theBarRectTransform.sizeDelta.y);

			}
			catch (Exception e)
			{

			}
			//find canvas object and display text of damage taken
			try
			{
				Transform unit = this.transform;
				
				foreach (Transform child in unit.GetComponentsInChildren<Transform>())
				{
					GameObject indicator = child.gameObject;
					if (indicator != null && indicator.name == "damage") DMGtext = indicator;
				}

				
				Text text = DMGtext.GetComponent<Text>();
				text.text = "DMG: "+ (checkHealth - health);

			}
			catch (Exception e)
			{
				Debug.Log(e.StackTrace);
				if (DMGtext == null)Debug.Log("was null");
			}

			checkHealth = health;
		}
	}

	private IEnumerator AddToTile()
	{
		yield return new WaitWhile(() => !tileManager.instantiated || !tileManager.checkIfFull());
		print("this was ran" + tileManager.checkIfFull());
		currentTile = tileManager.mapTiles[xPos, yPos];
		currentTile.currentUnit = this;
		gameObject.transform.position = new Vector2(xPos, yPos);

		if (unitManager.unitsAssignedTiles())
			tileManager.mapTiles[xPos, yPos].tileManager.resetAllTiles();
	}


	public void Start()
	{
		tileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
		unitManager = GameObject.Find("GameManager").GetComponent<UnitManager>();
		xPos = (int)(.5 + gameObject.transform.position.x);
		yPos = (int)(.5 + gameObject.transform.position.y);
		isMovingY = false;
		isMovingY = false;
		dest = null;
		damageCount = 0;
		print(gameObject.tag);
		playerUnit = gameObject.tag.Equals("PlayerUnit");
		spotted = false;
		if (currentTile == null || currentTile.currentUnit == null)
		{
			StartCoroutine(AddToTile());
		}
	}

}

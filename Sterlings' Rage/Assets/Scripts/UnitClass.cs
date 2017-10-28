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

	public string UnitClassName{
		get{return unitClassName;}
		set{unitClassName = value;}
	}

	public string Sprite{
		get{return sprite;}
		set{sprite = value;}
	}

	public int Health{
		get{return health;}
		set{health = value;}
	}

	public int Speed{
		get{return speed;}
		set{speed = value;}
	}
	
	public int Range{
		get{return range;}
		set{range = value;}
	}
	
	public int Damage{
		get{return damage;}
		set{damage = value;}
	}

	public int Cost{
		get{return cost;}
		set{cost = value;}
	}
	
}

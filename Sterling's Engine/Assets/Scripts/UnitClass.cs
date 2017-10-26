using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClass {
	private string unitClassName;

	// Stats
	private int Health;
	private int Speed;
	private int Range;
	private int Damage;
	private string Sprite;
	private int Cost;

	public string unitClassName{
		get{return unitClassName;}
		set{unitClassName = value;}
	}

	public string Sprite{
		get{return Sprite;}
		set{Sprite = value;}
	}

	public int Health{
		get{return Health;}
		set{Health = value;}
	}

	public int Speed{
		get{return Speed;}
		set{Speed = value;}
	}
	
	public int Range{
		get{return Range;}
		set{Range = value;}
	}
	
	public int Damage{
		get{return Damage;}
		set{Damage = value;}
	}

	public int Cost{
		get{return Cost;}
		set{Cost = value;}
	}
	
}

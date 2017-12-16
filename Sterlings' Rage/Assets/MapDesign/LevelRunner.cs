using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRunner : MonoBehaviour {
    public UnitClass boss;
    public string objective;
    public int numUnits;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (objective.Equals("defeat boss"))
        {
            if (boss.health <= 0)
            {
                print("Level Cleared");
            }
        }
        else if (objective.Equals("defeat all"))
        {
            if (UnitManager.EnemyUnits.Count == 0)
            {
                print("Level Cleared");
            }
        }
        else
        {
            //add more objectives if necessary
        }
    }
}

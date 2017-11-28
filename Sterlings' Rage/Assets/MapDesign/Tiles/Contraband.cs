using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contraband : MonoBehaviour {

    public MapTile currentTile;
    public int value;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Remove()
    {
        Destroy(gameObject);
    }
}

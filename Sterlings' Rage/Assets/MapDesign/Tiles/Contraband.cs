using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contraband : MonoBehaviour {

    public MapTile currentTile;
    public int value;
    private AI ai;

    // Use this for initialization
    void Start()
    {
        ai = GameObject.Find("AI").GetComponent<AI>();
    }

    void Awake()
    {
        ai = GameObject.Find("AI").GetComponent<AI>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Remove()
    {
        ai.remove(this);
        Destroy(gameObject);
    }
}

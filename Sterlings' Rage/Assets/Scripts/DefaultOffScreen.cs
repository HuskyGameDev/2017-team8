using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DefaultOffScreen : MonoBehaviour
{
    public GameObject AttachedTo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//not for UI
	private void OnMouseOver()
	{
		
		
	}
	//Not for UI
	private void OnMouseExit()
	{
		//Place it outside the UI window, might be good to make sure it isn't visible either, to lower the amount of drawing required.
		
		print("Hello I'm out");
	}

    public void EventMouseExit()
    {
        print("Hello");
        Vector3 temp = new Vector3(-1000, 0, 0);
        transform.localPosition = temp;
    }
}

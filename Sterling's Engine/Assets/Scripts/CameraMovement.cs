using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    //Speed is the rate at which the camera moves. Editable in the Script Component Window
    public int speed;
    public int scrollZone;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //For now all that is done is moving the camer via the arrow keys.
        //For now, relegating the movement to x or y at a time, not both. Will prioritize the keypresses.
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
		}else if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
		}else if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
		}else if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
		}

        print(Screen.height);
        print(Input.mousePosition.y);
        print(Input.mousePosition.x);

        //edge-scrolling, very unpolished right now
        //left-side of the screen. a 10th of the screen size
        if(Input.mousePosition.x <= Screen.width / 10)
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        //right of the screen
        if (Input.mousePosition.x >= (Screen.width * .9))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        //bottom of the screen
        if (Input.mousePosition.y <= Screen.height / 10)
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        //top of the screen
        if (Input.mousePosition.y >= (Screen.height * .9))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        //This will be the area for camera movement via mouse on edge

    }
}

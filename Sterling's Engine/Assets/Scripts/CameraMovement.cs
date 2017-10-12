using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    //scrollSpeed is the rate at which the camera moves. Editable in the Script Component Window
    public int scrollSpeed = 1;
    //percent of the edge to look at, default is 10%
    public int scrollPercent = 10;
    public int maxZoom = 10;
    public int minZoom = 1;
    public float zoomscrollSpeed = 0.2F;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //For now all that is done is moving the camer via the arrow keys.
        //For now, relegating the movement to x or y at a time, not both. Will prioritize the keypresses.
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Translate(new Vector3(scrollSpeed * Time.deltaTime,0,0));
		}
        if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate(new Vector3(-scrollSpeed * Time.deltaTime,0,0));
		}
        if (Input.GetKey(KeyCode.DownArrow))
		{
			transform.Translate(new Vector3(0,-scrollSpeed * Time.deltaTime,0));
		}
        if (Input.GetKey(KeyCode.UpArrow))
		{
			transform.Translate(new Vector3(0,scrollSpeed * Time.deltaTime,0));
		}

        //print(Screen.height);
        //print(Input.mousePosition.y);
        //print(Input.mousePosition.x);

        //edge-scrolling, very unpolished right now
        //left-side of the screen. a 10th of the screen size
        if(Input.mousePosition.x <= Screen.width / 10)
        {
            transform.Translate(new Vector3(-scrollSpeed * Time.deltaTime, 0, 0));
        }
        //right of the screen
        if (Input.mousePosition.x >= (Screen.width * .9))
        {
            transform.Translate(new Vector3(scrollSpeed * Time.deltaTime, 0, 0));
        }
        //bottom of the screen
        if (Input.mousePosition.y <= Screen.height / 10)
        {
            transform.Translate(new Vector3(0, -scrollSpeed * Time.deltaTime, 0));
        }
        //top of the screen
        if (Input.mousePosition.y >= (Screen.height * .9))
        {
            transform.Translate(new Vector3(0, scrollSpeed * Time.deltaTime, 0));
        }

        //This will be the area for camera zoom
        //Scroll In
        //print(Input.mouseScrollDelta);
        if (Input.GetAxis("Mouse ScrollWheel") < 0){
            Camera.main.orthographicSize = Camera.main.orthographicSize += zoomscrollSpeed;
        }
        //Scroll Out
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize = Camera.main.orthographicSize -= zoomscrollSpeed;
        }

        //This will be for Mousewheel click movement

        if (Input.GetKey(KeyCode.Mouse2))
        {
            //middle mouse is being held, just for proof of concept going to only be increasing the vertical climb.
            transform.Translate(new Vector3(0, scrollSpeed * Time.deltaTime, 0));
        }
    }
}

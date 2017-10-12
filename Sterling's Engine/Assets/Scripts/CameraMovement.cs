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
        int screenCenterX;
        int screenCenterY;
        int mouseX;
        int mouseY;
        float x;
        float y;
        float angle;
        if (Input.GetKey(KeyCode.Mouse2))
        {
            //middle mouse is being held, just for proof of concept going to only be increasing the vertical climb.
            //transform.Translate(new Vector3(0, scrollSpeed * Time.deltaTime, 0));

            // Middle Mouse click will allow the user to use the center of the screen as an edge, however moving faster the farther away from the center you are

            //get the Pixel Middle of the screen., update
            screenCenterX = Screen.width / 2;
            screenCenterY = Screen.height / 2;

            //get the mouse pixel coordinates, if it's less than zero, make it zero. If it's greater than the width, set it to the width

            mouseX = (int) Input.mousePosition.x;
            mouseY = (int) Input.mousePosition.y;

            //get the distance (x,y), constrain the mouse poosition

            if (mouseX < 0)
            {
                mouseX = 0;
            }
            if (mouseY < 0)
            {
                mouseY = 0;
            }
            if (mouseY > Screen.height)
            {
                mouseY = Screen.height;
            }
            if (mouseX > Screen.width)
            {
                mouseX = Screen.width;
            }

            x = screenCenterX - mouseX;
            y = screenCenterY - mouseY;

            //drop down into the respective quadrant, make sure to keep in mind of zeros
            // ScreenCenterX - MouseX: If the mouse is to the right of the center of the screen on the X axis, this will be negative, positive otherwise or in-line
            // ScreenCenterY - MouseY: If the mouse is to the top of the center of the screen on the y axis, this will be negative, positive otherwise or in-line 

            if (x <= 0)
            {
                if (y <= 0)
                {
                    //Upper-Right
                    //Normalize Values, Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                    x = Mathf.Abs(x);
                    y = Mathf.Abs(y);
                    angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                    print(angle + "yo");
                    //print(angle + " " + x + " " + y + " " + Screen.height/2 + " " + Screen.width/2);
                } else
                {
                    //Lower-Right
                    //Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                    x = Mathf.Abs(x);
                    angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                    print(angle + " No");
                    //print(angle + " " + x + " " + y + " " + Screen.height / 2 + " " + Screen.width / 2);
                }
            } else
            {
                if (y <= 0)
                {
                    //Upper-Left
                    //Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                    y = Mathf.Abs(y);
                    angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                    print(angle + "Hello");
                    //print(angle + " " + x + " " + y + " " + Screen.height / 2 + " " + Screen.width / 2);
                } else
                {
                    //Lower-Left
                    //Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                    y = Mathf.Abs(y);
                    angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                    print(angle + "There");
                    //print(angle + " " + x + " " + y + " " + Screen.height / 2 + " " + Screen.width / 2);
                }
            }

        }
    }
}

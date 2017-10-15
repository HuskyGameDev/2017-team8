using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    //scrollSpeed is the rate at which the camera moves. Editable in the Script Component Window
    public int scrollSpeed = 1;
    //percent of the edge to look at, default is 10%
    public float scrollPercent = 10;
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
        
        if(Input.mousePosition.x <= Screen.width * (scrollPercent/100))
        {
            //left-side of the screen
            //transform.Translate(new Vector3(-scrollSpeed * Time.deltaTime, 0, 0));
            //print(Input.mousePosition.x + " Scroll Percent/100:" + scrollPercent/100 + " ScreenW:" + Screen.width + " " + Screen.width * (scrollPercent / 100));
            SmoothMousing(2);
        }
        else if (Input.mousePosition.x >= (Screen.width * (1 - (scrollPercent / 100))))
        {
            //right of the screen
            //transform.Translate(new Vector3(scrollSpeed * Time.deltaTime, 0, 0));
            SmoothMousing(2);
        }
        else if (Input.mousePosition.y <= Screen.height * (scrollPercent / 100))
        {
            //bottom of the screen
            //transform.Translate(new Vector3(0, -scrollSpeed * Time.deltaTime, 0));
            SmoothMousing(2);
        }
        else if (Input.mousePosition.y >= (Screen.height * (1 - (scrollPercent / 100))))
        {
            //top of the screen
            //transform.Translate(new Vector3(0, scrollSpeed * Time.deltaTime, 0));
            SmoothMousing(2);
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
            SmoothMousing(3);
        }
    }
    /*
     * This method functions to adjust the camera translation with more fidelity than using the arrow (or when implemented the wasd keys)
     * 
     * multiplier: multiplies the translation, so you can customize the speed of certaiin methods of scrolling.
     * */
    void SmoothMousing(int multiplier)
    {
        int screenCenterX;
        int screenCenterY;
        int mouseX;
        int mouseY;
        float x;
        float y;
        float angle;
        float changeX;
        float changeY;
        //middle mouse is being held, just for proof of concept going to only be increasing the vertical climb.
        //transform.Translate(new Vector3(0, scrollSpeed * Time.deltaTime, 0));

        // Middle Mouse click will allow the user to use the center of the screen as an edge, however moving faster the farther away from the center you are

        //get the Pixel Middle of the screen., update
        screenCenterX = Screen.width / 2;
        screenCenterY = Screen.height / 2;

        //get the mouse pixel coordinates, if it's less than zero, make it zero. If it's greater than the width, set it to the width

        mouseX = (int)Input.mousePosition.x;
        mouseY = (int)Input.mousePosition.y;

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
                changeX = (90F - angle) / 90F * scrollSpeed * multiplier;
                changeY = angle / 90F * scrollSpeed * multiplier;
                print(angle + "yo" + changeX + " " + changeY);
                //print(angle + " " + x + " " + y + " " + Screen.height/2 + " " + Screen.width/2);
                //now you've got each angle related to the X-axis (could probably make this into a method, and do something else to make this perform better), just need to add to the vector with appropriate +-
                //upper-right, so add X, add Y
                transform.Translate(new Vector3(changeX * Time.deltaTime, changeY * Time.deltaTime, 0));
            }
            else
            {
                //Lower-Right
                //Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                x = Mathf.Abs(x);
                angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                changeX = (90F - angle) / 90F * scrollSpeed * multiplier;
                changeY = angle / 90F * scrollSpeed * multiplier;
                print(angle + " No" + changeX + " " + changeY);
                //print(angle + " " + x + " " + y + " " + Screen.height / 2 + " " + Screen.width / 2);
                //.lower right, so add X, subtract Y
                transform.Translate(new Vector3(changeX * Time.deltaTime, changeY * Time.deltaTime * -1, 0));
            }
        }
        else
        {
            if (y <= 0)
            {
                //Upper-Left
                //Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                y = Mathf.Abs(y);
                angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                changeX = (90F - angle) / 90F * scrollSpeed * multiplier;
                changeY = angle / 90F * scrollSpeed * multiplier;
                print(angle + "Hello" + changeX + " " + changeY);
                //print(angle + " " + x + " " + y + " " + Screen.height / 2 + " " + Screen.width / 2);
                //upper left, so subtract X, add Y
                transform.Translate(new Vector3(changeX * Time.deltaTime * -1, changeY * Time.deltaTime, 0));
            }
            else
            {
                //Lower-Left
                //Get angle from the x-axis, then manipulate the scrollSpeed Accordingly
                y = Mathf.Abs(y);
                angle = Mathf.Rad2Deg * Mathf.Atan2(y, x);
                changeX = (90F - angle) / 90F * scrollSpeed * multiplier;
                changeY = angle / 90F * scrollSpeed * multiplier;
                print(angle + "There" + changeX + " " + changeY);
                //print(angle + " " + x + " " + y + " " + Screen.height / 2 + " " + Screen.width / 2);
                // lower left so subtract X, subtract Y
                transform.Translate(new Vector3(changeX * Time.deltaTime * -1, changeY * Time.deltaTime * -1, 0));
            }
        }
    }
}

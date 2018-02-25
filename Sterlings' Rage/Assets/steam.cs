using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steam : MonoBehaviour {
	public RectTransform IMG;
	public float i;
	public float increament;
	public float scaleMax;
	public float scaleMin;
	public float scale;
	public int directionScale; //1 increasing size, and -1 decreasing size

	// Use this for initialization
	void Start () {
		scaleMax = IMG.sizeDelta.x*2 ;
		scaleMin = IMG.sizeDelta.x;
		scale = Random.Range(20f, 40f);
		directionScale = Random.Range(0,2);
		if (directionScale == 0) directionScale--;
		i = 0;
		increament = Random.Range(.5f,2f);
		directionScale = Random.Range(0,2);
	}

	// Update is called once per frame
	void Update()
	{
		i += increament;
		if ( i > 360 ) i = i - 360;
		IMG.rotation = Quaternion.Euler(0, 0, i);


		if (IMG.sizeDelta.x <= scaleMin)
		{
			directionScale = -1;
		}
		else if (IMG.sizeDelta.x > scaleMax)
		{
			directionScale = 1;
		}


		IMG.sizeDelta = new Vector2(IMG.sizeDelta.x - (scale * Time.deltaTime)* directionScale , IMG.sizeDelta.y - (scale * Time.deltaTime)* directionScale );

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.

public class DefaultOffScreen : EventTrigger // required interface when using the OnPointerEnter method.
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnPointerEnter(PointerEventData eventData){
		print("Hello Pointer");
		Debug.Log("The cursor entered the selectable UI element.");
	}

	public override void OnPointerExit(PointerEventData data)
	{
		Debug.Log("OnPointerExit called.");
		Vector3 temp = new Vector3(-1000,0,0);
		transform.localPosition = temp;
	}
	//not for UI
	private void OnMouseOver()
	{
		
		print("Hello");
	}
	//Not for UI
	private void OnMouseExit()
	{
		//Place it outside the UI window, might be good to make sure it isn't visible either, to lower the amount of drawing required.
		//Vector3 temp = new Vector3(-1000,0,0);
		//transform.localPosition = temp;
		print("Hello I'm out");
	}
}

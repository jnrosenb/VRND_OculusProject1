using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControlScript : MonoBehaviour 
{
	public int childCount;
	private OVRInput.Controller thisController;

	private GameObject[] labels;
	private int index = 0;
	public int Index { get { return index; } }
	private bool isSwitching = false;


	// Use this for initialization
	void Start () 
	{
		thisController = OVRInput.Controller.RTouch;
		labels = new GameObject[childCount];
		
		for (int i = 0; i < childCount; i++)
		{
			labels [i] = transform.GetChild (i).gameObject;
			labels [i].SetActive (false);
		}		
		labels [index].SetActive(true);
	}


	void Update()
	{
		if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).y >= 0.25f && !isSwitching)
		{
			isSwitching = true;
			downIndex ();
		}
		if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).y <= -0.25f && !isSwitching)
		{
			isSwitching = true;
			upIndex();
		}
		if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).y > -0.25f && OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick).x < 0.25f)
		{
			isSwitching = false;
		}
	}


	public void upIndex()
	{
		if (index < childCount - 1)
		{
			labels [index++].SetActive (false);
			labels [index].SetActive (true);
		}
	}


	private void downIndex()
	{
		if (index > 0)
		{
			labels [index--].SetActive (false);
			labels [index].SetActive (true);
		}
	}
}

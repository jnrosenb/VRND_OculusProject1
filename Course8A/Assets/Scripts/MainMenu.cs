using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour 
{
	private int state = 1;
	private Animator animator;
	private bool isSwitching = false;
	private OVRInput.Controller thisController;


	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator> ();
		thisController = OVRInput.Controller.RTouch;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x >= 0.25f && !isSwitching)
		{
			isSwitching = true;

			if (state < 2)
			{
				animator.SetBool ("changeRight", true);
				state++;
				animator.SetInteger ("CurrentState", state);
				Debug.Log ("Should've gone right. Current state is " + state);
			}
		}
		if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x <= -0.25f && !isSwitching)
		{
			isSwitching = true;

			if (state > 0)
			{
				animator.SetBool ("changeLeft", true);
				state--;
				animator.SetInteger ("CurrentState", state);
				Debug.Log ("Should change left. Current state " + state);
			}
		}
		if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x > -0.25f && OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick).x < 0.25f)
		{
			isSwitching = false;
//			animator.SetBool ("changeRight", false);
//			animator.SetBool ("changeLeft", false);
		}	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceBody : MonoBehaviour {

	private float leftTorque = 0f;
	public float LTorque
	{
		get{ return leftTorque; }
		set{ leftTorque = value; }
	}

	private float rightTorque = 0f;
	public float RTorque
	{
		get{ return rightTorque; }
		set{ rightTorque = value; }
	}

	private Rigidbody rgbdy;


	// Use this for initialization
	void Start () 
	{
		rgbdy = GetComponent<Rigidbody> ();
		rightTorque = 0f;
		leftTorque = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//This is just to help me measure the limits of the balance when rotating.
		float xRotation = this.gameObject.transform.rotation.eulerAngles.x;
		if (xRotation > 180f)
			xRotation -= 360f;

		if (leftTorque > rightTorque && xRotation <= 30f)
		{
			//rgbdy.angularVelocity.Set (1f, 0f, 0f);
			this.gameObject.transform.Rotate (0.1f, 0f, 0f);
		}
		else if (rightTorque > leftTorque && xRotation >= -30f)
		{
			//rgbdy.angularVelocity.Set (-1f, 0f, 0f);
			this.gameObject.transform.Rotate (-0.1f, 0f, 0f);
		}
	}
}

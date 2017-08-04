using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	public GameObject otherPortal;

	// Use this for initialization
	void Start () {
		
	}

	//When the ball enters the collider:
	void OnTriggerEnter(Collider col)
	{
		GameObject ball = col.gameObject;

		ball.transform.position = otherPortal.transform.position;  
	}
}

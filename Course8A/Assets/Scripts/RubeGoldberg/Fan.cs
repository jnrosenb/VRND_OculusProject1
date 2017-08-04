using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {

	public Transform origin;
	public float fanForce = 1f;

	// Use this for initialization
	void Start () {
		
	}

//	void OnTriggerEnter(Collider col)
//	{
//		Physics.gravity /= 4;
//	}

	void OnTriggerStay(Collider col)
	{
		Rigidbody ball = col.gameObject.GetComponent<Rigidbody> ();

//		if (Mathf.Abs(ball.velocity.y) <= 50f)
//		{
			float zDist = Mathf.Abs (col.gameObject.transform.position.z - origin.position.z);

			ball.AddForce (new Vector3 (0f, zDist * zDist * fanForce, 0f));
//		}
	}

//	void OnTriggerExit(Collider col)
//	{
//		Physics.gravity *= 4;
//	}
}

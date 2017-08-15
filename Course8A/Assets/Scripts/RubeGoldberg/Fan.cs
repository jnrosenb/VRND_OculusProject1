using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {

	public Transform origin;
	public float fanForce = 1f;
	public Transform downRef;


	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Ball")
		{
			Rigidbody ball = col.gameObject.GetComponent<Rigidbody> ();
			float zDist = Mathf.Abs (col.gameObject.transform.position.z - origin.position.z);
			//ball.AddForce (new Vector3 (0f, zDist * fanForce, 0f));
			ball.AddForce ((zDist * fanForce) * (transform.position - downRef.position).normalized);
		}
	}
}

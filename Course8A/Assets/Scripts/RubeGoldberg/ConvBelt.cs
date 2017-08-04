using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvBelt : MonoBehaviour 
{

	public float pullStrength = 1f;
	private Vector3 startPos;
	private Vector3 endPos;


	void Start()
	{
		foreach (Transform t in transform)
		{
			if (t.name == "StartPos")
			{
				startPos = t.position;
			}
			else if (t.name == "EndPos")
			{
				endPos = t.position;
			}
		}
	}


	void OnCollisionStay(Collision col)
	{
		if (col.gameObject.tag == "Ball")
		{
			Rigidbody rgby = col.gameObject.GetComponent<Rigidbody> ();
			rgby.AddForce ((endPos - startPos).normalized * pullStrength);
		}
	}
}

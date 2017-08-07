using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

	public float trampolineForce = 100f;
	
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint cp in collision.contacts)
		{
			if (cp.otherCollider.gameObject.tag == "Ball")
			{
				//Rigidbody ball = cp.thisCollider.gameObject.GetComponent<Rigidbody> ();
				Rigidbody ball = cp.otherCollider.gameObject.GetComponent<Rigidbody> ();

				//Vector3 ballDirection = ball.velocity.Normalize ();

				ball.AddForce (-cp.normal * trampolineForce);
			}
		}
	}
}

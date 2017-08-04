using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceThing : MonoBehaviour {

	public BalanceBody body;

	//When ball is on top:
	void OnCollisionStay(Collision collision)
	{
		foreach(ContactPoint cp in collision.contacts)
		{
			//Cacheing some useful variables.
			Vector3 point = cp.point;
			Rigidbody otherRgbdy = cp.thisCollider.gameObject.GetComponent<Rigidbody> ();
			float mass = otherRgbdy.mass;

			//Contact point z component minus center z component gives the straight distance to the
			//Center of the balance bar. Negative if it is to the left, positive if to the right.
			float zDistance = point.z - body.gameObject.transform.position.z;

			//If its positive, it sums to one side. Otherwise, to the other:
			if (zDistance > 0)
			{
				body.RTorque += Physics.gravity.y * mass * Mathf.Abs (zDistance);
			}
			else
			{
				body.LTorque += Physics.gravity.y * mass * Mathf.Abs (zDistance);
			}
		}
	}

	//When ball is on top:
	void OnCollisionExit(Collision collision)
	{
		foreach(ContactPoint cp in collision.contacts)
		{
			//Cacheing some useful variables.
			Vector3 point = cp.point;
			Rigidbody otherRgbdy = cp.thisCollider.gameObject.GetComponent<Rigidbody> ();

			//Contact point z component minus center z component gives the straight distance to the
			//Center of the balance bar. Negative if it is to the left, positive if to the right.
			float zDistance = point.z - body.gameObject.transform.position.z;

			//If its positive, it sums to one side. Otherwise, to the other:
			if (zDistance > 0)
			{
				body.RTorque -= Physics.gravity.y * otherRgbdy.mass * Mathf.Abs (zDistance);
			}
			else
			{
				body.LTorque -= Physics.gravity.y * otherRgbdy.mass * Mathf.Abs (zDistance);
			}
		}
	}

}

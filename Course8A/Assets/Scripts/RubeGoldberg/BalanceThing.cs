using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceThing : MonoBehaviour {

	public BalanceBody body;
	public Transform directionRef;
	public float centerDeadZone = 0.1f;

	//When ball is on top:
	void OnCollisionStay(Collision collision)
	{
		foreach(ContactPoint cp in collision.contacts)
		{
			//Direction reference to know which side to affect:
			Vector3 direction = directionRef.position - body.transform.position;

			//This is just to help me measure the limits of the balance when rotating.
			float xRotation = this.gameObject.transform.rotation.eulerAngles.x;
			if (xRotation > 180f)
				xRotation -= 360f;
			
			//Cacheing some useful variables.
			Vector3 point = cp.point;
			Rigidbody otherRgbdy = cp.thisCollider.gameObject.GetComponent<Rigidbody> ();
			float mass = otherRgbdy.mass;

			float distance = Vector3.Distance (body.gameObject.transform.position, point);
			Vector3 CollisionDirection = point - body.gameObject.transform.position;

			//If its positive, it sums to one side. Otherwise, to the other:
			if (Vector3.Dot(direction, CollisionDirection) < 0 && distance >= centerDeadZone)
			{
				body.RTorque += Physics.gravity.y * mass * distance;// * Mathf.Sin(xRotation + 90);
			}
			else if (Vector3.Dot(direction, CollisionDirection) > 0 && distance >= centerDeadZone)
			{
				body.LTorque += Physics.gravity.y * mass * distance;// * Mathf.Sin(xRotation + 90);
			}
		}
	}

	//When something collides:
	void OnCollisionEnter(Collision collision)
	{
		body.numberElementsOnTop++;
	}

	//When ball is on top:
	void OnCollisionExit(Collision collision)
	{
		body.numberElementsOnTop--;
	}

}

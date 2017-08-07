using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript: MonoBehaviour {

	public Vector3 ballStartingPos = new Vector3(0f, 1.153f, -2.072f);

	// Use this for initialization
	void Start () 
	{
		//transform.position = ballStartingPos;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	//This will be called when it hits the ground:
	void OnCollisionEnter(Collision col)
	{
		if (col.collider.gameObject.tag == "ground")
		{
			Rigidbody rgbdy = this.GetComponent<Rigidbody> ();
			rgbdy.isKinematic = true;
			this.transform.position = ballStartingPos;
			rgbdy.isKinematic = false;
		}
	}
}

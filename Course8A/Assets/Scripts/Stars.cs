using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Ball")
		{
			ScoreManager.starsCollected++;
			Destroy(this.gameObject);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRControllerInputManager : MonoBehaviour 
{
	
	public bool leftHand;

	public float maxDistance = 15f;
	public float playerHeight = 1.3f;
	public GameObject template;
	public GameObject particleTemplate;
	public GameObject pointerTemplate;
	public GameObject player;

	private GameObject ray1;
	private GameObject targetPointer;
	private ParticleSystem pSys;
	private LineRenderer line1;
	private Vector3 rayTarget;
	private Vector3 teleportationPos;
	private ObjectMenuManager menuManager;
	private bool isSwitching = false;

	private OVRInput.Controller thisController;

	//Use this for initialization
	void Start () 
	{
		if (leftHand)
			thisController = OVRInput.Controller.LTouch;
		else
			thisController = OVRInput.Controller.RTouch;

		if (leftHand)
		{
			ray1 = GameObject.Instantiate (template);
			pSys = GameObject.Instantiate (particleTemplate).GetComponent<ParticleSystem>() as ParticleSystem;
			line1 = ray1.GetComponent<LineRenderer> () as LineRenderer;
			ray1.SetActive (false);
			pSys.gameObject.SetActive (false);

			targetPointer = GameObject.Instantiate (pointerTemplate);
			targetPointer.SetActive (false);
		}

		if (!leftHand)
		{
			menuManager = GetComponentInChildren<ObjectMenuManager> () as ObjectMenuManager;
		}
	}


	//Update is called once per frame
	void Update () 
	{
		//Teleportation code:
		if (leftHand)
		{
			teleportManager ();
		}

		//Object menu code:
		if (!leftHand)
		{
			//Menu manager on/off mechanic:
			if (OVRInput.Get (OVRInput.Button.PrimaryIndexTrigger, thisController))
			{
				menuManager.turnOn();
			}
			if (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger, thisController))
			{
				menuManager.turnOff();
			}

			//Menu manager item switching:
			if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x >= 0.25f && !isSwitching)
			{
				menuManager.shiftRight();
				isSwitching = true;
			}
			if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x <= -0.25f && !isSwitching)
			{
				menuManager.shiftLeft();
				isSwitching = true;
			}
			if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x > -0.25f && OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x < 0.25f)
			{
				isSwitching = false;
			}
		}
	}



	//This method will be called when it is grabbed:
	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.layer == 9)
		{
			Rigidbody rgby = col.gameObject.GetComponent<Rigidbody> () as Rigidbody;

			if (OVRInput.GetDown (OVRInput.Button.PrimaryHandTrigger, thisController))
			{
				col.gameObject.transform.SetParent (transform);
				rgby.isKinematic = true;
			}
			if (OVRInput.GetUp (OVRInput.Button.PrimaryHandTrigger, thisController))
			{
				rgby.isKinematic = false;
				rgby.velocity = OVRInput.GetLocalControllerVelocity (thisController);
				rgby.angularVelocity = OVRInput.GetLocalControllerAngularVelocity (thisController);
				col.gameObject.transform.SetParent (null);
			}
		}
	}


	//Handles the teleporting bit:
	private void teleportManager()
	{
		if (OVRInput.Get (OVRInput.Button.PrimaryIndexTrigger))
		{
			ray1.SetActive (true);

			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit, maxDistance, 256))
			{
				float t = 0f;
				int i = 0;
				while (t <= 1f)
				{
					Vector3 bt = (1 - t) * transform.position + (t) * hit.point;
					line1.SetPosition (i++, bt);
					t += 0.0625f;
				}

				teleportationPos = hit.point;
				targetPointer.transform.position = teleportationPos;
				targetPointer.SetActive (true);
			}
			else
			{
				rayTarget = transform.position + maxDistance * transform.forward;

				RaycastHit hitGround;
				if (Physics.Raycast (rayTarget, Vector3.down, out hitGround, 100 * maxDistance, 256))
				{
					float t = 0f;
					int i = 0;
					while (t <= 1f)
					{
						Vector3 bt = ((1 - t) * (1 - t) * transform.position) + ((2 * t * (1 - t)) * rayTarget) + (t * t * hitGround.point);
						line1.SetPosition (i++, bt);
						t += 0.0625f;
					}

					ray1.SetActive (true);

					teleportationPos = hitGround.point;
					targetPointer.transform.position = teleportationPos;
					targetPointer.SetActive (true);
				}
			}
		}
		if (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger))
		{
			ray1.SetActive (false);
			targetPointer.SetActive (false);

			player.transform.position = new Vector3 (teleportationPos.x, teleportationPos.y + playerHeight, teleportationPos.z);
		}
	}
}

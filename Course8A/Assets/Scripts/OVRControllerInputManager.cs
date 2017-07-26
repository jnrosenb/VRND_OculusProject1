using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRControllerInputManager : MonoBehaviour 
{
	//Important vars and templates:
	public bool leftHand;
	public float maxDistance = 15f;
	public float playerHeight = 1.3f;
	public GameObject template;
	public GameObject particleTemplate;
	public GameObject pointerTemplate;
	public GameObject player;

	//Raycasting variables (teleporting):
	private GameObject ray1;
	private GameObject targetPointer;
	private ParticleSystem pSys;
	private LineRenderer line1;
	private Vector3 rayTarget;
	private Vector3 teleportationPos;
	private ObjectMenuManager menuManager;
	private bool isSwitching = false;

	//Variables relating to the throw action:
	private OVRInput.Controller thisController;
	public State handHoldingState;
	private Rigidbody handRgby;
	private Rigidbody heldObjRgby;
	private FixedJoint joint;

	//Enum representing the hand states:
	public enum State
	{
		EMPTY,
		TOUCHING,
		HOLDING
	};


	//Use this for initialization
	void Start () 
	{
		this.handRgby = GetComponent<Rigidbody> ();
		this.handHoldingState = State.EMPTY;

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

		//Object holding and throwing mechanich:
		switch (handHoldingState)
		{
			case State.TOUCHING:
				if (joint == null && OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, thisController) >= 0.5f)
				{
					heldObjRgby.velocity = Vector3.zero;
					joint = heldObjRgby.gameObject.AddComponent<FixedJoint> () as FixedJoint;
					joint.connectedBody = handRgby;
					handHoldingState = State.HOLDING;
				}
				break;
			case State.HOLDING:
				if (joint != null && OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, thisController) < 0.5f)
				{
					DestroyImmediate (joint);
					joint = null;

					heldObjRgby.velocity = OVRInput.GetLocalControllerVelocity (thisController);
					heldObjRgby.angularVelocity = OVRInput.GetLocalControllerAngularVelocity (thisController);

					heldObjRgby = null;
					handHoldingState = State.EMPTY;
				}
				break;
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


	//This method will be called when col enter trigger:
	void OnTriggerEnter(Collider col)
	{
		if (handHoldingState == State.EMPTY)
		{
			GameObject temp = col.gameObject;
			Rigidbody tempRgby = temp.GetComponent<Rigidbody> ();

			if (temp != null && temp.layer == LayerMask.NameToLayer("throwable") && tempRgby != null)
			{
				heldObjRgby = temp.GetComponent<Rigidbody>();
				handHoldingState = State.TOUCHING;
			}
		}
	}


	//This method will be called when col leaves trigger:
	void OnTriggerExit(Collider col)
	{
		if (!(handHoldingState == State.HOLDING))
		{
			if (col.gameObject.layer == LayerMask.NameToLayer("throwable"))
			{
				heldObjRgby = null;
				handHoldingState = State.EMPTY;
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

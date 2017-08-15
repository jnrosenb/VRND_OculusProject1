using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRControllerInputManager : MonoBehaviour 
{
	//Important vars and templates:
	public bool leftHand;
	public float maxDistance = 15f;
	public float playerHeight = 1.3f;
	public GameObject pointerTemplate1;
	public GameObject[] pointerTemplate2;
	public GameObject rayTemplate1;
	public GameObject rayTemplate2;
	public GameObject player;
	//public GameObject particleTemplate;

	//Raycasting variables (teleporting):
	private GameObject ray1;
	private GameObject targetPointer;
	private LineRenderer line1;
	private Vector3 rayTarget;
	private Vector3 teleportationPos;
	//private ParticleSystem pSys;

	//MenuManager Variables:
	private bool isSwitching = false;
	private bool inSpawnProcess = false;
	private ObjectMenuManager menuManager;
	private GameObject ray2;
	private GameObject[] pointers2;
	private GameObject targetPointer2;
	private LineRenderer line2;
	private Vector3 rayTarget2;
	private Vector3 newObjPosition;

	//Variables relating to the throw action:
	private OVRInput.Controller thisController;
	public State handHoldingState;
	private Rigidbody handRgby;
	private Rigidbody heldObjRgby;
	private FixedJoint joint;
	private bool isStructure;

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
			ray1 = GameObject.Instantiate (rayTemplate1);
			//pSys = GameObject.Instantiate (particleTemplate).GetComponent<ParticleSystem>() as ParticleSystem;
			line1 = ray1.GetComponent<LineRenderer> () as LineRenderer;
			ray1.SetActive (false);
			//pSys.gameObject.SetActive (false);

			targetPointer = GameObject.Instantiate (pointerTemplate1);
			targetPointer.SetActive (false);
		}

		if (!leftHand)
		{
			menuManager = GetComponentInChildren<ObjectMenuManager> () as ObjectMenuManager;
			ray2 = GameObject.Instantiate (rayTemplate2);
			line2 = ray2.GetComponent<LineRenderer> () as LineRenderer;
			ray2.SetActive (false);

			//Replace the hardcoded number for a variable later:
			pointers2 = new GameObject[7];
			for (int i = 0; i < 7; i++)
			{
				GameObject pointer = GameObject.Instantiate (pointerTemplate2[i]);
				pointer.SetActive (false);
				pointers2 [i] = pointer;
			}
			targetPointer2 = pointers2 [0];

			targetPointer2.SetActive (false);
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
					if (!isStructure)
					{
						heldObjRgby.velocity = Vector3.zero;
						joint = heldObjRgby.gameObject.AddComponent<FixedJoint> () as FixedJoint;
						joint.connectedBody = handRgby;
						handHoldingState = State.HOLDING;
					}
					else
					{
						joint = heldObjRgby.gameObject.AddComponent<FixedJoint> () as FixedJoint;
						heldObjRgby.gameObject.transform.SetParent (this.transform);
						handHoldingState = State.HOLDING;
					}
			}
			break;
		case State.HOLDING:
			if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, thisController) < 0.5f && joint != null )
			{
				DestroyImmediate (joint);
				joint = null;

				heldObjRgby.velocity = OVRInput.GetLocalControllerVelocity (thisController);
				heldObjRgby.angularVelocity = OVRInput.GetLocalControllerAngularVelocity (thisController);

				heldObjRgby.gameObject.transform.SetParent (null);
				heldObjRgby = null;
				handHoldingState = State.EMPTY;
			}
			break;
		}

		//Object menu code:
		if (!leftHand)
		{
			//When button is pressed, interface for positioning item shows. When released, item is created:
			if (OVRInput.Get (OVRInput.Button.PrimaryIndexTrigger, thisController))
			{
				ray2.SetActive (true);
				inSpawnProcess = true;
				menuManager.turnOff();

				RaycastHit hit;
				if (Physics.Raycast (transform.position, transform.forward, out hit, maxDistance, 256))
				{
					line2.SetPosition (0, transform.position);
					line2.SetPosition (1, hit.point);
					newObjPosition = hit.point;
					targetPointer2.transform.position = new Vector3(newObjPosition.x, 0f, newObjPosition.z);
					targetPointer2.SetActive (true);
				}
				else
				{
					rayTarget2 = transform.position + maxDistance * transform.forward;
					RaycastHit hitGround;
					if (Physics.Raycast (rayTarget2, Vector3.down, out hitGround, 100 * maxDistance, 256))
					{
						line2.SetPosition (0, transform.position);
						line2.SetPosition (1, rayTarget2);
						ray2.SetActive (true);

						newObjPosition = hitGround.point;
						targetPointer2.transform.position =  new Vector3(newObjPosition.x, 0f, newObjPosition.z);;
						targetPointer2.SetActive (true);
					}
				}
			}
			if (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger, thisController))
			{
				targetPointer2.SetActive (false);
				ray2.SetActive (false);
				targetPointer2.SetActive (false);
				menuManager.createObject( new Vector3(newObjPosition.x, 0f, newObjPosition.z), targetPointer2.transform.rotation);
				inSpawnProcess = false;
			}

			//Menu manager on/off mechanic. It will only turn it on if not in spawnProcess:
			if (OVRInput.Get (OVRInput.Touch.PrimaryThumbstick, thisController) && !inSpawnProcess)
			{
				menuManager.turnOn();
			}
			if (OVRInput.GetUp (OVRInput.Touch.PrimaryThumbstick, thisController))
			{
				menuManager.turnOff();
			}

			//Menu manager item switching:
			if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x >= 0.25f && !isSwitching)
			{
				//If in spawn process, pointer will be visible:
				if (inSpawnProcess)
				{
					targetPointer2.SetActive (true);
					targetPointer2.transform.Rotate (new Vector3 (0f, OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x, 0f), Space.Self);
				}
				else
				{
					targetPointer2.SetActive (false);
					menuManager.shiftRight ();
					isSwitching = true;
					targetPointer2 = pointers2 [menuManager.index];
				}
			}
			if (OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x <= -0.25f && !isSwitching)
			{
				//If in spawn process, pointer will be visible:
				if (inSpawnProcess)
				{
					targetPointer2.SetActive (true);
					targetPointer2.transform.Rotate (new Vector3 (0f, OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x, 0f), Space.Self);
				}
				else
				{
					targetPointer2.SetActive (false);
					menuManager.shiftLeft ();
					isSwitching = true;
					targetPointer2 = pointers2 [menuManager.index];
				}
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
			else if (temp != null && temp.tag == "Structure" && tempRgby != null)
			{
				heldObjRgby = temp.GetComponent<Rigidbody>();
				handHoldingState = State.TOUCHING;
				isStructure = true;
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
			else if (col.gameObject.tag == "Structure")
			{
				heldObjRgby = null;
				handHoldingState = State.EMPTY;
			}
			isStructure = false;
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

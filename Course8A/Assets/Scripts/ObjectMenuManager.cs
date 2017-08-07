using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour 
{

	public GameObject[] objectsTemplates;

	private GameObject[] realObjects;
	private GameObject selected;
	private int index = 0;
	private int maxIndex;

	// Use this for initialization
	void Start () 
	{
		maxIndex = objectsTemplates.Length - 1;
		realObjects = new GameObject[maxIndex + 1];

		if (maxIndex > 0)
		{
			for (int i = 0; i < maxIndex + 1; i++)
			{
				realObjects [i] = GameObject.Instantiate (objectsTemplates[i]);
				realObjects [i].SetActive (false);
			}

			selected = realObjects [index];
		}
	}


	//Shift right:
	public void shiftRight()
	{
		selected.SetActive (false);

		if (index >= maxIndex)
		{
			index = 0;
			selected = realObjects [index];
		}
		else
			selected = realObjects [++index];
	}


	//Shift left:
	public void shiftLeft()
	{
		selected.SetActive (false);

		if (index <= 0)
		{
			index = maxIndex;
			selected = realObjects [index];
		}
		else
			selected = realObjects [--index];
	}


	//Turns menu selected object visible:
	public void turnOn()
	{
		Transform controller = gameObject.transform.parent;
		selected.transform.position = controller.position + 1f * controller.forward + 0.2f * controller.up; 
		//selected.transform.rotation = Quaternion.Euler(0f, controller.rotation.eulerAngles.y, 0f);

		selected.SetActive (true);
	}


	//Turns menu selected object invisible:
	public void turnOff()
	{
		selected.SetActive (false);
	}

}

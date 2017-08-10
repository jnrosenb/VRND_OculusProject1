using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour 
{
	//Templates:
	public GameObject[] objectsTemplates;
	private GameObject[] realObjects;

	//Other vars:
	private GameObject[] displayObjects;
	private GameObject selected;
	public int index = 0;
	private int maxIndex;
	private bool[] usedIndexes;


	// Use this for initialization
	void Start () 
	{
		maxIndex = objectsTemplates.Length - 1;
		displayObjects = new GameObject[maxIndex + 1];
		usedIndexes = new bool[maxIndex + 1];

		if (maxIndex > 0)
		{
			for (int i = 0; i < maxIndex + 1; i++)
				usedIndexes [i] = false;

			for (int i = 0; i < maxIndex + 1; i++)
			{
				GameObject displayObj = transform.GetChild (i).gameObject;
				displayObjects [i] = displayObj;
				displayObjects [i].SetActive (false);
			}

			selected = displayObjects [index];
		}
	}


	//Creates the currently selected item:
	public void createObject(Vector3 newObjPos, Quaternion rotation)
	{
		if (!usedIndexes [index])
		{
			Transform controller = gameObject.transform.parent;
			GameObject obj = GameObject.Instantiate (objectsTemplates[index]);
			obj.transform.position = newObjPos;
			obj.transform.rotation = rotation;

			usedIndexes [index] = true;
		}
	}


	//Shift right:
	public void shiftRight()
	{
		selected.SetActive (false);

		if (index >= maxIndex)
		{
			index = 0;
			selected = displayObjects [index];
		}
		else
			selected = displayObjects [++index];
	}


	//Shift left:
	public void shiftLeft()
	{
		selected.SetActive (false);

		if (index <= 0)
		{
			index = maxIndex;
			selected = displayObjects [index];
		}
		else
			selected = displayObjects [--index];
	}


	//Turns menu selected object visible:
	public void turnOn()
	{
		Transform controller = gameObject.transform.parent;
		selected.transform.position = controller.position + 1f * controller.forward + 0.2f * controller.up; 
		selected.transform.rotation = Quaternion.Euler(0f, controller.rotation.eulerAngles.y, 0f);

		selected.SetActive (true);
	}


	//Turns menu selected object invisible:
	public void turnOff()
	{
		selected.SetActive (false);
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PhysicsDisabler : MonoBehaviour {

	public List<Transform> trList; 
	public bool onOff = true;
	void Start () 
	{
		foreach(Transform tr in this.transform)
		{
			if(tr.GetComponent<Rigidbody>())
			{
				trList.Add(tr);
			}
		}
		foreach(Transform rg in trList)
		{
			rg.rigidbody.isKinematic = true;
			rg.collider.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Transform rg in trList)
		{
			if(!onOff && rg.rigidbody.isKinematic)
			{
				rg.rigidbody.isKinematic = false;
				rg.collider.enabled = true;
			}
			if(onOff && !rg.rigidbody.isKinematic)
			{
				rg.rigidbody.isKinematic = true;
				rg.collider.enabled = false;
			}
		}

	}
}

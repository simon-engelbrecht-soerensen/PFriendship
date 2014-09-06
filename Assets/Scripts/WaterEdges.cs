using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WaterEdges : MonoBehaviour {
	public List<ContactPoint> cPoints = new List<ContactPoint>();
//	public ContactPoint[] contactsA = new ContactPoint[100];
		// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnCollisionEnter(Collision col)
	{
		for(int i = 0; i < col.contacts.Length; i++)
		{
			cPoints.Add(col.contacts[i]);
//			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			cube.transform.position = col.contacts[i].point;
		}
//		for(int i = 0;i < col.contacts.) {
			Debug.Log (cPoints.Count);
						
//
//		}
	}

//	void OnCollisionExit(Collision col2)
//	{
//		for(int i = 0; i < col2.contacts.Length; i++)
//		{
//			cPoints.Remove(col2.contacts[i]);
//		}
////		Debug.Log (cPoints.Count);
//		Debug.Log ("NGEUIO");
//	}
//
//	void FixedUpdate()
//	{
//		Debug.Log (cPoints.Count); 
//	}

	void OnDrawGizmo()
	{


		if(cPoints.Count > 0)
		{
			foreach(ContactPoint cP in cPoints)
			{
				Gizmos.DrawSphere(cP.point, 10);
////				Gizmos.DrawSphere(cP.point.transform.position, 1);
			}
		}
	}
}

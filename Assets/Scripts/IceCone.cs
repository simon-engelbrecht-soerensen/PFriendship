using UnityEngine;
using System.Collections;

public class IceCone : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {
	
	}
	
	void OnTriggerStay(Collider col)
	{
		if(col.gameObject.tag == "enemy")
		{
			
		}
	}
}

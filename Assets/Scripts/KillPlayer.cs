using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.CompareTag("Player"))
	    {
			col.gameObject.GetComponent<PlayerStats>().health = -1;
		}
		if(col.gameObject.CompareTag("gold"))
		{
			col.gameObject.SetActive(false);
		}
	}
}

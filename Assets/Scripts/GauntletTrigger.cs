using UnityEngine;
using System.Collections;

public class GauntletTrigger : MonoBehaviour {
	public bool startGauntlet;
	public bool EndGauntlet;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Player")
		{
			if(startGauntlet)
			{
				transform.parent.gameObject.GetComponent<Gauntlet>().started = true;
			}
			if(!startGauntlet)
			{
				transform.parent.gameObject.GetComponent<Gauntlet>().started = false;
			}
		}
	}
}

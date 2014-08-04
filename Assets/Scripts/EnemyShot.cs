using UnityEngine;
using System.Collections;

public class EnemyShot : MonoBehaviour {
	public float dmgToPlayer = 0.5f;
//	public float backlashForce = 5f;

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
			col.gameObject.GetComponent<PlayerStats>().health -= dmgToPlayer;
			col.gameObject.GetComponent<PlayerStats>().StartCoroutine("GotHit");
		}

		Destroy (this.gameObject);
	}

	void OnColliderEnter(Collision col)
	{
		if(col.gameObject.tag != "Player")
		{
			Destroy(this.gameObject);
		}
	}
}

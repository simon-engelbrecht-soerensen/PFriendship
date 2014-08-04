using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitboxDetect : MonoBehaviour {
	public bool detectHit;
	public float backlashForce;
	bool push;
	GameObject pushObj;
	public float pushDuration = 0.2f;
	public List<Transform> insideCol;
	void Start () {
	
	}

	void Update () {
		if(detectHit)
		{
			HitPlayer ();
		}
	}

	void FixedUpdate()
	{
		if(push)
		{
			StartCoroutine("Push",pushObj);
		}
	}
	IEnumerator OnTriggerEnter(Collider col)
	{
		insideCol.Add(col.gameObject.transform);
		yield return new WaitForSeconds(0.2f);
	}
	IEnumerator OnTriggerExit(Collider col)
	{
		insideCol.Remove(col.gameObject.transform);
		yield return new WaitForSeconds(0.2f);
	}

	void HitPlayer()
	{
		foreach(Transform player in insideCol)
		{
			if(player.gameObject.tag == "Player")
			{
				if(!player.gameObject.GetComponent<PlayerStats>().invul)
				{
					detectHit = false;
					StartCoroutine("MakeHitHappen",player.gameObject);
					push = true;
					pushObj = player.gameObject;
				}
				
			}
		}
	}

//	void OnTriggerStay(Collider col)
//	{
//		if(detectHit)
//		{
//
//			if(col.gameObject.tag == "Player")
//			{
//				if(!col.gameObject.GetComponent<PlayerStats>().invul)
//				{
//					detectHit = false;
//					StartCoroutine("MakeHitHappen",col.gameObject);
//					push = true;
//					pushObj = col.gameObject;
//				}
//
//			}
//		}
//	}

	IEnumerator MakeHitHappen(GameObject col)
	{
//		Debug.Log ("HIT");

//		col.rigidbody.AddForceAtPosition(direction.normalized * backlashForce, transform.position);
		col.GetComponent<PlayerStats>().hit = true;
		yield return new WaitForSeconds(pushDuration);
		push = false;
		//				col.rigidbody.AddForce((col.gameObject.transform.position - transform.position).normalized * backlashForce, ForceMode.Impulse);
	}

	IEnumerator Push(GameObject col)
	{
		Vector3 direction = col.transform.position - transform.parent.transform.position;
//		col.rigidbody.AddForce (direction * backlashForce);
		yield return null;
	}
}

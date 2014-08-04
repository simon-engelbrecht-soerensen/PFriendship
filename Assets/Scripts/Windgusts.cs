using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Windgusts : MonoBehaviour {
	public List<GameObject> windGusts;
	private float objWidthX;
	private float objWidthZ;
	private float objHeight;
	public int amtToCache = 10;
	public float timeBetween = 3;
	private float counter;
	void Start () 
	{
		objWidthX = this.transform.localScale.x;
		objWidthZ = this.transform.localScale.z;
		objHeight = this.transform.localScale.y;
//		for(int i = 0; i<amtToCache; i++)
//		{
//			GameObject bloodSplatInst = Instantiate(bloodSplatPlane,Vector3.zero,bloodSplatPlane.gameObject.transform.rotation) as GameObject;
//			bloodSplatInst.transform.parent = this.transform;
//			cachedBlood.Add(bloodSplatInst);
//		}
//		foreach(GameObject splat in cachedBlood)
//		{
//			splat.SetActive(false);
//		}
//		Instantiate(windGusts[1], new Vector3(Random.Range(this.transform.position.x - objWidthX/2,this.transform.position.x + objWidthX/2),Random.Range(this.transform.position.y - objHeight/2,this.transform.position.y + objHeight/2),Random.Range(this.transform.position.z - objWidthZ/2,this.transform.position.z + objWidthZ/2)),Quaternion.identity);
	}
	

	void Update ()
	{
		if(counter < Random.Range(timeBetween,timeBetween +5))
		{
			counter += Time.deltaTime;
		}
		else
		{
			counter = 0;
			Instantiate(windGusts[Random.Range(0,windGusts.Count)], new Vector3(Random.Range(this.transform.position.x - objWidthX/2,this.transform.position.x + objWidthX/2),Random.Range(this.transform.position.y - objHeight/2,this.transform.position.y + objHeight/2),Random.Range(this.transform.position.z - objWidthZ/2,this.transform.position.z + objWidthZ/2)), Quaternion.identity);
		}


	}
}

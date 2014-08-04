using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Simpleblood : MonoBehaviour {
	public GameObject bloodSplatPlane;
	public int amtToCache = 20;
	public List<GameObject> cachedBlood;
	public int count;
	void Start () {
		for(int i = 0; i<amtToCache; i++)
		{
			GameObject bloodSplatInst = Instantiate(bloodSplatPlane,Vector3.zero,bloodSplatPlane.gameObject.transform.rotation) as GameObject;
			bloodSplatInst.transform.parent = this.transform;
			cachedBlood.Add(bloodSplatInst);
		}
		foreach(GameObject splat in cachedBlood)
		{
			splat.SetActive(false);
		}
	}

	void Update () {
	
	}

	public void CastRay(Transform place)
	{
		RaycastHit hit;
		bool onOff = false;
		if(Physics.Raycast(place.position,-Vector3.up, out hit, 10))
		{
			if(count < amtToCache-1)
			{
				count += 1;
			}
			else
			{
				count = 0;
			}
			float scale = Random.Range(2,5);
			Vector3 hitT = hit.point;
			hitT.y = hit.point.y + 0.01f;
			cachedBlood[count].SetActive(true);
			cachedBlood[count].transform.position = hitT;
			cachedBlood[count].transform.localScale = new Vector3(scale,scale,1);
			cachedBlood[count].transform.localEulerAngles = new Vector3(cachedBlood[count].transform.localEulerAngles.x, Random.Range (0,360), cachedBlood[count].transform.localEulerAngles.z);
		}


	}
}

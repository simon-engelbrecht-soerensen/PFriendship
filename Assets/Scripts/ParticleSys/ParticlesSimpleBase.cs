using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof(MeshRenderer))]
public class ParticlesSimpleBase : MonoBehaviour {

	public GameObject particle;
	public List<GameObject> allParticles; 

	public float timeBeforeStart = 0;
	public float amountOfParticles = 10f;
	public bool oneTimeSpawn = false;
	public float emissionSpeed = 1f;
	public float lifeTime = 1f;
	public Material material;
	private GameObject parentObject;
	int i = 0;
	// Use this for initialization
	void Start () {
		if(GameObject.Find("ParticleParent"))
		{
			parentObject = GameObject.Find("ParticleParent");
		}
		else
		{
			parentObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			parentObject.name = "ParticleParent";
			parentObject.transform.position = new Vector3(9999,9999,9999);
		}

		if(!particle)
		{
			for(int i = 0; i < amountOfParticles; i++)
			{
	//			GameObject temp = Instantiate(particle,new Vector3(9999,9999,9999),Quaternion.identity) as GameObject;
				GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
				temp.transform.position = new Vector3(9999,9999,9999);
				temp.renderer.material = material;
				temp.transform.parent = parentObject.transform;
				allParticles.Add (temp);
			}
		}
		else
		{
			for(int i = 0; i < amountOfParticles; i++)
			{
				GameObject temp = Instantiate(particle,new Vector3(9999,9999,9999),Quaternion.identity) as GameObject;
//				GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
				temp.transform.position = new Vector3(9999,9999,9999);
//				temp.renderer.material = material;
				temp.transform.parent = parentObject.transform;
				allParticles.Add (temp);
			}
		}

		if(!oneTimeSpawn)
		{
			StartCoroutine("Emit");
		}
		else if(oneTimeSpawn)
		{
			StartCoroutine("OneTimeSpawn");
		}


	}
	
	// Update is called once per frame
	void Update () {
//		if(i >= amountOfParticles-1)
//		{
//			i=0;
//		} 
////		yield return new WaitForSeconds(1/emissionSpeed);
//		
//		allParticles[i].transform.position = this.transform.position;
//		i++;

	}

	public IEnumerator OneTimeSpawn()
	{
		yield return new WaitForSeconds(timeBeforeStart);
		foreach(GameObject p in allParticles)
		{
			p.transform.position = this.transform.position;
		}
	}
	public IEnumerator Emit()
	{
		int i = 0;
		yield return new WaitForSeconds(timeBeforeStart);
//
		while(true)
		{
			if(i >= amountOfParticles-1)
			{
				i=0;
			} 
			yield return new WaitForSeconds(0.0002f);

			allParticles[i].transform.position = this.transform.position;
//			StartCoroutine("Die", allParticles[i].gameObject);			

			i++;

			
		}
	}
	IEnumerator Die(GameObject g)
	{
		yield return new WaitForSeconds(lifeTime);
		Debug.Log ("BUGFE");
		StopCoroutine("Emit");
		g.transform.position = new Vector3(1111,1111,1111);
	}



}

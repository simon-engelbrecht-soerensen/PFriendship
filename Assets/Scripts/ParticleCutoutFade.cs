using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
//using Particle = UnityEngine.ParticleSystem.Particle;

public class ParticleCutoutFade : MonoBehaviour {
	public float waitTime = 1f;
	public float alphaValue = 0.2f;
	public ParticleSystem.Particle[] allParticles = new ParticleSystem.Particle[200];
	ParticleSystem pSys;
//	public List[] particles;
	// Use this for initialization
	void Start () {
//		allParticles = new ParticleSystem.Particle[particleSystem.particleCount];
		pSys = GetComponent<ParticleSystem>();
//		StartCoroutine("CutFade");
	}
	
	// Update is called once per frame
	void Update () {
//		allParticles = new ParticleSystem.Particle[particleSystem.particleCount];
//
////		pSys.GetParticles(allParticles);
//		pSys.GetParticles(allParticles);
//		particles = allParticles;
//		Debug.Log (allParticles.Length);
//					Debug.Log(allParticles[11].color);
//		foreach(ParticleSystem.Particle p in allParticles)
//		{
////			p.color
//			p.color = new Color(255,0,0,0.5f);
//		}

	}

	void LateUpdate()
	{
		allParticles = new ParticleSystem.Particle[particleSystem.particleCount];
		
		//		pSys.GetParticles(allParticles);
		pSys.GetParticles(allParticles);
		int i = 0;
		while(i< allParticles.Length-1)
		{
			

				
				//				if(allParticles[i].startLifetime - allParticles[i].lifetime > waitTime)
				//				{
			allParticles[i].color = new Color(157,0,0,alphaValue);
				//				}
				//				Debug.Log(allParticles[1].startLifetime - allParticles[1].lifetime);
				//				Debug.Log(allParticles[1].color.a);

			//			if(i >= allParticles.Length-1)
			//			{
			//				i=0;
			//			} 
			//			Debug.Log (allParticles[1].color);
			i++;
		}
			particleSystem.SetParticles(allParticles,200);

	}
	IEnumerator CutFade()
	{
//		int i = 0;
		while(true)
		{

			for(int i = 0; i< allParticles.Length-1; i++)
			{

//				if(allParticles[i].startLifetime - allParticles[i].lifetime > waitTime)
//				{
				allParticles[i].color = Color.green;
//				}
//				Debug.Log(allParticles[1].startLifetime - allParticles[1].lifetime);
//				Debug.Log(allParticles[1].color.a);
			}
//			if(i >= allParticles.Length-1)
//			{
//				i=0;
//			} 
//			Debug.Log (allParticles[1].color);
			particleSystem.SetParticles(allParticles,200);

//			yield return new WaitForSeconds(0.2f);
			yield return null;

//
////			
//				allParticles[i].color = new Color(255,255,255,0.5f);
////			Debug.Log(allParticles[11].color);
//			i++;
////			}
		}
//		yield return new WaitForSeconds(1f);
//		renderer.material.SetFloat("_SliceAmount", 0.8f);

	}
}

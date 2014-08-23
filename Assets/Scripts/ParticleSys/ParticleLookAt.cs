using UnityEngine;
using System.Collections;

[RequireComponent(typeof (ParticlesSimpleBase))]
public class ParticleLookAt : MonoBehaviour {

	ParticlesSimpleBase pBase;
	Transform cam;
	// Use this for initialization
	void Start () {
		pBase = GetComponent<ParticlesSimpleBase>();
		cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < pBase.allParticles.Count; i++)
		{
//			pBase.allParticles[i].transform.LookAt(transform.position - cam);
			pBase.allParticles[i].transform.LookAt(2 * pBase.allParticles[i].transform.position - cam.position);
		}

	
	}
}

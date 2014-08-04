using UnityEngine;
using System.Collections;

public class Enemy_old : MonoBehaviour {
	public Transform centerObj;
	public float speed = 5f;
	public bool dead;
	public int health = 3;
	void Start () 
	{
		speed = Random.Range(1f ,2f);
//		centerObj = GameObject.FindGameObjectWithTag("CenterObject").transform;
	}
	
	void Update () 
	{
		float step = Time.deltaTime * speed;
		transform.position = Vector3.MoveTowards(this.transform.position,centerObj.position, step);
	}
}

using UnityEngine;
using System.Collections;

public class SpawEnemies : MonoBehaviour {
	public float spawnFreq = 5f;
	float counter;
	public int simultaniousSpawn = 3;
	public GameObject enemyPref;
	void Start () 
	{	
		for(int i = 0; i < simultaniousSpawn; i++)
		{
			Vector3 pos = new Vector3(Random.Range(transform.position.x - transform.localScale.x/2,transform.position.x + transform.localScale.x/2),Random.Range(transform.position.y - transform.localScale.y/2,transform.position.y + transform.localScale.y/2),0);
			GameObject enemy = Instantiate(enemyPref, pos, enemyPref.transform.localRotation) as GameObject;
		}
	}
	
	void Update () 
	{
	counter += Time.deltaTime * 1;
		if(spawnFreq < counter)
		{
			counter = 0f;
			for(int i = 0; i < simultaniousSpawn; i++)
			{
				Vector3 pos = new Vector3(Random.Range(transform.position.x - transform.localScale.x/2,transform.position.x + transform.localScale.x/2),Random.Range(transform.position.y - transform.localScale.y/2,transform.position.y + transform.localScale.y/2),0);
				GameObject enemy = Instantiate(enemyPref, pos, enemyPref.transform.localRotation) as GameObject;
			}
				
		}
	}
}

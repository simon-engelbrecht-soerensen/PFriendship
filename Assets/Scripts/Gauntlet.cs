using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gauntlet : MonoBehaviour {
	private Transform gauntletStart;
	private Transform gauntletEnd;
	public List<Transform> spawnLocations;

	public int spawnInterval = 1;
	private float counter;
	public bool started = false;
	public GameObject enemyType;
	public int maxEnemies = 20;
	private int maxEnemiesCounter;
	void Start () {
		foreach(Transform children in this.transform)
		{
			if(children.gameObject.name == "EnemySpawner")
			{
				spawnLocations.Add(children);
			}
		}
	}

	void Update () {
		if(started)
		{
			counter += Time.deltaTime * 1;
			if(spawnInterval < counter)
			{
				counter = 0f;
				maxEnemiesCounter += 1;
				foreach(Transform sL in spawnLocations)
				{
					if(maxEnemiesCounter < maxEnemies)
					{
						GameObject enemy = Instantiate(enemyType, sL.transform.position, enemyType.transform.localRotation) as GameObject;	
						enemy.GetComponent<Enemy>().chargeEnemy = true;
					}
				}
			}
		}


	}

}

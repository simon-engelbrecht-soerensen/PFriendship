using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour 
{

//	public List<EnemyToSpawn> enemiesToSpawn = new List<EnemyToSpawn>();
//	public EnemyToSpawn[] enemiestospawn;
//	public EnemiesToSpawn[] enemiesToSpawn;
	public List<EnemiesToSpawn> enemiesToSpawn = new List<EnemiesToSpawn>();
	private float timer;
	public ChargePoint chargePoint;

	[System.Serializable]
	public class EnemiesToSpawn : System.Object
	{
		public string name;
		public GameObject enemyType;
		public int spawnTime;
		public int spawnAmount;
		public float frequency;
	}

	void Start () 
	{
		if(transform.parent.tag == "chargePoint")
		{
			chargePoint = transform.parent.GetComponent<ChargePoint>();
		}
		for(int i = 0;i <= enemiesToSpawn.Count-1; i++)
		{
			if(enemiesToSpawn[i].enemyType != null)
			{
				StartCoroutine(SpawnCycle(enemiesToSpawn[i].enemyType, enemiesToSpawn[i].spawnAmount, enemiesToSpawn[i].frequency, enemiesToSpawn[i].spawnTime));
			}
		}
	}

	void Update () 
	{
		timer = chargePoint.timer;

//		if((int)timer == enemiesToSpawn[0].spawnTime)
//		{
//			float spawnCount = 0;
//			spawnCount += Time.deltaTime * 1;
//			if(spawnCount > enemiesToSpawn[0].frequency)
//			{
//				spawnCount = 0;
//				StartCoroutine(SpawnCycle(enemiesToSpawn[0].enemyType, enemiesToSpawn[0].spawnAmount, enemiesToSpawn[0].frequency ));
//			}
////			Debug.Log ("TIMES UP");
//		}
	}

	IEnumerator SpawnCycle(GameObject typeOfEnm, int amount, float freq, int timeSpawn)
	{
		float spawnCount = 0;
		while(true)
		{

//			spawnCount += Time.deltaTime * 1;
//			if(spawnCount > freq)
//			{
//				spawnCount = 0;
			if(timer > timeSpawn && spawnCount < amount)
			{
				spawnCount += 1;
//				Debug.Log (spawnCount);
				GameObject enm = Instantiate(typeOfEnm,this.transform.position, Quaternion.identity) as GameObject;
				enm.GetComponent<Enemy>().chargeEnemy = true;
				yield return new WaitForSeconds(freq);
			}
			else yield return new WaitForSeconds(1);
//			}
			
		}

	}


}






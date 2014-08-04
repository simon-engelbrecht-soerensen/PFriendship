using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDistance : MonoBehaviour {
	public List<Transform> enemies;
	public List<Transform> players;
	public float closeDistance = 100.0f;
	public bool done;
	public List<Transform> enemiesRemove;
	void Awake () 
	{

		GameObject[] enemiesTemp = GameObject.FindGameObjectsWithTag("enemy");
		foreach(GameObject enm in enemiesTemp)
		{
			enemies.Add(enm.transform);
			enm.SetActive(false);
		}
//		foreach(Transform enm2 in enemies)
//		{
//			enm2.gameObject.SetActive(false);
//		}
		GameObject[] playersTemp = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject plyr in playersTemp)
		{
			players.Add(plyr.transform);
		}
		StartCoroutine(CheckDist());

	}
	

	void Update () 
	{
		if(enemiesRemove.Count > 0)
		{
			foreach(Transform enm in enemiesRemove)
			{
				enemies.Remove(enm);
			}
		}
//		if(!done)
//		{
//			foreach(Transform enm in enemies)
//			{
//				if(enm.gameObject.activeSelf)
//				{
//					enm.gameObject.SetActive(false);
//				}
//			}
////			done = true;
//		}
	}

	IEnumerator CheckDist()
	{
		while(true)
		{

		foreach(Transform enm in enemies)
		{
			
			foreach(Transform plyr in players)
			{
				Vector3 offset = enm.position - plyr.position;
				float sqrLen = offset.sqrMagnitude;
				if( sqrLen < closeDistance*closeDistance )
					{
//						print (sqrLen);
						enm.gameObject.SetActive(true);
						enemiesRemove.Add(enm);
					}
			}
		}
			yield return new WaitForSeconds(Random.Range(2,3));
		}

	}
}

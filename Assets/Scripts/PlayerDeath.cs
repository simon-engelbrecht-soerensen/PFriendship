using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour {
	public float respawnTime = 2.0f;
	public Vector3 SpawnPoint;
	public FollowTargets followTargets;
	public GameObject cam;
	public int playersAlive;
	private bool gameOver;
	public float restartTime = 2;
	public MidObject midObj;
//	private FollowTargets followTargets;
	public int standbyQueue;
	public float respawnCount = 10f;

	void Start () {
		midObj = GameObject.Find ("MidObject").GetComponent<MidObject>();
		followTargets = Camera.main.GetComponent<FollowTargets>();
		playersAlive = followTargets.targets.Count;
	}

	void Update () {
		if(playersAlive < 1)
		{
			gameOver = true;
			Debug.Log("GameOver");
			StartCoroutine("RestartGame");
			
		}
	}

	public IEnumerator KillPlayer(GameObject player)
	{
		bool standby1 = false;
//		Debug.Log ("LALAL");
		player.SetActive(false);
		if(playersAlive >= 1)
		{
			playersAlive -= 1;

			followTargets.targets.Remove(player.transform);
			yield return new WaitForSeconds(respawnTime);
			if(!gameOver)
			{
				if(midObj.goldCount > respawnCount)
				{
					StartCoroutine("SpawnPlayer",player);
				}
				else
				{
					StartCoroutine("Standby",player);
					standby1 = true;
				}
			}
		}
//		if(!standby1)
//		{
//			if(playersAlive < 1)
//			{
//				gameOver = true;
//				Debug.Log("GameOver");
//				StartCoroutine("RestartGame");
//
//			}
//		}



//		CameraFollow.players.Remove(player);


	}
	public IEnumerator OutOfVisionSpawn(GameObject player)
	{
		bool toSpawn = true;
//		Vector3 spawnPoint = new Vector3(followTargets.center.x, followTargets.center.y -1, followTargets.center.z);
		Vector3 spawnPoint = new Vector3(midObj.transform.position.x, midObj.transform.position.y, midObj.transform.position.z);  
//		while(toSpawn)
//		{
//			
//			
//			Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 1);
//			if(hitColliders.Length > 0)
//			{
//				spawnPoint = new Vector3(spawnPoint.x + 0.5f, spawnPoint.y + 0.5f, spawnPoint.z);
////				foreach(Collider col in hitColliders)
////				{
//					//					Debug.Log (col.name);
////				}
//			}
//			else
//			{
				
				toSpawn = false;
				player.transform.position = spawnPoint;
				
				
//			}
			yield return null;
//		}
	}

	public IEnumerator SpawnPlayer(GameObject player)
	{
		bool toSpawn = true;
		Vector3 spawnPoint = new Vector3(midObj.transform.position.x, midObj.transform.position.y, midObj.transform.position.z);
//		while(toSpawn)
//		{
//
//
//			Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.5f);
//			if(hitColliders.Length > 0)
//			{
//				foreach(Collider col in hitColliders)
//				{
//					if(col.gameObject.name != "MidObject")
//					{
//					spawnPoint = new Vector3(spawnPoint.x + 0.2f, spawnPoint.y, spawnPoint.z + 0.2f);
//					}
//
////					Debug.Log (col.name);
//				}
//			}
//			else
//			{

				
				playersAlive += 1;
				player.transform.position = spawnPoint;
				followTargets.targets.Add(player.transform);
				player.SetActive(true);
				midObj.goldCount -= respawnCount;
				player.GetComponent<PlayerStats>().health = player.GetComponent<PlayerStats>().startHealth;
				player.GetComponent<PlayerStats>().uiSpriteOver.enabled = true;
				player.GetComponent<PlayerStats>().uiSpriteUnder.enabled = true;
				player.GetComponent<Movement>().dashing = false;
				player.GetComponentInChildren<PlayerAttack>().swinging = false;


//			}
			yield return null;
//		}
	}
	IEnumerator Standby(GameObject player)
	{
		StopCoroutine("KillPlayer");
		bool standby2 = true;
		standbyQueue += 1;
		int placeInQueue = standbyQueue;
		while(standby2)
		{
			if(midObj.goldCount > respawnCount)
			{
				if(placeInQueue == standbyQueue)
				{
					StartCoroutine("SpawnPlayer",player);
					standby2 = false;
				}
			}
			yield return null;
		}
	}
	IEnumerator RestartGame()
	{
		yield return new WaitForSeconds(restartTime);
		Application.LoadLevel(Application.loadedLevel);
	}


}

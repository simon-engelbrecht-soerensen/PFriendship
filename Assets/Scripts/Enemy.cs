using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	public NavMeshAgent agent;
	public GameObject target;
	[HideInInspector]
	public bool dead;
	public float dieTime = 3f;
	public bool meleeEnemy;
	public bool rangedEnemy;
	public float meleeDistance;
	public float rangedDistance;
	public float distance;
	public float agroRange;
	public bool agroed;
	public bool playerVisible;
	float angle;
	public float attackChargeTime = 0.5f;
	public float shootChargeTime = 0.5f;
	public float attackTime = 0.1f;
	public float resetRotTime = 0.2f;
	public float hitboxTimer = 0.2f;
	bool detectHit;
	public GameObject swordAnchor; 
	public HitboxDetect swordHitbox;
	public NavMeshObstacle obstacle;
	public bool charging;
	public bool attacking;
	public GameObject projectile;
	public float health = 3f;
	private float startHealth;
	private float healthForUI;
	public bool hit;
	public bool iSee;
	public GameObject[] players; 
	public bool spellSelect;
	public Color spellSelectColor;
	public Color startColor;
	public bool stunned;
	public bool decoyed;
	public Animator animator; 
	public bool inAttackRange;
	public GameObject splatter;

	private float chargeAnimCount;
	private float hitAnimCount;
	private bool chargeAnimDone;
	private bool hitAnimDone;
	public bool chargeEnemy;
	public Transform gibObj;
	public Transform gibObj2;
	public Transform skellyObj;
	public Transform armorObj;
	public Transform nudeObj;
	public Transform knightRigObj;
	public List<Collider> ragdollObjs = new List<Collider>();
	private Transform[] ragChildren;
	public bool skellyDeath;
	public bool gibDeath;
	public bool nakedDeath;
	public bool ragdollDeath;
	public List<Transform> gibObjs = new List<Transform>();
	public bool testAnim;
	private bool moveDeath = false;
	public bool targetMid;
	private GameObject midObj;
	public Renderer[] childrenWithR;
	public SkinnedMeshRenderer[] childrenWithSMR;
	private Vector3 screenPos;
	public Material cutoutMat;
	public bool abducted;
	public bool suckedUp;
	private bool notNaked;
	private float fadeAmt;
	private bool countAmount;

	private AudioSource audioS;
	public AudioClip deathSound;
	private bool deathPlay;
	public GameObject healthbarOver;
	public GameObject healthbarUnder;
	private GameObject healthSpriteOverInst;
	private GameObject healthSpriteUnderInst;
	[HideInInspector]
	public UISprite uiSpriteOver;
	[HideInInspector]
	public UISprite uiSpriteUnder;
	private Vector3 screenPos2;
	private float screenHeight = Screen.height;
	private float screenWidth = Screen.width;
	public float heightOffset;
	public float widthOffset;
	public int maxCoins = 5;
	private int coinsToSpawn;
	public bool toSpawnCoins;
	public GameObject coinPref;
	private Camera mainCam;
	public Texture2D gizmo;
	void Awake()
	{
		childrenWithR = gameObject.GetComponentsInChildren<Renderer>();
		childrenWithSMR = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
	}
	void Start () 
	{
		mainCam = Camera.main;
		animator = GetComponentInChildren<Animator>();

//		foreach(Transform child in transform)
//		{
//			child.gameObject.SetActive(false);  
//		}
//		agent.avoidancePriority = Random.Range(1,100);
		if(Random.value < 0.7f)
		{
			toSpawnCoins = true;
			coinsToSpawn = Random.Range(1,maxCoins);
		}
		startHealth = health;
		healthSpriteOverInst = Instantiate(healthbarOver, Vector3.zero, Quaternion.identity) as GameObject;
		healthSpriteUnderInst = Instantiate(healthbarUnder, Vector3.zero, Quaternion.identity) as GameObject;

		uiSpriteOver = healthSpriteOverInst.GetComponent<UISprite>();
		uiSpriteUnder = healthSpriteUnderInst.GetComponent<UISprite>();

		audioS = this.gameObject.GetComponent<AudioSource>();
		notNaked = true;
		foreach(SkinnedMeshRenderer smr in childrenWithSMR)
		{
//			smr.enabled = false;
			smr.gameObject.SetActive(false);
		}
//		foreach(Renderer r in childrenWithR)
//		{
//			r.enabled = false;
//		}
//		if(gameObject.name == "MeleeEnemy02")
//		{
			knightRigObj = transform.Find ("Knight_02/Knight_");
			ragChildren = knightRigObj.gameObject.GetComponentsInChildren<Transform>();
			foreach(Transform tra in ragChildren)
			{
				if(tra.gameObject.collider)
				{
					ragdollObjs.Add(tra.gameObject.collider);
				}
			}
			foreach(Collider col in ragdollObjs)
			{
				col.gameObject.rigidbody.useGravity = false;
				col.enabled = false;
			}
			
			gibObj2 = transform.Find("Knight_02/KnightGibs2");
			gibObj2.gameObject.SetActive(false);
			gibObj = transform.Find("Knight_02/KnightGibs");
			skellyObj = transform.Find("Knight_02/KnightSkeleton");
			armorObj = transform.Find("Knight_02/KnightArmor");
			nudeObj = transform.Find("Knight_02/KnightNude");
			//			skellyObj.gameObject.SetActive(false);
			
//		}
		midObj = GameObject.Find("MidObject");

		splatter = GameObject.Find("BloodManager");
		agroed = false;

//		startColor = this.renderer.material.color;
		if(!targetMid)
		{
			target = GameObject.Find("Player1");
			StartCoroutine("RecalculateNearest");
		}
//		agent.SetDestination(target.transform.position);
		StartCoroutine("StepUpdate");


		StartCoroutine("StepUpdateSlow");
//		if (Random.value >= 0.8 && meleeEnemy)	
//		{
//			targetMid = true;			
//		}
		if(targetMid)
		{
			target = midObj;
		}
//		if(gibObj2)
//		{
//			foreach(Transform gib in gibObj2)
//			{
//				gibObjs.Add(gib);
//			}
//		}
//		foreach(Transform gibs in gibObjs)
//		{
//			gibs.gameObject.SetActive(false);
//
//		}

	}

	void FixedUpdate()
	{
//		if(animator)
//		{
//
////			animator.speed = distance * 0.12f;
//		}

	}
	void Update () 
	{
		screenPos2 = mainCam.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y +2.3f, transform.position.z));	
		screenPos2.x -= (screenWidth / 2.0f) - widthOffset;
		screenPos2.y -= (screenHeight / 2.0f) - heightOffset;
		healthSpriteOverInst.transform.localPosition = screenPos2;
		healthSpriteUnderInst.transform.localPosition = screenPos2;
		uiSpriteOver.fillAmount = healthForUI;

//		if(health != 0)
//		{
			healthForUI = health / startHealth;
//			uiSpriteOver.enabled = true;
//			uiSpriteUnder.enabled = true;
//		}
		if((int)health <= 0)
		{
			uiSpriteOver.enabled = false;
			uiSpriteUnder.enabled = false;
		}

		if(countAmount)
		{
			fadeAmt += Time.deltaTime /2;
		}
		screenPos = Camera.main.WorldToScreenPoint (transform.position);
		if(!suckedUp)
		{
			if(screenPos.x > -0.1 &&  screenPos.x < Screen.width +0.1 && screenPos.y > -0.1 && screenPos.y < Screen.height+0.1)
			{
				if(animator == null)
				{
					animator = GetComponentInChildren<Animator>();
				}
				foreach(Transform child in transform)
				{
					if(!child.gameObject.activeSelf)
					{
					child.gameObject.SetActive(true);
					}
				}

				foreach(SkinnedMeshRenderer smr in childrenWithSMR)
				{
					smr.enabled = true;
//					smr.gameObject.SetActive(true);
				}

			}

			else
			{
//				foreach(Transform child in transform)
//				{
//					if(child.gameObject.activeSelf)
//					{
//						child.gameObject.SetActive(false);
//					}
//				}
				foreach(SkinnedMeshRenderer smr in childrenWithSMR)
				{
					smr.enabled = false;
//					smr.gameObject.SetActive(false);
				}
			}
		}


//		if(distance < 22)
//		{
			
//			foreach(Renderer r in childrenWithR)
//			{
//				r.enabled = true;
//			}
//		}
//		else
//		{
			
//			foreach(Renderer r in childrenWithR)
//			{
//				r.enabled = false;
//			}
//		}
		if(abducted && notNaked)
		{
			notNaked = false;
			gibObj.gameObject.SetActive(false);
			armorObj.gameObject.SetActive(false);
			nudeObj.gameObject.SetActive(true);
			skellyObj.gameObject.SetActive(false);
			nakedDeath = true;
			if(gibDeath)
			{
				gibDeath = false;
				if(Random.value > 0.5f)
				{
					skellyDeath = true;
					}
					else
					{
						ragdollDeath = true;
					}
				}
			
			
		}


		if(testAnim)
		{
			animator.animation.enabled = false;
		}
		distance = Vector3.Distance(target.transform.position, this.transform.position);
//		distance = Vector3.Distance(target.transform.position, this.transform.position);
//		distanceToMidobj = Vector3.Distance(midObj.transform.position, this.transform.position);
		if(meleeEnemy)
		{
			MoveTowardsTarget();
			if(distance < meleeDistance)
			{
				inAttackRange = true;
				agent.updatePosition = false;
				//						startAttack = true;
			}
			else if(distance > meleeDistance +1)
			{
				inAttackRange = false;
				MoveTowardsTarget();

				//						startAttack = false;
			}
			if(inAttackRange)
			{
				if(!chargeAnimDone)
				{
					charging = true;
				}
				else
				{
					charging = false;
					attacking = true;
				}
				if(hitAnimDone)
				{
					attacking = false;
					charging = true;
					chargeAnimDone = false;
					hitAnimDone = false;
					StartCoroutine("HitboxHit");
				}
			}
			else
			{
				charging = false;
				attacking = false;
				chargeAnimDone = false;
				hitAnimDone = false;
			}
		}


		if(rangedEnemy)
		{
			MoveTowardsTarget();
			if(distance < rangedDistance)
			{
				inAttackRange = true;
				agent.updatePosition =false;
			}
			else if(distance > rangedDistance +1)
			{
				inAttackRange = false;

			}

			if(inAttackRange)
			{
				if(!chargeAnimDone)
				{
					charging = true;
				}
				else
				{
					charging = false;
					attacking = true;
				}
				if(hitAnimDone)
				{
					attacking = false;
					charging = true;
					chargeAnimDone = false;
					hitAnimDone = false;
//					StartCoroutine("HitboxHit");
					Vector3 dir = target.transform.position - this.transform.position;
					dir.Normalize();
					GameObject shot = Instantiate (projectile,this.transform.position + dir ,Quaternion.identity) as GameObject;
					
					shot.rigidbody.AddForce(dir * 500);
				}
			}
			else
			{
				charging = false;
				attacking = false;
				chargeAnimDone = false;
				hitAnimDone = false;
			}
		}
		if(dead)
		{
			StartCoroutine("Die");
		}
		if(stunned || dead)
		{
			if(agent.enabled)
			{
//				agent.Stop();
//				rigidbody.isKinematic = false;
//				rigidbody.velocity = new Vector3(0,0,0);


			}
		}

//		if(spellSelect)
//		{
//			this.renderer.material.color = spellSelectColor;
//		}
//		else this.renderer.material.color = startColor;


		Vector3 targetDir = target.transform.position - transform.position;
		if(animator)
		{
			animator.SetBool("charging",charging);
			animator.SetBool("attacking",attacking);
			animator.SetFloat("direction", targetDir.normalized.x * targetDir.normalized.z);
			animator.SetBool("attackRange", inAttackRange);
			animator.SetBool("gotHit",hit);
			animator.SetBool("agroed", agroed);
			chargeAnimCount = animator.GetFloat ("chargeDone");
			hitAnimCount = animator.GetFloat("hitDone");
		}
		if(chargeAnimCount > 1 && !chargeAnimDone )
		{
			chargeAnimDone = true;
		}
//		if(charging && chargeAnimDone)
//		{
//			charging = false;
//			StartCoroutine("MeleeAttack");
//		}

		if(hitAnimCount > 1.05 && !hitAnimDone )
		{
			hitAnimDone = true;
		}
		if(attacking && hitAnimDone)
		{
			attacking = false;
		}
		Vector3 forward = transform.forward;
		float angle = Vector3.Angle(targetDir, forward);
		if(agroed && !dead)
		{
			if (angle < 5.0){
	//			print("close");
			}
			else{
				Vector3 _direction = (target.transform.position - transform.position).normalized;
				
				//create the rotation we need to be in to look at the target
				Quaternion _lookRotation = Quaternion.LookRotation(_direction);
				_lookRotation.x = 0;
				_lookRotation.z = 0;
				//rotate us over time according to speed until we are in the required rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 100);
			}
		}
//		if(hit)
//		{
//			StartCoroutine("GotHit");
//		}

		if(!stunned && !dead)
		{

		}
//			if(inAttackRange)
//			{
//				if(!charging)
//				{
//					StartCoroutine("MeleeAttackCharge");
////					Debug.Log ("Charging");
//				}
//				if(rangedEnemy)
//				{
//					StartCoroutine("RangedAttackCharge");
//				}
//
//			}
//			if(attacking && !charging)
//			{
////				if(meleeEnemy)
////				{
////					StartCoroutine("MeleeAttackCharge");
////				}
////				if(rangedEnemy)
////				{
////					StartCoroutine("RangedAttackCharge");
////				}
//			}
//		}

	}

	IEnumerator StepUpdateSlow()
	{
		while(true)
		{
//		Debug.Log ("slowupdt");
		StartCoroutine("RecalculateNearest");
		
		yield return new WaitForSeconds(0.5f);
		}

	}

	IEnumerator StepUpdate()
	{
//		VisionCheck();
			while(true)
			{	
//				if(!targetMid)
//				{
//			if(agroed)
//			{
					VisionCheck();

//			}
//				}

	//			transform.LookAt(new Vector3(0, target.transform.position.y,0));
				agent.updateRotation = true;

				
//				if(meleeEnemy)
//				{
//					if(distance < meleeDistance)
//					{
//						inAttackRange = true;
//						agent.updatePosition = false;
////						startAttack = true;
//					}
//					else if(distance > meleeDistance +1)
//					{
//						inAttackRange = false;
//						MoveTowardsTarget();
////						startAttack = false;
//					}
//					if(inAttackRange)
//					{
//						if(!chargeAnimDone)
//						{
//							charging = true;
//						}
//						else
//						{
//							charging = false;
//							attacking = true;
//						}
//						if(hitAnimDone)
//						{
//							attacking = false;
//							charging = true;
//							chargeAnimDone = false;
//							hitAnimDone = false;
//						}
//					}
//				}
			//					if(inAttackRange)
//						{
//
//						}
//					if(inAttackRange && !attacking)
//					{
//						if(agent.enabled)
//						{
//											agent.updatePosition = false;
//						}
//						attacking = true;
//					}
//					else if(!attacking)
//					{
					
							//						Debug.Log ("moving");
//						MoveTowardsTarget();
							//						StartCoroutine("ResetRot");
//					}

					
			//						if(distance < meleeDistance && !attacking)
//						{
//							if(agent.enabled)
//							{
////								agent.updatePosition = false;
//							}
//							attacking = true;
//						}
//						else if(distance > meleeDistance &&!attacking)
//						{
//
//		//						Debug.Log ("moving");
//								MoveTowardsTarget();
//		//						StartCoroutine("ResetRot");
//						}





			


//			if(rangedEnemy)
//			{
//
//				if(distance < rangedDistance)
//				{
//					inAttackRange = true;
//					agent.updatePosition =false;
//				}
//				else if(distance > rangedDistance +1)
//				{
//					inAttackRange = false;
//					MoveTowardsTarget();
//				}


					//					if(distance < rangedDistance && !attacking)
//					{
//						if(agent.enabled)
//						{
//							agent.updatePosition =false;
//						}
//						attacking = true;
//					}
//					else if(!attacking)
//					{
//					
//						MoveTowardsTarget();
//					}

//			}

			yield return new WaitForSeconds(0.2f);
			}

	}

	public IEnumerator GotHit(float time) 
	{
//		hit = false;
		if(splatter)
		{
//			Debug.Log ();
			splatter.gameObject.GetComponent<Simpleblood>().CastRay(this.transform);
		}
		yield return new WaitForSeconds(time);

		hit = false;
	}


	void MoveTowardsTarget()
	{
		if(!stunned && !dead)
		{
			if(distance < agroRange)
			{
				agroed = true;
			}
			if(chargeEnemy)
			{
				agroed = true;
				iSee = true;
				foreach(Transform child in transform)
				{
					if(!child.gameObject.activeSelf)
					{
						child.gameObject.SetActive(true);
					}
				}
			}
			if(distance < agroRange || agroed)
			{
				if(agent.enabled && iSee)
				{
					agent.SetDestination(new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z));
					agent.updatePosition = true;
				}
			}
		}
	}

	IEnumerator RangedAttackCharge()
	{

		//do animation for shot
		agent.updateRotation = true;
//		agent.enabled = false;
//		Debug.Log ("Charging");
		charging = true;
		yield return new WaitForSeconds(shootChargeTime);
		StartCoroutine("RangedAttack");
	}

	IEnumerator RangedAttack()
	{
		agent.updateRotation = true;
		Vector3 dir = target.transform.position - this.transform.position;
		dir.Normalize();
		GameObject shot = Instantiate (projectile,this.transform.position + dir ,Quaternion.identity) as GameObject;
		
		shot.rigidbody.AddForce(dir * 100);
//		agent.enabled = true;
		yield return null;
		attacking = false;
		charging = false;
	}

	IEnumerator MeleeAttackCharge()
	{
		while(!chargeAnimDone)
		{
			if(!stunned && !dead)
			{
				charging = true;
		//		agent.updateRotation = true;
		//		charging = true;
	//
	//			yield return new WaitForSeconds(attackChargeTime);
	//			charging = false;
	//			StartCoroutine("MeleeAttack");
				yield return null;
			}
		}
		if(chargeAnimDone)
		{
			charging = false;
			attacking = true;
//			StartCoroutine("MeleeAttack");
		}

	}

	IEnumerator MeleeAttack()
	{
		StopCoroutine("MeleeAttackCharge");
		while(!hitAnimDone)
		{
			if(!stunned && !dead)
			{
//				attacking = true;

	//			StartCoroutine("HitboxHit");
	//			attacking = false;
	//			StartCoroutine("ResetRot");
	//			StopCoroutine("MeleeAttack");
				yield return null;
			}
		}
		if(hitAnimDone)
		{
			charging = false;
			attacking = false;
			//			StartCoroutine("MeleeAttack");
		}
	}

	IEnumerator ResetRot()
	{
		if(!stunned && !dead)
		{
//		Debug.Log ("reset");
//		Debug.Log (Mathf.Approximately(swordAnchor.transform.localEulerAngles.x,0));
//		if(!Mathf.Approximately(swordAnchor.transform.localEulerAngles.x,0))
//		{
//			float curRot = swordAnchor.transform.localEulerAngles.x;
//			yield return StartCoroutine(pTween.To(resetRotTime, (float t) => {
//				angle = Mathf.Lerp(curRot, 0, t);
//				swordAnchor.transform.localEulerAngles = new Vector3(angle,0, 0);
//			}));
//		}
			yield return new WaitForSeconds(resetRotTime);
		StartCoroutine("RecalculateNearest");
		}
	}



	IEnumerator HitboxHit()
	{
		swordHitbox.detectHit = true;
		yield return new WaitForSeconds(hitboxTimer);
		swordHitbox.detectHit = false;
	}

	public IEnumerator RecalculateNearest()
	{
		if(!stunned && !decoyed && !dead && !targetMid)
		{
		players = GameObject.FindGameObjectsWithTag("Player");
		
		if(players.Length > 0)
		{
			GameObject closestPlayer = players[0];
			float dist = Vector3.Distance(transform.position, players[0].transform.position);
			
			for(int i = 0;i<players.Length;i++)
			{
				if(players[i].gameObject.GetComponent<PlayerStats>().visible && iSee)
				{
				float tempDist = Vector3.Distance(transform.position, players[i].transform.position);
				if(tempDist < dist) {
					dist = Vector3.Distance(transform.position, players[i].transform.position);

						closestPlayer = players[i];
					}

				}
			}
			target = closestPlayer;
		}
		}
		yield return null;
	}

	void VisionCheck()
	{
//		NavMeshHit hit;
//		
////		 Note the negative test condition! Using -1 for the mask 
////		 indicates all layers are to be used.
//
//		for(int i = 0;i<players.Length;i++)
//		{
//
//			if (!NavMesh.Raycast(this.transform.position, players[i].transform.position,out hit, -1)) 
//			{
//				// Target is "visible" from our position.
//
//				players[i].GetComponent<PlayerStats>().visible = true;
//				iSee = true;
//			}
//			if (NavMesh.Raycast(this.transform.position, players[i].transform.position,out hit, -1)) 
//			{
//
//				players[i].GetComponent<PlayerStats>().visible = false;
//
//			}
//
//
//		}


		for(int i = 0;i<players.Length;i++)
		{
			RaycastHit hit;
//			Debug.Log(Physics.Raycast(transform.position, (players[i].transform.position - transform.position), out hit, agroRange));
			if(Physics.Raycast(new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z), (players[i].transform.position - transform.position), out hit, agroRange))
			{
				if(hit.collider.gameObject.tag == "Player")
				{
//					Debug.DrawRay(transform.position, (players[i].transform.position - transform.position));
					players[i].GetComponent<PlayerStats>().visible = true;
					iSee = true;
				}
			}
//			else
//			{
//				players[i].GetComponent<PlayerStats>().visible = false;
//			}

//			if (Physics.Linecast (transform.position, players[i].transform.position)) {
//				players[i].GetComponent<PlayerStats>().visible = true;
//				iSee = true;
//				
//			}
//			Debug.DrawRay(new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z), (players[i].transform.position - transform.position));
//			if(Physics.Raycast(transform.position, (players[i].transform.position - transform.position), out hit, agroRange))
//				
//			{
//
//				players[i].GetComponent<PlayerStats>().visible = true;
//				iSee = true;
//			}
//			else
//			{
//				players[i].GetComponent<PlayerStats>().visible = false;
//			}
//
		}




	}

	IEnumerator Die()
	{
		if(toSpawnCoins)
		{
			toSpawnCoins = false;
			for(int i = 1; i < coinsToSpawn; i++)
			{
				GameObject coin = Instantiate(coinPref, new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z), Quaternion.identity) as GameObject;
				coin.transform.parent = null;

			}
		}

		if(!deathPlay)
		{
			deathPlay = true;
			audioS.clip = deathSound;
			audioS.Play();
		}

		if(collider)
		{
			collider.enabled = false;
		}
		if(skellyDeath)
		{

			foreach(Collider col in ragdollObjs)
			{
				col.enabled = true;
			}
			gibObj.gameObject.SetActive(false);
			animator.enabled = false;
			armorObj.gameObject.SetActive(false);
			nudeObj.gameObject.SetActive(false);
			skellyObj.gameObject.SetActive(true);
			agent.enabled = false;
			if(!moveDeath)
			{
				moveDeath = true;
//				rigidbody.AddForce(this.transform.position,ForceMode.Impulse);
//				transform.position = new Vector3(transform.position.x, transform.position.y +2.5f, transform.position.z);
			}
			yield return new WaitForEndOfFrame();
			foreach(Collider col in ragdollObjs)
			{
				col.rigidbody.useGravity = true; 
			}

		}
		if(gibDeath)
		{
//			gibObj.gameObject.SetActive(false);
			animator.enabled = false;
//			foreach(Transform gibs in gibObjs)
//			{
//				gibs.gameObject.SetActive(true);
////				gibs.gameObject.collider.enabled = true;
////				gibs.gameObject.rigidbody.isKinematic = false;
////				gibs.gameObject.transform.parent = null;
//			}
			gibObj2.gameObject.SetActive(true);
			gibObj.gameObject.SetActive(false);
			armorObj.gameObject.SetActive(false);
			nudeObj.gameObject.SetActive(false);
			skellyObj.gameObject.SetActive(false);
		}
					agent.enabled = false;
				
		if(ragdollDeath)
		{
			if(!nakedDeath)
			{

			foreach(Collider col in ragdollObjs)
			{
				col.enabled = true;
			}
				animator.enabled = false;
				armorObj.gameObject.SetActive(false);
				nudeObj.gameObject.SetActive(false);
				skellyObj.gameObject.SetActive(false);
				agent.enabled = false;
			}
			else
			{
				foreach(Collider col in ragdollObjs)
				{
					col.enabled = true;
				}
				animator.enabled = false;
				gibObj.gameObject.SetActive(false);
				armorObj.gameObject.SetActive(false);
		//		nudeObj.gameObject.SetActive(false);
				skellyObj.gameObject.SetActive(false);
				agent.enabled = false;
				}
				yield return new WaitForEndOfFrame();
				foreach(Collider col in ragdollObjs)
				{
				col.rigidbody.useGravity = true;
				}	

//		gameObject.SetActive(false);
		}
		yield return new WaitForEndOfFrame();
		childrenWithR = gameObject.GetComponentsInChildren<Renderer>();
		childrenWithSMR = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		//		foreach(SkinnedMeshRenderer smr in childrenWithSMR)
		//		{
		//			smr.material = cutoutMat;
		//		}
//		foreach(Renderer r in childrenWithR)
//		{
//			r.material = cutoutMat;
//		}
		yield return new WaitForSeconds(Random.Range(dieTime,dieTime+5));
//		float amt = 0;
		countAmount = true;
//		while(amt < 1)
//		{

//			amt += Time.deltaTime /2;
//			Debug.Log (amt);
			foreach(Renderer r in childrenWithR)
			{
				r.material = cutoutMat;
				r.material.SetFloat("_SliceAmount",fadeAmt);
			}
		yield return new WaitForSeconds(2);
		gameObject.SetActive(false);
//		}

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon (transform.position, "Picture_portraitG.jpg", true);
	}


}

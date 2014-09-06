using UnityEngine;
using System.Collections;
//using GamepadInput;
using System.Collections.Generic;
using XInputDotNetPure;

public class PlayerAttack : MonoBehaviour {
	public enum ButtonOne { melee, ranged};
	public ButtonOne button1;
	public enum ButtonTwo { melee, ranged};
	public ButtonTwo button2;

	public GameObject shot;
	public GameObject muzzleFlash;
	public float shotRange;
	public float shotAmt;
	public float shotSpeed = 200f;
	public float shotCooldown = 0.5f;

	public bool spreadShot;
	public float shotSpread;
	public bool swinging;
	public float swingTime = 0.3f;
	public float swingSpeed = 10f;
//	public GameObject swordAnchor;
	public float killTime = 0.3f;
	public float minAngle;
	public float maxAngle = 360f;
//	float mTime = 0f;
	float angle;
	public float backlashForce = 100f;
	public float backlashDuration = 0.2f;

	public Transform explosionPoint;
	public AnimationCurve swingCurve;
	public Controls controls;
	public Movement movement;

	public GameObject splatter;
	public int amountOfSplatter = 5;
	public float chargeCounter;
	public bool countingCharge;
	public float chargePlace = 0.3f;
	private float meleeDmg;
	public float meleeAttackDmg = 1;
	public float meleeChargeDmg = 3;
	bool hitEnemy;
	public Animator animator;
	public bool hitSomething;
	public List<GameObject> insideCol = new List<GameObject>();
	public List<GameObject> enemyInCol = new List<GameObject>();
	public List<GameObject> gObs = new List<GameObject>();

	public Transform parent;

	public AudioClip playerAttackSound;
	public AudioSource audioSource;
	public AudioClip playerHitSound;
	public AudioClip hitDoorSound;
	public GameObject hitDoorEffect;
	PlayerIndex player1 = PlayerIndex.One;
	PlayerIndex player2 = PlayerIndex.Two;
	PlayerIndex player3 = PlayerIndex.Three;
	PlayerIndex player4 = PlayerIndex.Four;
	bool pressed;
	bool shotPressed;

	public Transform trailObj;
	public GameObject hitEffect;
	private GameObject hitEffectInst;
	private bool swingType;

	void Start () {
		audioSource = transform.parent.GetComponent<AudioSource>();

		animator = transform.parent.GetComponentInChildren<Animator>();
		minAngle = this.transform.eulerAngles.z;
		if(!spreadShot)
		{
			shotAmt = 1;
		}
		hitEffectInst = Instantiate(hitEffect,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		hitEffectInst.SetActive(false);
	}
	
	void Update () {

//		trailObj.gameObject.GetComponent<WeaponTrail>().Itterate(0.01f);
//		trailObj.gameObject.GetComponent<WeaponTrail>().UpdateTrail(0.01f,Time.deltaTime);
		GamePadState p1 = GamePad.GetState(player1);
		GamePadState p2 = GamePad.GetState(player2);
		GamePadState p3 = GamePad.GetState(player3);
		GamePadState p4 = GamePad.GetState(player4);

		parent = transform.parent;

		if(animator)
		{
			animator.SetBool("attack",swinging);
			animator.SetBool("charging",countingCharge);
//			animator.SetInteger
			animator.SetBool("swingType", swingType); 
		}
		if(!Camera.main.GetComponent<PauseMenu>().paused)
		{
		if(button1 == ButtonOne.melee)
		{
			switch(controls.playerNumber)
			{
			case 1:
//				if(GamePad.GetButtonDown (GamePad.Button.A, GamePad.Index.One) && !swinging)
				if(p1.Buttons.A == ButtonState.Pressed && !swinging && !pressed)
				{

					pressed = true;
					meleeDmg = meleeAttackDmg;
					StartCoroutine("Swing");
					StartCoroutine("StartAttack");
					StartCoroutine("HitObject");
//					StartCoroutine("HitEnemy");
					GamePad.SetVibration(player1,0.1f, 0.3f);

				}
//				if(GamePad.GetButtonUp (GamePad.Button.A, GamePad.Index.One) && countingCharge)
				if(p1.Buttons.A == ButtonState.Released)
				{
					pressed = false;
					countingCharge = false;
					StartCoroutine("CheckIfCharge");
					chargeCounter = 0;
					GamePad.SetVibration(player1,0, 0);

				}
				break;
				
			case 2:
				if(p2.Buttons.A == ButtonState.Pressed && !swinging && !pressed)
				{
					pressed = true;
					meleeDmg = meleeAttackDmg;
					StartCoroutine("Swing");
					StartCoroutine("StartAttack");
					StartCoroutine("HitObject");
					GamePad.SetVibration(player2,0.1f, 0.3f);
				
				}
				if(p2.Buttons.A == ButtonState.Released)
				{
					pressed = false;
					countingCharge = false;
					StartCoroutine("CheckIfCharge");
					chargeCounter = 0;
					GamePad.SetVibration(player2,0, 0);
				
					
				}
				break;
				
			case 3:
				if(p3.Buttons.A == ButtonState.Pressed && !swinging && !pressed)
				{
					pressed = true;
					meleeDmg = meleeAttackDmg;
					StartCoroutine("Swing");
					StartCoroutine("StartAttack");
					StartCoroutine("HitObject");
					GamePad.SetVibration(player3,0.1f, 0.3f);
					
				}
				if(p3.Buttons.A == ButtonState.Released)
				{
					pressed = false;
					countingCharge = false;
					StartCoroutine("CheckIfCharge");
					chargeCounter = 0;
					GamePad.SetVibration(player3,0, 0);

					
				}
				break;
				
			case 4:
				if(p4.Buttons.A == ButtonState.Pressed && !swinging && !pressed)
				{
					pressed = true;
					meleeDmg = meleeAttackDmg;
					StartCoroutine("Swing");
					StartCoroutine("StartAttack");
					StartCoroutine("HitObject");
					GamePad.SetVibration(player4,0.1f, 0.3f);
					
				}
				if(p4.Buttons.A == ButtonState.Released)
				{
					pressed = false;
					countingCharge = false;
					StartCoroutine("CheckIfCharge");
					chargeCounter = 0;
					GamePad.SetVibration(player4,0, 0);
				}
				break;
				
			default:
				break;
				
			}
		}

//		if(button2 == ButtonTwo.melee)
//		{
//			switch(controls.playerNumber)
//			{
//			case 1:
//				if(GamePad.GetButtonDown (GamePad.Button.B, GamePad.Index.One) && !swinging)
//				{
//					meleeDmg = meleeAttackDmg;
//					StartCoroutine("Swing");
//					StartCoroutine("StartAttack");
//					
//				}
//				if(GamePad.GetButtonUp (GamePad.Button.B, GamePad.Index.One) && countingCharge)
//				{
//					countingCharge = false;
//					StartCoroutine("CheckIfCharge");
//					chargeCounter = 0;
//					
//				}
//				break;
//				
//			case 2:
//				if(GamePad.GetButtonDown (GamePad.Button.B, GamePad.Index.Two) && !swinging)
//				{
//					meleeDmg = meleeAttackDmg;
//					StartCoroutine("Swing");
//					StartCoroutine("StartAttack");
//					
//				}
//				if(GamePad.GetButtonUp (GamePad.Button.B, GamePad.Index.Two) && countingCharge)
//				{
//					countingCharge = false;
//					StartCoroutine("CheckIfCharge");
//					chargeCounter = 0;
//					
//				}
//				break;
//				
//			case 3:
//				if(GamePad.GetButtonDown (GamePad.Button.B, GamePad.Index.Three) && !swinging)
//				{
//					meleeDmg = meleeAttackDmg;
//					StartCoroutine("Swing");
//					StartCoroutine("StartAttack");
//					
//				}
//				if(GamePad.GetButtonUp (GamePad.Button.B, GamePad.Index.Three) && countingCharge)
//				{
//					countingCharge = false;
//					StartCoroutine("CheckIfCharge");
//					chargeCounter = 0;
//					
//				}
//				break;
//				
//			case 4:
//				if(GamePad.GetButtonDown (GamePad.Button.B, GamePad.Index.Four) && !swinging)
//				{
//					meleeDmg = meleeAttackDmg;
//					StartCoroutine("Swing");
//					StartCoroutine("StartAttack");
//					
//				}
//				if(GamePad.GetButtonUp (GamePad.Button.B, GamePad.Index.Four) && countingCharge)
//				{
//					countingCharge = false;
//					StartCoroutine("CheckIfCharge");
//					chargeCounter = 0;
//					
//				}
//				break;
//				
//			default:
//				break;
//				
//			}
//		}

//		if(button1 == ButtonOne.ranged)
//		{
			switch(controls.playerNumber)
			{
				case 1:
				//				if(GamePad.GetButtonDown (GamePad.Button.A, GamePad.Index.One) && !swinging)
				if(p1.Buttons.RightShoulder == ButtonState.Pressed && !shotPressed)
				{
					
										StartCoroutine("Shoot");
					shotPressed = true;
					
				}

				break;
				
				case 2:
				if(p2.Buttons.RightShoulder == ButtonState.Pressed &&  !shotPressed)
				{
					shotPressed = true;
					StartCoroutine("Shoot");

					
				}

				break;
				
				case 3:
				if(p3.Buttons.RightShoulder == ButtonState.Pressed && !shotPressed)
				{
					shotPressed = true;
					StartCoroutine("Shoot");

					
				}

				break;
				
				case 4:
				if(p4.Buttons.RightShoulder == ButtonState.Pressed && !shotPressed)
				{
										shotPressed = true;

					StartCoroutine("Shoot");

				}

				break;
				
				default:
				break;
				
			}
//		}

//		if(button2 == ButtonTwo.ranged)
//		{
//			switch(controls.playerNumber)
//			{
//			case 1:
////				if(GamePad.GetButtonDown (GamePad.Button.B, GamePad.Index.One))
//				if(p1.Buttons.B == ButtonState.Pressed && !shotPressed)
//				{
//					shotPressed = true;
//					StartCoroutine("Shoot");
//
//				}
//				break;
//				
//			case 2:
//				if(p2.Buttons.B == ButtonState.Pressed && !shotPressed)
//				{
//					shotPressed = true;
//					StartCoroutine("Shoot");
//
//				}
//				break;
//				
//			case 3:
//				if(p3.Buttons.B == ButtonState.Pressed && !shotPressed)
//				{
//					shotPressed = true;
//					StartCoroutine("Shoot");
//					
//				}
//				break;
//				
//			case 4:
//				if(p4.Buttons.B == ButtonState.Pressed && !shotPressed)
//				{
//					shotPressed = true;
//					StartCoroutine("Shoot");
//					
//				}
//				break;
//				
//			default:
//				break;
//				
//			}
//		}
		}
		
		
	}
	IEnumerator Backlash(GameObject col)
	{
		bool move = false;
		float mTime = 0;
		while(!move)
		{
			if(mTime < backlashDuration)
			{
				mTime += Time.deltaTime;
//				Debug.Log ("???");
	//			Debug.Log ("lala");
//				Enemy enm = col.GetComponent<Enemy>();
	//			enm.stunned = true;
	//			col.rigidbody.isKinematic = false;
	//			col.collider.enabled = false;
	//			Debug.Log (col.name);
	//			rigidbody.MovePosition(rigidbody.position + new Vector3(100,0,0) * Time.deltaTime);
	//			col.rigidbody.AddForce((-transform.forward * 3 - transform.parent.position).normalized * backlashForce, ForceMode.Impulse);
				
	//			enm.agent.enabled = false;
				Vector3 dir = (col.rigidbody.position - transform.parent.position).normalized;
				col.rigidbody.MovePosition(col.rigidbody.position + dir * backlashForce * Time.deltaTime);
				

	//			enm.agent.enabled = true;
//				if(col)
//				{
	//				col.collider.enabled = true;
					
	//				col.rigidbody.isKinematic = true;
//				}
			}
//			else move = true;

			yield return null;
		}
		yield return new WaitForSeconds(backlashDuration);
		move = true;
	}
	void OnTriggerEnter(Collider col)
	{
		insideCol.Add(col.gameObject);
	}
	void OnTriggerExit(Collider col)
	{
		insideCol.Remove(col.gameObject);
	}

	IEnumerator OnTriggerStay(Collider col)
	{		
		bool temp = false;
		if(col.gameObject.tag == "enemy" && swinging && !col.gameObject.GetComponent<Enemy>().hit)
		{
			audioSource.clip = playerHitSound;
			audioSource.Play();

			Enemy colEnm = col.gameObject.GetComponent<Enemy>();
			StartCoroutine("Backlash",col.gameObject);
			colEnm.hit = true;
			colEnm.StartCoroutine("GotHit",swingTime);
//			Debug.Log ("hit");
//			for (int i = 0;i < amountOfSplatter; i++) 
//			{
//				GameObject splat = Instantiate(splatter,col.gameObject.transform.position, Quaternion.identity) as GameObject;
//				splat.rigidbody.AddExplosionForce(100f,col.gameObject.transform.position * 2,100f);
//			}

			GameObject splat = Instantiate(splatter,new Vector3(col.gameObject.transform.position.x,col.gameObject.transform.position.y +1.5f,col.gameObject.transform.position.z), Quaternion.identity) as GameObject;


//			Vector3 dir = (parent.transform.position - new Vector3(col.gameObject.transform.position.x,col.gameObject.transform.position.y +1.5f,col.gameObject.transform.position.z)).normalized;
			GameObject hitE = Instantiate(hitEffect,new Vector3(col.gameObject.transform.position.x,col.gameObject.transform.position.y +1.5f,col.gameObject.transform.position.z),Quaternion.identity) as GameObject;
			hitE.transform.rotation = parent.transform.rotation;

//			hitEffectInst.SetActive(true);
//			hitEffectInst.transform.position = col.gameObject.transform.position;

//			if(col.GetComponent<Enemy>().health <= 0)
//			{

//			col.gameObject.GetComponent<Enemy>().dead = true;
			colEnm.health -= meleeDmg;
			yield return new WaitForSeconds(backlashDuration);

//			StopCoroutine("Backlash");
			if(col)
			{
				if(colEnm.health <= 0)
				{
//					colEnm.animator.enabled = false;
					colEnm.dead = true;

				}
				col.rigidbody.isKinematic = true;
			}
			yield return new WaitForSeconds(2);
			splat.SetActive(false);

////			yield return new WaitForSeconds(swingTime - backlashDuration);
////			Destroy (splat);
////			if(col)
////			{
////				col.gameObject.GetComponent<Enemy>().hit = false;
////			}
//				//				StartCoroutine("KillObj",col.gameObject);
//				
////			}
////			else 
////			{	col.rigidbody.isKinematic = false;
////				col.rigidbody.AddForce((col.transform.position - transform.position).normalized * 3, ForceMode.Impulse);
////				yield return new WaitForSeconds(0.2f);
////				if(col)
////				{
////					col.rigidbody.isKinematic = true;
////				}
////			}
//
//
		}
		if(col)
		{

			if(col.gameObject.tag == "destruct" && swinging && !col.gameObject.GetComponent<DestroyItem>().destroy)
			{					
				audioSource.clip = hitDoorSound;
				audioSource.PlayOneShot(hitDoorSound);
				col.gameObject.GetComponent<DestroyItem>().destroy = true;
				col.gameObject.GetComponent<DestroyItem>().player = this.transform;
				insideCol.Remove(col.gameObject);
//				GameObject hitD = Instantiate(hitDoorEffect,new Vector3(this.transform.position.x,this.transform.position.y,this.transform.position.z),Quaternion.identity) as GameObject;
//				hitD.transform.rotation = parent.transform.rotation;
			}

//			Debug.Log (temp);
//			if(col.gameObject.tag == "hitAndMove" && swinging)
//			{
//				col.rigidbody.AddForce((col.transform.position - transform.parent.position).normalized * (backlashForce/2), ForceMode.Impulse);
//				col.rigidbody.AddTorque (new Vector3(10,10,10));
////				temp = true;
//				 
//			}
		}
		yield return null;
	}
		
	IEnumerator StartAttack()
	{


//		yield return new WaitForSeconds(swingTime);
		if(pressed)
		{
			countingCharge = true;
			while(countingCharge)
			{

				chargeCounter += Time.deltaTime * 1;

				yield return null;
			}
		}

	}

	void CheckIfCharge()
	{	
		if(chargeCounter > chargePlace)
		{

			meleeDmg = meleeChargeDmg;
			//Charged swing

			StartCoroutine("Swing");

		}
		chargeCounter = 0;

	}
	IEnumerator Swing()
	{
		swinging = true;
//		
		if(Random.value > 0.5f) swingType = true;
		else swingType = false;

		if(trailObj)
		{
		StartCoroutine("TrailLerp");
//		trailObj.gameObject.renderer.enabled = true;
		}
//		trailObj.gameObject.GetComponent<WeaponTrail>().StartTrail(0.01f,0.01f);
		audioSource.clip = playerAttackSound;
		audioSource.Play();
//		movement.speed = movement.speedWhenAtttacking;
		yield return new WaitForSeconds(swingTime);
//		trailObj.gameObject.GetComponent<WeaponTrail>().ClearTrail();
//		trailObj.gameObject.renderer.enabled = false;
//		yield return StartCoroutine(pTween.To(swingTime, (float t) => {
//			angle = Mathf.Lerp(minAngle, maxAngle, t);
//			swordAnchor.transform.localEulerAngles = new Vector3(0,- angle, 0);
//		}));
//		if(trailObj)
//		{
//			trailObj.gameObject.renderer.enabled = false;
//		}
//		movement.speed = movement.startSpeed;
		hitSomething = false;
		swinging = false;
		hitEnemy = false;
		trailObj.gameObject.renderer.enabled = false;

	}

	IEnumerator TrailLerp()
	{
		float mTime = 0;
		if(swingType)
		{
			mTime = 0;
		}
		else
		{
			mTime = 1.5f;

		}
		bool onOff = true;
		trailObj.gameObject.renderer.enabled = true;
		if(swingType)
		{
			while(onOff)
			{
				if(mTime < 1.5f)
				{
					mTime += Time.deltaTime *10f;
	//				Debug.Log(mTime);
					trailObj.renderer.material.SetFloat("_CutOff",mTime);
				}
				else
				{
					onOff = false;
					trailObj.gameObject.renderer.enabled = false;

				}
				yield return null;
			}
		}

		if(!swingType)
		{
//			trailObj.renderer.material.SetFloat("_CutOff",1.5f);
//			mTime = 0;
			while(onOff)
			{
				if(mTime > 0)
				{
					mTime -= Time.deltaTime *10f;
					//				Debug.Log(mTime);
					trailObj.renderer.material.SetFloat("_CutOff",mTime);
				}
				else
				{
					onOff = false;
					trailObj.gameObject.renderer.enabled = false;
					
				}
				yield return null;
			}
		}
	}

	IEnumerator Shoot()
	{
		Vector3 dir = movement.inputDir - this.transform.position;
		dir.Normalize();
		for(int i = 0; i <= shotAmt-1; i++)
		{

			GameObject shotPref = Instantiate(shot,this.transform.position + ( this.transform.forward *1.4f), transform.rotation) as GameObject;
			GameObject muzzlePref = Instantiate(muzzleFlash,new Vector3(this.transform.position.x + ( this.transform.forward.x * 1.01f), this.transform.position.y + 0.05f, this.transform.position.z + (this.transform.forward.z * 1.01f)), transform.rotation) as GameObject;
//			shotPref.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y * Random.Range(-2,2), transform.rotation.z);
			if(spreadShot)
			{
				shotPref.transform.Rotate (0,Random.Range(-shotSpread,shotSpread),0);
			}
			shotPref.rigidbody.AddForce(shotPref.transform.forward * shotSpeed);
		}
		yield return new WaitForSeconds(shotCooldown);
		shotPressed = false;
	}

	IEnumerator HitObject()
	{
		foreach(GameObject gObj in insideCol) 
		{
			if(gObj.tag == "hitAndMove")
			{
				audioSource.clip = hitDoorSound;
				audioSource.Play();
				gObj.rigidbody.AddForce((gObj.transform.position - transform.parent.position).normalized * (backlashForce), ForceMode.Impulse);
				gObj.rigidbody.AddTorque (new Vector3(10,10,10));
				GameObject hitD = Instantiate(hitDoorEffect,new Vector3(this.transform.position.x,this.transform.position.y,this.transform.position.z),Quaternion.identity) as GameObject;
				hitD.transform.rotation = parent.transform.rotation;
			}
		}

		foreach(GameObject gObj2 in insideCol) 
		{
			if(gObj2.gameObject.tag == "destruct" && swinging && !gObj2.gameObject.GetComponent<DestroyItem>().destroy)
			{					
				audioSource.clip = hitDoorSound;
				audioSource.PlayOneShot(hitDoorSound);
				gObj2.gameObject.GetComponent<DestroyItem>().destroy = true;
				gObj2.gameObject.GetComponent<DestroyItem>().player = this.transform;
				gObs.Add(gObj2.gameObject);
				GameObject hitD = Instantiate(hitDoorEffect,new Vector3(this.transform.position.x,this.transform.position.y,this.transform.position.z),Quaternion.identity) as GameObject;
				hitD.transform.rotation = parent.transform.rotation;
			}
		}
		foreach(GameObject gO in gObs)
		{
			insideCol.Remove(gO);
		}
		yield return null;
	}

//	IEnumerator HitEnemy()
//	{
//		foreach(GameObject gObj2 in insideCol)
//		{
//			if(gObj2.gameObject.tag == "enemy")
//			{
//				enemyInCol.Add (gObj2);
//			}
//		}
//		foreach(GameObject gObj in enemyInCol)
//		{
//			if(swinging && !gObj.gameObject.GetComponent<Enemy>().hit)
//			{
//				audioSource.clip = playerHitSound;
//				audioSource.Play();
//				
//				Enemy colEnm = gObj.gameObject.GetComponent<Enemy>();
//				StartCoroutine("Backlash",gObj.gameObject);
//				colEnm.hit = true;
//				colEnm.StartCoroutine("GotHit",swingTime);
//
//				
//				GameObject splat = Instantiate(splatter,new Vector3(gObj.gameObject.transform.position.x,gObj.gameObject.transform.position.y +1.5f,gObj.gameObject.transform.position.z), Quaternion.identity) as GameObject;
//				
//				
//				GameObject hitE = Instantiate(hitEffect,new Vector3(gObj.gameObject.transform.position.x,gObj.gameObject.transform.position.y +1.5f,gObj.gameObject.transform.position.z),Quaternion.identity) as GameObject;
//				hitE.transform.rotation = parent.transform.rotation;
//				
//
//				colEnm.health -= meleeDmg;
//				yield return new WaitForSeconds(backlashDuration);
//
//				if(gObj)
//				{
//					if(colEnm.health <= 0)
//					{
//						colEnm.dead = true;
//						
//					}
//					gObj.rigidbody.isKinematic = true;
//				}
//				yield return new WaitForSeconds(2);
//				splat.SetActive(false);
//
//				
//			}
//		}
//	}



}

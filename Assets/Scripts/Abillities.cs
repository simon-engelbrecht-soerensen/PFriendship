using UnityEngine;
using System.Collections;
//using GamepadInput;
using XInputDotNetPure;
public class Abillities : MonoBehaviour {

	public enum Ability { Stun, Push, Decoy, Suck};
	public Ability ability;
	public Controls controls;
	public float overlapSize = 5f;
//	public float overlapSize = 5f;
	public float manaCost = 50f;
	public float totalMana = 100;
	public float moveSpeed;
	public float maxDist = 10f;
	public bool useOnHold;
	public float spellDuration = 5f;
	public float pushForce = 25;
//	public bool useOnRelease;
	public Collider[] overlapSphere;
	public Collider[] overlapSphere2;
	private Vector3 startPos;
	private bool visible;
	private Vector2 rightStick1;
	private Vector2 rightStick2;
	private Vector2 rightStick3;
	private Vector2 rightStick4;
	private Vector2 rightStick;
	public Vector3 newPos;
	public Vector3 newPos2;

	private bool resetPos;
	private bool pressed;
	private Vector3 playerMovement;
	private Vector3 offset;
	public GameObject decoy;
	public GameObject spellMarker;
	public GameObject spellMarkerPush;
	private GameObject spellMarkerInst;
	private GameObject spellMarkerPushInst;
//	public GameObject spellCastObj;
//	public GameObject testObj;
//	public Vector3 velocity;
//	public Vector3 previous;
//	private Collider[] overlapSphere;
	private Vector3 lastDir;
	public GameObject abductBeam;

	PlayerIndex player1 = PlayerIndex.One;
	PlayerIndex player2 = PlayerIndex.Two;
	PlayerIndex player3 = PlayerIndex.Three;
	PlayerIndex player4 = PlayerIndex.Four;

	void Start () 
	{
		newPos = startPos;
		spellMarkerInst = Instantiate(spellMarker,transform.position,Quaternion.identity) as GameObject;
		spellMarkerInst.renderer.enabled = false;
		spellMarkerInst.transform.parent = this.transform;

		spellMarkerPushInst = Instantiate(spellMarkerPush,transform.position,Quaternion.identity) as GameObject;
		spellMarkerPushInst.SetActive(false);
//		spellMarkerPushInst.transform.parent = this.transform;
		offset = startPos;
	}
	

	void Update () 
	{
		if(totalMana < 100)
		{
			totalMana += Time.deltaTime * 15;
		}
		GamePadState p1 = GamePad.GetState(player1);
		GamePadState p2 = GamePad.GetState(player2);
		GamePadState p3 = GamePad.GetState(player3);
		GamePadState p4 = GamePad.GetState(player4);

		newPos = this.transform.position + (transform.forward * 3);

		spellMarkerInst.transform.position = newPos;
//		if(ability != Ability.Push)
//		{
//			newPos = offset + transform.position;
//			newPos = RotateAroundPoint(this.transform.position, Vector3.zero, Quaternion.Euler(90,0,0));
//		}
//		spellMarker.transform.position = newPos;
//		if(spellMarkerInst)
//		{
////			spellMarkerInst.transform.position = newPos;
//		}
//		if(spellMarkerPushInst)
//		{
////			spellMarkerPushInst.transform.position = newPos;
////			spellMarkerPushInst.transform.LookAt(this.transform.position);
//		}

//		Debug.Log (rightStick);
//		Debug.Log (newPos + this.transform.position);
//		offset = startPos - this.transform.position
//		Vector3 delt = startPos - this.transform.position;
//		delt.Normalize();

//		testObj.transform.position = testObj.transform.position + (new Vector3(rightStick.x , 0, rightStick.y) * moveSpeed) * Time.deltaTime;


//		Debug.Log (newPos);
//
//		velocity = (transform.position - previous) / Time.deltaTime; 
//		previous = transform.position;

		//		newPos = (new Vector3(rightStick.x, 0, rightStick.y) * moveSpeed) + offset;
//		rightStick1 = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One);
//		rightStick2 = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.Two);
//		rightStick3 = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.Three);
//		rightStick4 = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.Four);
		rightStick1 = new Vector2(p1.ThumbSticks.Right.X, p1.ThumbSticks.Right.Y);
		rightStick2 = new Vector2(p2.ThumbSticks.Right.X, p2.ThumbSticks.Right.Y);

		rightStick3 = new Vector2(p3.ThumbSticks.Right.X, p3.ThumbSticks.Right.Y);

		rightStick4 = new Vector2(p4.ThumbSticks.Right.X, p4.ThumbSticks.Right.Y);



//		startPos = transform.TransformDirection(Vector3.forward) * 2;
		startPos = transform.forward * 2;
//		Debug.DrawRay (transform.position, startPos, Color.green);


//		Debug.DrawLine(this.transform.position, transform.TransformDirection(Vector3.forward) * 1);
//		overlapSphere = Physics.OverlapSphere(this.transform.position,overlapSize);
		switch(controls.playerNumber)
		{
		case 1:
			rightStick = rightStick1;
//			if(GamePad.GetTrigger (GamePad.Trigger.RightTrigger, GamePad.Index.One) > 0.1f && !pressed)
			if(p1.Triggers.Right  > 0.1f && !pressed)
			{
				pressed = true;
				StartCoroutine("AbilityHold", ability);
				visible = true;

			}
//			else if(GamePad.GetTrigger (GamePad.Trigger.RightTrigger, GamePad.Index.One) < 0.1f && pressed )
			else if(p1.Triggers.Right < 0.1f && pressed)
			{
				pressed = false;
				visible = false;
				StopCoroutine("AbilityHold");
				StartCoroutine("AbilityCast", ability);
				spellMarkerInst.renderer.enabled = false;
				spellMarkerPushInst.SetActive(false);
				if(totalMana >= manaCost)
				{
					totalMana -= manaCost;
				}

//				spellTimer = 0f;

			}
			break;
			
		case 2:
			rightStick = rightStick2;
			if(p2.Triggers.Right  > 0.1f && !pressed)	
			{
				pressed = true;
				StartCoroutine("AbilityHold", ability);
				visible = true;
				Debug.Log ("?!?!");
			}
			else if(p2.Triggers.Right < 0.1f && pressed)
			{
				pressed = false;
				visible = false;
				StopCoroutine("AbilityHold");
				StartCoroutine("AbilityCast", ability);	
				spellMarkerInst.renderer.enabled = false;
				spellMarkerPushInst.SetActive(false);
				if(totalMana >= manaCost)
				{
					totalMana -= manaCost;
				}
			}
			break;
			
		case 3:
			rightStick = rightStick3;
			if(p3.Triggers.Right > 0.1f && !pressed)			
			{
				
				pressed = true;
				StartCoroutine("AbilityHold", ability);
				visible = true;
			}
			else if(p3.Triggers.Right < 0.1f && pressed)
			{
				pressed = false;
				visible = false;
				StopCoroutine("AbilityHold");
				StartCoroutine("AbilityCast", ability);		
				spellMarkerInst.renderer.enabled = false;
				spellMarkerPushInst.SetActive(false);
				if(totalMana >= manaCost)
				{
					totalMana -= manaCost;
				}
			}
			break;
			
		case 4:
			rightStick = rightStick4;
			if(p4.Triggers.Right > 0.1f && !pressed)			
			{
				
				pressed = true;
				StartCoroutine("AbilityHold", ability);
				visible = true;
			}
			else if(p4.Triggers.Right < 0.1f && pressed)
			{
				pressed = false;
				visible = false;
				StopCoroutine("AbilityHold");
				StartCoroutine("AbilityCast", ability);	
				spellMarkerInst.renderer.enabled = false;
				spellMarkerPushInst.SetActive(false);
				if(totalMana >= manaCost)
				{
					totalMana -= manaCost;
				}
			}
			break;
			
		default:
			break;
			
		}


	}

	IEnumerator AbilityHold(Ability ability)
	{
//		Debug.Log ("hold");

			while(pressed)
			{
				if(totalMana >= manaCost)
				{
					if(ability == Ability.Decoy || ability == Ability.Suck || ability == Ability.Stun)
					{
						
//						offset = offset + (new Vector3(rightStick.x , 0, rightStick.y) * moveSpeed) * Time.deltaTime;
//						float distance = Vector3.Distance (this.transform.position, newPos);
	//					newPos = transform.position + offset;
	//					newPos = offset + transform.position;
	//					spellMarker.renderer.enabled = true;
						
//						offset = Vector3.ClampMagnitude(offset, maxDist);
						
						spellMarkerInst.renderer.enabled = true;
		
						
						
						
	//					Debug.Log (distance);
						
						overlapSphere = Physics.OverlapSphere(newPos,overlapSize);
						overlapSphere2 = Physics.OverlapSphere(newPos,overlapSize + 0.2f);
						
						foreach(Collider col in overlapSphere)
						{
							if(col.gameObject.tag == "enemy")
							{
								col.gameObject.GetComponent<Enemy>().spellSelect = true;
	//							col.gameObject.renderer.material.color = col.gameObject.GetComponent<Enemy>().spellSelectColor;
							}
						}
						foreach(Collider col in overlapSphere2)
						{
							if(col.gameObject.tag == "enemy" && Vector3.Distance(newPos, col.transform.position) > overlapSize)
							{
								col.gameObject.GetComponent<Enemy>().spellSelect = false;
	//							col.gameObject.renderer.material.color = col.gameObject.GetComponent<Enemy>().startColor;
							}
						}
					}

				if(ability == Ability.Push)
				{
//					spellMarkerPushInst.SetActive(true);
					spellMarkerInst.renderer.enabled = true;
//					Vector3 dir = new Vector3(rightStick.x , 0, rightStick.y);
//					dir = dir.normalized;
//					Vector3 dirKeep = Vector3.zero;
	//				if(dir.x != 0 && dir.z != 0)
	//				{
	//					newPos = transform.position + dir * maxDist;
	//				}

	//				if(dir != lastDir)
	//				{
	//					newPos = transform.position + dir * maxDist;
	//				}
//					newPos = this.transform.position + (transform.forward * 3);
//					newPos = (newPos - transform.position + dir).normalized * maxDist + transform.position;
	//				transform.position - otherObject.transform.position).normalized * distance + otherObject.transform.position;
	//				Debug.Log (dir);
//
//					lastDir = dir;
					overlapSphere = Physics.OverlapSphere(newPos,overlapSize);
					overlapSphere2 = Physics.OverlapSphere(newPos,overlapSize + 0.2f);
					foreach(Collider col in overlapSphere)
					{
						if(col.gameObject.tag == "enemy")
						{
							col.gameObject.GetComponent<Enemy>().spellSelect = true;
	//						col.gameObject.renderer.material.color = col.gameObject.GetComponent<Enemy>().spellSelectColor;
						}
					}
					foreach(Collider col in overlapSphere2)
					{
						if(col.gameObject.tag == "enemy" && Vector3.Distance(newPos, col.transform.position) > overlapSize)
						{
							col.gameObject.GetComponent<Enemy>().spellSelect = false;
	//						col.gameObject.renderer.material.color = col.gameObject.GetComponent<Enemy>().startColor;
						}
					}
				}
			
			}

				yield return null;
			}
		
//		
	}

	IEnumerator AbilityCast(Ability ability)
	{
	
		while(!pressed)
		{	
			if(ability == Ability.Stun || ability == Ability.Suck || ability == Ability.Decoy || ability == Ability.Push)
			{
				overlapSphere = Physics.OverlapSphere(newPos,overlapSize);
				foreach(Collider col in overlapSphere)
				{

					if(col.gameObject.tag == "enemy")
					{

						if(col.gameObject.GetComponent<Enemy>().spellSelect && !col.gameObject.GetComponent<Enemy>().abducted)
						{
							StartCoroutine(AbilityUse(ability, col.gameObject));

							col.gameObject.GetComponent<Enemy>().spellSelect = false;
//							
						}
					}
				}

			
			}

//			if(ability == Ability.Decoy)
//			{
//				StartCoroutine(AbilityUse(ability, decoy));	
//			}
			yield return null;

		}
	}

	IEnumerator AbilityUse(Ability ability, GameObject objEnm) 
	{				


//		Debug.Log ("tesssst");
		if(ability == Ability.Stun)
		{
			objEnm.gameObject.GetComponent<Enemy>().stunned = true;
			yield return new WaitForSeconds(spellDuration);
			if(objEnm)
			{
				Debug.Log ("unstunned!");
				objEnm.gameObject.GetComponent<Enemy>().stunned = false;
			}
		}
		if(ability == Ability.Decoy)
		{
			GameObject decoyObj = Instantiate(decoy, newPos,Quaternion.identity) as GameObject;
			objEnm.gameObject.GetComponent<Enemy>().decoyed = true;
			objEnm.gameObject.GetComponent<Enemy>().target = decoyObj;

			yield return new WaitForSeconds(spellDuration);
			objEnm.gameObject.GetComponent<Enemy>().decoyed = false;
//			objEnm.gameObject.GetComponent<Enemy>().RecalculateNearest();
//			overlapSphere = Physics.OverlapSphere(newPos,overlapSize);

//			foreach(Collider col in overlapSphere)
//			{
//				
//				if(col.gameObject.tag == "enemy")
//				{
////					GameObject prevT = col.gameObject.GetComponent<Enemy>().agent.de
//					col.gameObject.GetComponent<Enemy>().agent.SetDestination(decoyObj.transform.position);
//					col.gameObject.GetComponent<Enemy>().decoyed = true;
//					yield return new WaitForSeconds(spellDuration);
//					col.gameObject.GetComponent<Enemy>().decoyed = false;
////					col.gameObject.GetComponent<Enemy>().RecalculateNearest();
////					yield break;
//				}
//			}

		}
		if(ability == Ability.Push)
		{
			if(!objEnm.GetComponent<Enemy>().abducted)
			{
				StartCoroutine("Push", objEnm);
			}
			//if enemy.hit == false
//			objEnm.rigidbody.isKinematic = false;
//			objEnm.rigidbody.AddForce((objEnm.transform.position - transform.position).normalized * 50, ForceMode.Impulse);
			yield return new WaitForSeconds(spellDuration);

//			objEnm.rigidbody.isKinematic = true;


		}
		if(ability == Ability.Suck)
		{
//			Debug.Log (objEnm.name);
			if(!objEnm.gameObject.GetComponent<Enemy>().abducted)
			{
			objEnm.gameObject.GetComponent<Enemy>().stunned = true;

//			objEnm.gameObject.renderer.enabled = false;
			StartCoroutine("Abduct",objEnm);
			}
//			yield return new WaitForSeconds(spellDuration);
//			objEnm.gameObject.GetComponent<Enemy>().stunned = false;
//			objEnm.gameObject.renderer.enabled = true;
			//gem vector for enemy
			//stun enemy
			//disable agent?
			//flyv op
			//yield
			//fald ned
		}
//		while(true)
//		{
//			objEnm.rigidbody.isKinematic = false;
//			objEnm.rigidbody.AddForce((objEnm.transform.position - transform.position).normalized * backlashForce, ForceMode.Impulse);
//		}
	}

	void OnDrawGizmos()	
	{
//		if(visible)
//		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(newPos, overlapSize);
//		}
	}

	IEnumerator Push(GameObject objEnm)
	{
//		bool onOff = true;
//		while(onOff)
//		{
			objEnm.rigidbody.isKinematic = false;
			objEnm.collider.enabled = false;
			objEnm.rigidbody.AddForce((objEnm.transform.position - transform.position).normalized * pushForce, ForceMode.Impulse);
			yield return new WaitForSeconds(spellDuration);
			
			objEnm.rigidbody.isKinematic = true;
			objEnm.collider.enabled = true;
//			onOff = false;
			yield return null;
//		}

	}

	IEnumerator Abduct(GameObject enmObj)
	{
		enmObj.gameObject.GetComponent<Enemy>().suckedUp = true;
		float mTime = 0f;
		Vector3 thisPos = enmObj.transform.position;
		bool onOff = true;
		Vector3 dirUp = enmObj.transform.up.normalized;
		enmObj.collider.enabled = false;
		GameObject beam = Instantiate(abductBeam, thisPos, Quaternion.identity) as GameObject;
		while(onOff)
		{
//			Debug.Log ("ONONON");
			if(mTime < spellDuration)
			if(mTime < spellDuration/2)
			{
				enmObj.GetComponent<Enemy>().agent.enabled = false;
				mTime += Time.deltaTime * 1;

				enmObj.collider.enabled = false;
				//				enmObj.transform.position = Vector3.Lerp(thisPos,new Vector3(thisPos.x,thisPos.y + 10,thisPos.z),mTime);
				enmObj.rigidbody.MovePosition(enmObj.transform.position + dirUp/4);
			}
			else
			{
				StartCoroutine(DeDuct (enmObj, thisPos, beam));
				onOff = false;
//				enmObj.transform.position = new Vector3(thisPos.x,thisPos.y,thisPos.z);
//				enmObj.gameObject.GetComponent<Enemy>().stunned = false;
			}
			yield return null;
		}

	}

	IEnumerator DeDuct(GameObject enmObj, Vector3 thisPos, GameObject beam)
	{

		float mTime = 1f; 
		Vector3 thisPos2 = enmObj.transform.position;
		enmObj.GetComponent<Enemy>().abducted = true;
		Vector3 dirDown = -enmObj.transform.up.normalized;
		foreach(Transform t in enmObj.gameObject.GetComponent<Enemy>().nudeObj)
		{
			t.gameObject.renderer.material.SetFloat("_SliceAmount", 0);
		}
		yield return new WaitForSeconds(2);

		enmObj.transform.position = thisPos;
		//				beam.SetActive(false);
		yield return new WaitForSeconds(0.2f);
		enmObj.gameObject.GetComponent<Enemy>().suckedUp = false;
		enmObj.collider.enabled = true;
		beam.gameObject.SetActive(false);
		enmObj.GetComponent<Enemy>().agent.enabled = true;
		enmObj.collider.enabled = true;
		enmObj.gameObject.GetComponent<Enemy>().stunned = false;

//		while(true)
//		{
//
//			if(mTime > 0)
//			{
//				mTime -= Time.deltaTime / 3;
//				foreach(Transform t in enmObj.gameObject.GetComponent<Enemy>().nudeObj)
//				{
//					t.gameObject.renderer.material.SetFloat("_SliceAmount", Mathf.Lerp(0,1,mTime));
//				}
////				//enmObj.GetComponent<Enemy>().agent.enabled = false;
////				//				enmObj.collider.enabled = false;
////				Debug.Log (mTime);
////				mTime += Time.deltaTime * 1;
////
////				enmObj.rigidbody.MovePosition(enmObj.transform.position + dirDown/2);
////				//	enmObj.transform.position = Vector3.Lerp(new Vector3(thisPos2.x,thisPos2.y,thisPos2.z),new Vector3(thisPos2.x,thisPos2.y-8,thisPos2.z),mTime);
////					
//			}
//			else
//			{
//
////				
//			}
//			yield return null;
//		}
//		yield return new WaitForEndOfFrame();
//		enmObj.GetComponent<Enemy>().agent.enabled = true; 
//		enmObj.collider.enabled = true;
//		enmObj.gameObject.GetComponent<Enemy>().stunned = false;
	}

	Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle) 
	{
		return angle * ( point - pivot) + pivot;
	}


}

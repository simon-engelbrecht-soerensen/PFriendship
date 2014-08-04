using UnityEngine;
using System.Collections;
using GamepadInput;

public class MeleeAttack : MonoBehaviour {
	public bool swinging;
	public float swingTime = 0.3f;
	public float swingSpeed = 10f;
	public GameObject swordAnchor;
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
	bool hitEnemy;
	void Start () {
	minAngle = this.transform.eulerAngles.z;
	}
	
	void Update () {

		switch(controls.playerNumber)
		{
		case 1:
			if(GamePad.GetButtonDown (GamePad.Button.A, GamePad.Index.One) && !swinging)
			{
			StartCoroutine("Swing");
			
			}
			break;
			
		case 2:
			if(GamePad.GetButtonDown (GamePad.Button.A, GamePad.Index.Two) && !swinging)
			{
			StartCoroutine("Swing");
			
			}
			break;
			
		case 3:
			if(GamePad.GetButtonDown (GamePad.Button.A, GamePad.Index.Three) && !swinging)
			{
			StartCoroutine("Swing");
			
			}
			break;
			
		case 4:
			if(GamePad.GetButtonDown (GamePad.Button.A, GamePad.Index.Four) && !swinging)
			{
			StartCoroutine("Swing");
			
			}
			break;
			
		default:
			break;
			
		}
		
		
	}
	
	IEnumerator OnTriggerStay(Collider col)
	{		
		if(col.gameObject.tag == "enemy" && swinging && !col.gameObject.GetComponent<Enemy>().hit)
		{
			for (int i = 0;i < amountOfSplatter; i++) 
			{
				GameObject splat = Instantiate(splatter,col.gameObject.transform.position, Quaternion.identity) as GameObject;
				splat.rigidbody.AddExplosionForce(100f,col.gameObject.transform.position * 2,100f);
			}

//			if(col.GetComponent<Enemy>().health <= 0)
//			{
			col.rigidbody.isKinematic = false;
			col.rigidbody.AddForce((col.transform.position - transform.position).normalized * backlashForce, ForceMode.Impulse);
//			col.gameObject.GetComponent<Enemy>().dead = true;
			col.gameObject.GetComponent<Enemy>().health -= 1;
			col.gameObject.GetComponent<Enemy>().hit = true;
			yield return new WaitForSeconds(backlashDuration);
			if(col)
			{
				if(col.gameObject.GetComponent<Enemy>().health <= 0)
				{
					Destroy (col.gameObject);
				}
				col.rigidbody.isKinematic = true;
			}
				//				StartCoroutine("KillObj",col.gameObject);
				
//			}
//			else 
//			{	col.rigidbody.isKinematic = false;
//				col.rigidbody.AddForce((col.transform.position - transform.position).normalized * 3, ForceMode.Impulse);
//				yield return new WaitForSeconds(0.2f);
//				if(col)
//				{
//					col.rigidbody.isKinematic = true;
//				}
//			}
			yield return null;

		}
	}
		
	IEnumerator Swing()
	{
		swinging = true;
		movement.speed = movement.speedWhenAtttacking;
		yield return StartCoroutine(pTween.To(swingTime, (float t) => {
			angle = Mathf.Lerp(minAngle, maxAngle, t);
			swordAnchor.transform.localEulerAngles = new Vector3(0,- angle, 0);
		}));
		movement.speed = movement.startSpeed;
		swinging = false;
		hitEnemy = false;
	}
	
//	IEnumerator KillObj(GameObject toKill)
//	{
////		Debug.Log("DIE");
//		yield return new WaitForSeconds(killTime);
//		
//		Destroy (toKill);
//	}

}

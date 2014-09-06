using UnityEngine;
using System.Collections;
//using GamepadInput;
using System.Collections.Generic;
//using UnityEditor;
using XInputDotNetPure;
public class Movement : MonoBehaviour {
	public Vector3 inputDir;
	public Vector3 inputDir2;


	public float movementX;
	public float movementY;
	public float movementMax = 5f;
	public float drag = 5f;
	private float dragBase;
	public float accel = 3f;
	public float reverseAccel = 1f;
	public float deAccel = 2f;
	public float bounceDamp = 2f;
	public float minBounceThreshold = 0.1f;
	public float angle = 0f;
	private float angularVelocity = 0f;
	private float targetAngle = 0;
	public float angleSmoothDuration = 0.05f;
	private float angleMaxSpeed = 5800f;
	public float speed = 0;
	public float dashSpeed = 20f;
	public bool dashing;
	public float dashDuration = 1.5f;
	public float dashCooldown = 2f;
	private bool dashable;
	public float speedWhenAtttacking = 1f;
	public float startSpeed;
	public float movementOverall;
	public Controls controls;
	Vector2 leftStick1;
	Vector2 leftStick2;
	Vector2 leftStick3;
	Vector2 leftStick4;
	bool dashButton1;
	bool dashButton2;
	bool dashButton3;
	bool dashButton4;
	bool dashButton;
	float standStillButton1;
	float standStillButton2;
	float standStillButton3;
	float standStillButton4;
	float standStillButton;
	public bool grounded;
	float gravity = -10;
	float distToGround;
	public float raycastRange = 0.3f;
	private bool moving;
	private bool standStill;
	public Animator animator;
	public ParticleSystem footDust;
	public float feet;
	public bool feetToggle;
	public SkinnedMeshRenderer[] childrenWithSMR;
	private Renderer[] childrenWithR;
	private bool startFreeze;
	public GameObject aura;
	public bool spawnEffect;
	private float mMaxStart;
	private TrailRenderer thisTrail;
	private Transform teleportEffect;
	public bool exited;
//	Color color;
//	GamePadState Player1State;	
//	GamePadState Player2State;	
//	
//	PlayerIndex Player1Index = (PlayerIndex)0;	
//	PlayerIndex Player2Index = (PlayerIndex)1;
	PlayerIndex player1 = PlayerIndex.One;
	PlayerIndex player2 = PlayerIndex.Two;
	PlayerIndex player3 = PlayerIndex.Three;
	PlayerIndex player4 = PlayerIndex.Four;

	void Start () 
	{		
		teleportEffect = transform.Find("Hit02Over");
		thisTrail = GetComponent<TrailRenderer>();
		thisTrail.enabled = false;
		mMaxStart = movementMax;
		dashable = true;
		startSpeed = speed;
		dragBase = drag;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		distToGround = collider.bounds.extents.y;
		animator = GetComponentInChildren<Animator>();
		childrenWithSMR = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		childrenWithR = gameObject.GetComponentsInChildren<Renderer>();
		if(spawnEffect)
		{
			startFreeze = true;
			standStill = true;
			StartCoroutine("EnterWorld");

			foreach(SkinnedMeshRenderer smr in childrenWithSMR)
			{
				Color color = smr.material.color;
				color.a = 0;
				smr.material.color = color;
			}
			foreach(Renderer r in childrenWithR)
			{
				if(r.material.HasProperty("_Color"))
				{
					if(r.tag != "aimMark")
					{
						Color color = r.material.color;
						color.a = 0;
						r.material.color = color;
					}
				}
			}
		}

		
	}
	
	void Update()
	{
		GamePadState p1 = GamePad.GetState(player1);
		GamePadState p2 = GamePad.GetState(player2);
		GamePadState p3 = GamePad.GetState(player3);
		GamePadState p4 = GamePad.GetState(player4);


		if(animator)
		{
			feet = animator.GetFloat("feetCurve");
			animator.SetFloat("angularVel",angularVelocity);
			animator.SetBool("moving", moving);
		}
		if(footDust && speed > 0.2f && !dashing)
		{
			if(feet > 1 && !feetToggle)
			{
				feetToggle = true;
				footDust.Emit(1);
			}
			if(feet < 0.1 && feetToggle)
			{
				feetToggle = false;
				footDust.Emit(1);
			}
		}

		standStillButton1 = p1.Triggers.Left;
		standStillButton2 = p2.Triggers.Left;
		standStillButton3 = p3.Triggers.Left;
		standStillButton4 = p4.Triggers.Left;

		if(p1.Buttons.X == ButtonState.Pressed) dashButton1 = true;
		else dashButton1 = false;
		if(p2.Buttons.X == ButtonState.Pressed) dashButton2 = true;
		else dashButton2 = false;
		if(p3.Buttons.X == ButtonState.Pressed) dashButton3 = true;
		else dashButton3 = false;
		if(p4.Buttons.X == ButtonState.Pressed) dashButton4 = true;
		else dashButton4 = false;

//		standStillButton2 = GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.Two);
//		standStillButton3 = GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.Three);
//		standStillButton4 = GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.Four);
//

//		dashButton1 = p1.Buttons.X
//		dashButton1 = GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.One);
//		dashButton2 = GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.Two);
//		dashButton3 = GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.Three);
//		dashButton4 = GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.Four);
//		//		Debug.Log (IsGrounded());
//		leftStick1 = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
//		leftStick2 = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two);
//		leftStick3 = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three);
//		leftStick4 = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four);


		switch(controls.playerNumber)
		{
		case 1:
			standStillButton = standStillButton1;
			dashButton = dashButton1;
			inputDir = new Vector3(p1.ThumbSticks.Left.X, p1.ThumbSticks.Left.Y,  0);
			inputDir2 = new Vector3(p1.ThumbSticks.Right.X, p1.ThumbSticks.Right.Y,  0);

//			rumbleButton
			break;

		case 2:
			standStillButton = standStillButton2;
			dashButton = dashButton2;
			inputDir = new Vector3(p2.ThumbSticks.Left.X, p2.ThumbSticks.Left.Y, 0);
			inputDir2 = new Vector3(p2.ThumbSticks.Right.X, p2.ThumbSticks.Right.Y,  0);

			break;
			
		case 3:
			standStillButton = standStillButton3;
			dashButton = dashButton3;
			inputDir = new Vector3(p3.ThumbSticks.Left.X,p3.ThumbSticks.Left.Y,0);
			inputDir2 = new Vector3(p3.ThumbSticks.Right.X, p3.ThumbSticks.Right.Y,  0);

			break;
			
		case 4:
			standStillButton = standStillButton4;
			dashButton = dashButton4;
			inputDir = new Vector3(p4.ThumbSticks.Left.X,p4.ThumbSticks.Left.Y,0);
			inputDir2 = new Vector3(p4.ThumbSticks.Right.X, p4.ThumbSticks.Right.Y,  0);

			break;
			
		default:
			break;
		}

		if(inputDir.magnitude > 1f)
			inputDir.Normalize();

//		Debug.DrawLine(Vector3.zero, new Vector3(inputDir.x, 0f, inputDir.y) * 10f, Color.red);

		
		if(standStillButton >= 0.8f)
		{
			standStill = true;
			rigidbody.velocity = new Vector3(0,0,0);
			angleMaxSpeed = 300;
		}
		else
		{
			if(!startFreeze)
			{
				angleMaxSpeed = 5800;
				standStill = false; 
			}
		}

		if(dashButton)
		{
			StartCoroutine("Dash");
		}
		if(dashing)
		{
			speed = dashSpeed;
			drag = 1;
//			accel = 100000;
			movementMax = 10000000;


		}
		inputDir = Quaternion.Euler(35, 0, 45) * inputDir;
		inputDir2 = Quaternion.Euler(35, 0, 45) * inputDir2;

	}
	void FixedUpdate()
	{



		MovementF ();

	}

	IEnumerator Dash()
	{
		if(dashButton && !dashing && dashable)
		{
//			float tempAccel = accel;
			float tempSpeed = speed;
			dashable = false;
			dashing = true;

//			Debug.Log(tempSpeed);
			foreach(SkinnedMeshRenderer smr in childrenWithSMR)
			{
				smr.enabled = false;
			}
			foreach(Renderer r in childrenWithR)
			{
				if(r != this.renderer && r.gameObject.tag != "spellMarker")
				{
					r.enabled = false;
				}
			}
			thisTrail.enabled = true;
			teleportEffect.gameObject.SetActive(true);
			teleportEffect.gameObject.transform.rotation = this.transform.rotation;
			teleportEffect.gameObject.transform.position = this.gameObject.transform.position;
			teleportEffect.gameObject.transform.parent = null;
			yield return new WaitForSeconds(dashDuration);
//			teleportEffect.gameObject.transform.parent = this.transform;



//			teleportEffect.gameObject.transform.position = this.gameObject.transform.position;


			speed = tempSpeed;
			drag = dragBase;
			movementMax = mMaxStart;
			foreach(SkinnedMeshRenderer smr in childrenWithSMR)
			{
				smr.enabled = true;
			}
			foreach(Renderer r in childrenWithR)
			{
				if(r != this.renderer && r.gameObject.tag != "spellMarker")
				{
					r.enabled = true;
				}
			}
			dashing = false;
			thisTrail.enabled = false;

			yield return new WaitForSeconds(dashCooldown);
			teleportEffect.gameObject.SetActive(false);
			teleportEffect.gameObject.transform.parent = this.transform;
			dashable = true;
		}

	}
	
	void MovementF()
	{

//		Debug.Log (inputDir.magnitude);
//		Debug.DrawRay(transform.position, -Vector3.up, Color.red);
		if(inputDir.magnitude > 0.1f)
		{
			moving = true;
		}
		else moving = false;

		if(drag < dragBase)
		{
			drag += Time.deltaTime * 5;
		}
		if(!IsGrounded())
		{
			gravity = -5;
		}
		else
		{
			gravity = transform.forward.y * speed;
		}
//		if(inputDir.magnitude > 0f)
//		{
		speed = speed + accel * inputDir.magnitude * Time.deltaTime;
		speed = Mathf.Clamp(speed, 0f, movementMax);
		speed = speed - speed * Mathf.Clamp01(drag * Time.deltaTime);
//		}
//		else
//		{
//			speed = 0f;	
//		}
//		transform.position = transform.position + (transform.forward * speed) * Time.deltaTime;		
//		rigidbody.AddForce(transform.forward * speed);	
		if(!standStill)
		{
			rigidbody.velocity = new Vector3(inputDir.x * speed, gravity, inputDir.z * speed);
		}
		
		if(inputDir2.magnitude > 0f)
		{
			float newTargetAngle = Mathf.Atan2(inputDir2.x, inputDir2.y) * Mathf.Rad2Deg;		
			targetAngle = newTargetAngle;
		}					
		
		angle = Mathf.SmoothDampAngle(angle, targetAngle, ref angularVelocity, angleSmoothDuration, angleMaxSpeed);
//		angle = targetAngle;
//	
		transform.localRotation =  Quaternion.Euler(0 , angle, 0);

	}

	bool IsGrounded()
	{
		return  Physics.Raycast(transform.position, -Vector3.up,raycastRange);
	}

	IEnumerator EnterWorld()
	{
//		yield return new WaitForSeconds(0.5f);
		float mTime = 0;
		float mTime2 = 1;
		bool onOff = true;
		GameObject auraInst = Instantiate(aura,transform.position,Quaternion.identity) as GameObject;
		Renderer[] beamChildren = auraInst.GetComponentsInChildren<Renderer>();

		foreach(Renderer ch in beamChildren)
		{
			Color beamC = ch.renderer.material.color;
			beamC.a = 0; 
			ch.renderer.material.color = beamC;

		}


//		while(mTime < 0.8)
//		{
//			mTime+= Time.deltaTime * 0.5f;
//			foreach(Renderer ch in beamChildren)
//			{
//				Color beamC = ch.renderer.material.color;
//				beamC.a = mTime; 
//				ch.renderer.material.color = beamC;
//			}
//
//			yield return null;
//		}
		while(onOff)
		{
			if(mTime < 1)
			{
//				Debug.Log (mTime);
				mTime += Time.deltaTime * 0.5f;
				foreach(Renderer ch in beamChildren)
				{

					Color beamC = ch.renderer.material.color;
					beamC.a = mTime; 
					ch.renderer.material.color = beamC;

				}
				foreach(SkinnedMeshRenderer smr in childrenWithSMR)
				{
					Color color = smr.material.color;
					color.a = mTime;
					smr.material.color = color;
					//			smr.enabled = false;
				}
				foreach(Renderer r in childrenWithR)
				{
					if(r.material.HasProperty("_Color"))
					{
						if(r.tag != "aimMark")
						{
							Color color = r.material.color;
							color.a = mTime;
							r.material.color = color;
						}
					}
					//			r.enabled = false;
				}
			}
			else
			{
				mTime2 -= Time.deltaTime * 0.5f;
				foreach(Renderer ch in beamChildren)
				{
					Color beamC = ch.renderer.material.color;
					beamC.a = mTime2; 
					ch.renderer.material.color = beamC;
				}
				if(mTime2 < 0)
				{
					onOff = false;
				}
			}
			yield return null;
		}
//		
		standStill = false;
		startFreeze = false;
		yield return new WaitForSeconds(2);

		auraInst.SetActive(false);

	}

	IEnumerator ExitWorld()
	{
		float mTime = 1;
		GameObject auraInst = Instantiate(aura,transform.position,Quaternion.identity) as GameObject;
		standStill = true;
		startFreeze = true;
		rigidbody.velocity = new Vector3(0,0,0);
		exited = true;
		while(mTime > 0)
		{
			Debug.Log (mTime);
			mTime -= Time.deltaTime * 0.5f;
			foreach(SkinnedMeshRenderer smr in childrenWithSMR)
			{
				smr.castShadows = false;
				smr.receiveShadows = false;
				Color color = smr.material.color;
				color.a = mTime;
				smr.material.color = color;
				//			smr.enabled = false;
			}
			foreach(Renderer r in childrenWithR)
			{
				r.castShadows = false;
				r.receiveShadows = false;
				Color color = r.material.color;
				color.a = mTime;
				r.material.color = color;
				//			r.enabled = false;
			}
			yield return null;
		}
		//		
		yield return new WaitForSeconds(2);
		auraInst.SetActive(false);

	}


}

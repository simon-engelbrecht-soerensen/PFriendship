using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MidObject : MonoBehaviour {
	public float distanceToKeep = 1.0f;
	public NavMeshAgent agent;
	public GameObject target;
	public GameObject prevTarget;
	public float floatHeight;
	public float suckRadius;
	
	public Collider[] overlapSphere;
	private Collider[] overlapSphere2;
	
	//	private  Vector3 curVel = Vector3.zero;
	public GameObject attatchedTo;
	
	public bool inChargeRange;
	public Color startColor;
	public GameObject chargeObj;
	public int chargeNumber;
	public bool charging;
	public float suckTime = 0.5f;
	public float goldCount;
	public List<Transform> children;
	public bool playingAnim;
	private LineRenderer lineR;
	public UILabel goldLabelUI;
	public AudioClip coinSound;
	private AudioSource aSource;

	void Start () {
		aSource = this.GetComponent<AudioSource>();
		aSource.clip = coinSound;
		lineR = this.gameObject.GetComponent<LineRenderer>();
//		lineR.SetVertexCount(20);
		agent.stoppingDistance = distanceToKeep;
		agent.height = floatHeight;
		agent.baseOffset = floatHeight;
		startColor = this.renderer.material.color;
		foreach(Transform child in transform)
		{
			children.Add(child);
		}
	}
	
	void Update () {
		goldLabelUI.text = goldCount.ToString();
		if(target)
		{
			lineR.enabled = true;
			lineR.SetPosition(0,this.transform.position);
			lineR.SetPosition(1,target.transform.position);
		}
		else lineR.enabled = false;
//		ch.gameObject.renderer.material.SetFloat("_SliceAmount", Mathf.PingPong(Time.time/5, 0.2f)+0.3f);

		foreach(Transform ch in children)
		{
			ch.gameObject.transform.Rotate(Vector3.right * Time.deltaTime * 20);
			ch.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * 15);
			if(ch.gameObject.name == "Black_outer")
			{
//				ch.gameObject.transform.Rotate(Vector3.right * Time.deltaTime * 20);
				ch.gameObject.renderer.material.SetFloat("_SliceAmount", Mathf.PingPong(Time.time/5, 0.2f)+0.3f);
			}
		}
		if(!charging)
		{
			if(target != null)
			{
				agent.SetDestination(target.transform.position);
			}
			if(target == null && !inChargeRange)
			{
				agent.SetDestination(this.transform.position);
			}
		}
		overlapSphere = Physics.OverlapSphere(this.transform.position, suckRadius);
		overlapSphere2 = Physics.OverlapSphere(this.transform.position, suckRadius + 0.2f);
		
		SphereDetect();
		
		if(inChargeRange)
		{
			//fade farve
			renderer.material.color = Color.yellow;
			if(target == null)
			{
				if(!chargeObj.GetComponent<ChargePoint>().charged)
				{
					charging = true;
					agent.SetDestination(chargeObj.transform.position);
					agent.stoppingDistance = 0f;
					chargeObj.GetComponent<ChargePoint>().started = true;
					chargeObj.GetComponent<ChargePoint>().midObj = this.GetComponent<MidObject>();
					if(!playingAnim)
					{
						playingAnim = true;
						chargeObj.transform.parent.gameObject.GetComponent<Animation>().Play ();
//						chargeObj.transform.parent.gameObject.GetComponent<Animation>().wrapMode = WrapMode.Once;
						chargeObj.transform.parent.FindChild("GoldSuckEffect").gameObject.SetActive(true);
					}

					agent.baseOffset = 10;
				}
				//				else
				//				{
				////					agent.SetDestination(prevTarget.transform.position);
				//					agent.stoppingDistance = distanceToKeep;
				//				}
				
			}
		}
		else
		{
			renderer.material.color = startColor;
		}
		
		if(charging)
		{
			
		}
		else
		{
			agent.stoppingDistance = distanceToKeep;

			agent.baseOffset = Mathf.SmoothStep(3,6,Mathf.PingPong(Time.time,1));
			playingAnim = false;
			if(chargeObj)
			{
				chargeObj.transform.parent.FindChild("GoldSuckEffect").gameObject.SetActive(false);
			}
		}
	}
	
	void SphereDetect()
	{
		
		for(int i = 0; i <= overlapSphere.Length -1; i++)
		{
			if(overlapSphere[i].gameObject.tag == "gold")
			{
				Vector3 curVel = Vector3.zero;
				overlapSphere[i].GetComponent<Rigidbody>().useGravity = false;
				//				overlapSphere[i].transform.position = new Vector3(Mathf.SmoothDamp(overlapSphere[i].transform.position.x,this.transform.position.x, ref curVel, suckTime),Mathf.SmoothDamp(overlapSphere[i].transform.position.y,this.transform.position.y, ref curVel, suckTime),Mathf.SmoothDamp(overlapSphere[i].transform.position.z,this.transform.position.z, ref curVel, suckTime));
//				overlapSphere[i].transform.position = Vector3.SmoothDamp(overlapSphere[i].transform.position, this.transform.position, ref curVel, suckTime);
				float step = 10 * Time.deltaTime;
				
				// Move our position a step closer to the target.
				overlapSphere[i].transform.position = Vector3.MoveTowards(overlapSphere[i].transform.position, this.transform.position, step);
				//				overlapSphere[i].rigidbody.MovePosition(
				
				//				overlapSphere[i].transform.position = Vector3.Slerp(
			}
			
			if(overlapSphere[i].gameObject.name == "ChargePoint")
			{
				if(!overlapSphere[i].GetComponent<ChargePoint>().charged)
				{
					chargeNumber = i;
					inChargeRange = true;
					chargeObj = overlapSphere[i].gameObject;
				}
			}
			
			//			if(overlapSphere[chargeNumber].gameObject.name != "ChargePoint")
			//			{
			//				inChargeRange = false;
			//			}
			
		}
		foreach(Collider col in overlapSphere2)
		{
			if(col.gameObject.name == "ChargePoint" && Vector3.Distance(this.transform.position, col.transform.position) > suckRadius)
			{
				inChargeRange = false;
				//							col.gameObject.renderer.material.color = col.gameObject.GetComponent<Enemy>().startColor;
			}
		}
		//		foreach(Collider col in overlapSphere) 
		//		{	
		//			if(col.gameObject.tag == "gold")
		//			{
		//
		//				col.gameObject.GetComponent<Rigidbody>().useGravity = false;
		//
		//				col.transform.position = new Vector3(Mathf.SmoothDamp(col.transform.position.x,this.transform.position.x, ref curVel, 0.05f),Mathf.SmoothDamp(col.transform.position.y,this.transform.position.y, ref curVel, 0.05f),Mathf.SmoothDamp(col.transform.position.z,this.transform.position.z, ref curVel, 0.05f));
		//			}
		//			if(col.gameObject.name == "ChargePoint")
		//			{
		//				chargeObj = col.gameObject;
		//				if(!chargeObj.GetComponent<ChargePoint>().charged)
		//				{
		//					inChargeRange = true;
		//				}
		////				ChargePoint cp = col.gameObject.GetComponent<ChargePoint>();
		////				if(target == null && cp.charged == false)
		////				{
		////					cp.midObj = GetComponent<MidObject>();
		////					target = col.gameObject;
		////					agent.stoppingDistance = 0;
		////					cp.started = true;
		////				}
		//
		//			}
		////			else
		////			{
		//////				inChargeRange = false;
		////			}
		//		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "gold")
		{
			goldCount += Random.Range(10,20);
			Destroy(col.gameObject);
			aSource.pitch = Random.Range(0.8f, 1f);
			aSource.Play ();
		}
	}
	
//	void OnGUI()
//	{
//		GUI.color = Color.yellow;
//		GUI.Label(new Rect(Screen.width/2 - 8,Screen.height-20,200,200),"Gold:" + goldCount);
//	}
}

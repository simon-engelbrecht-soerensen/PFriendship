using UnityEngine;
using System.Collections;

public class DestroyItem : MonoBehaviour {

//	[HideInInspector]
	public bool destroy;
//	[HideInInspector]
	public Transform player;

	public float hitForce = 3f;
	private bool animatable;
	private Animator animator;
	public int health = 3;
	private bool animating;
	public GameObject doorLeft;
	public GameObject doorRight;
	void Start () 
	{
		if(GetComponent<Animator>())
		{
			animator = GetComponent<Animator>();
			animatable = true;
		}
	}

	void Update () 
	{
		if(destroy && animatable)
		{
			if(health > 1 && !animating)
			{
				animating = true;
				StartCoroutine("Animate");
			}
			if(health == 1)
			{
				animatable = false;
			}
		}
		if(destroy && !animatable)
		{
			this.gameObject.collider.enabled = false;
			if(this.renderer)
			{
				this.renderer.enabled = false;
			}
			if(this.GetComponent<SkinnedMeshRenderer>())
			{
				this.GetComponent<SkinnedMeshRenderer>().enabled = false;
			}
			Transform [] t = GetComponentsInChildren<Transform>();
//			rigidbody.AddForce(player.transform.forward * hitForce, ForceMode.Impulse); 
			//rigidbody.AddForce(player.transform.position + this.transform.position * 10,ForceMode.Impulse);
//			Transform theParent = t[0];
//			Transform firstChild = t[1];
			for(int i = 2; i <= t.Length -1; i++)
			{
				if(t[i].gameObject.tag != "destruct")
				{
					if(t[i].gameObject.rigidbody)
					{
						t[i].gameObject.renderer.enabled = true;
						t[i].gameObject.collider.enabled = true;
						t[i].gameObject.rigidbody.isKinematic = false;
		//				t[i].rigidbody.AddForce(player.transform.forward * hitForce, ForceMode.Impulse);
		//				t[i].rigidbody.AddTorque (new Vector3(10,10,10)); 
		//				this.transform.GetChild(i).gameObject.SetActive(true);
						t[i].rigidbody.AddForce(player.transform.forward * hitForce, ForceMode.Impulse);
						t[i].rigidbody.AddTorque (new Vector3(10,10,10)); 
					}
				}
				else if(t[i].gameObject.tag == "destruct") t[i].gameObject.SetActive(false);
			}
			if(doorLeft && doorRight)
			{
				doorLeft.SetActive(true);
				doorRight.SetActive(true);
			}
//			for (int i = 0; i < this.transform.GetChildCount(); ++i)
//			{
//				this.transform.GetChild(i).gameObject.SetActive(true);
//
//			}
		
			transform.DetachChildren();
			Destroy(this.gameObject);
		}
	}

	IEnumerator Animate()
	{

		animator.SetBool("hit", true);
		destroy = false;
		health -= 1;
		yield return new WaitForSeconds(1f);
		animator.SetBool("hit", false);
		animating = false;


	}
}

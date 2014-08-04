using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
	public float health = 3;
	[HideInInspector]
	public float startHealth;
	public bool hit;
	public float invulTime = 0.2f;
	public bool invul;

	public float healthRegenCD = 3f;
	private float healthRegenCounter;
	private bool regenHealth;

	private Color startColor;
	public float spawnTime = 3.0f;
	public PlayerDeath playerDeath;
	public bool visible;
	public GameObject healthSpriteOver;
	public GameObject healthSpriteUnder;
	[HideInInspector]
	public GameObject healthSpriteOverInst;
	[HideInInspector]
	public GameObject healthSpriteUnderInst;
	[HideInInspector]
	public UISprite uiSpriteOver;
	[HideInInspector]
	public UISprite uiSpriteUnder;

//	public GameObject testCube;
	public Vector3 screenPos;
	public float heightOffset;
	public float widthOffset;
	private float screenHeight = Screen.height;
	private float screenWidth = Screen.width;
	public float healthForUI;
	public Color uiColor;
	public GameObject uiRoot;


	void Start () 
	{

		healthRegenCounter = 3;
		uiColor.a = 1;
//		health = 3;
//		startColor = this.renderer.material.color;
		startHealth = health;
		healthSpriteOverInst = Instantiate(healthSpriteOver, Vector3.zero, Quaternion.identity) as GameObject;
		healthSpriteUnderInst = Instantiate(healthSpriteUnder, Vector3.zero, Quaternion.identity) as GameObject;

		uiSpriteOver = healthSpriteOverInst.GetComponent<UISprite>();
		uiSpriteUnder = healthSpriteUnderInst.GetComponent<UISprite>();
		uiSpriteOver.color = uiColor;
		uiSpriteUnder.color = uiColor;
		uiSpriteOver.enabled = false;
		uiSpriteUnder.enabled = false;
		playerDeath = GameObject.Find("RespawnPlayers").GetComponent<PlayerDeath>();
	}


	void Update () 
	{
		if(health != 0)
		{
			healthForUI = health / startHealth;
//			uiSpriteOver.enabled = true;
//			uiSpriteUnder.enabled = true;
		}
//		else if(health == 0)
//		{
			
//		}

		if(healthRegenCounter < healthRegenCD)
		{
			healthRegenCounter += Time.deltaTime;
			uiSpriteOver.enabled = true;
			uiSpriteUnder.enabled = true;
		}
	
		if((int)health == startHealth)
		{
			uiSpriteOver.enabled = false;
			uiSpriteUnder.enabled = false;
		}

		if(health < startHealth && healthRegenCounter >= healthRegenCD)
		{
			regenHealth = true;


		}

		else
		{
			regenHealth = false;
//			uiSpriteOver.enabled = false;
//			uiSpriteUnder.enabled = false;

		}
		if(regenHealth && health < startHealth) 
		{
			health += Time.deltaTime;
		}



		screenPos = Camera.main.WorldToScreenPoint(transform.position);	
		screenPos.x -= (screenWidth / 2.0f) - widthOffset;
		screenPos.y -= (screenHeight / 2.0f) - heightOffset;
		healthSpriteOverInst.transform.localPosition = screenPos;
		healthSpriteUnderInst.transform.localPosition = screenPos;
		uiSpriteOver.fillAmount = healthForUI;
		if(hit && !invul)
		{

			invul = true;
			health -= 1;
			hit = false;
			StartCoroutine("GotHit");

		}
		if(health <= 0)
		{
			uiSpriteOver.enabled = false;
			uiSpriteUnder.enabled = false;
			healthForUI = 0;
//			Debug.Log (healthForUI);
			playerDeath.StartCoroutine("KillPlayer",this.gameObject);
			hit = false;
			invul = false;
		}

	}
	public IEnumerator GotHit()
	{
		healthRegenCounter = 0;
//		testCube.renderer.material.color = Color.red;
		yield return new WaitForSeconds(invulTime);
		invul = false;
//		testCube.renderer.material.color = startColor;
	}

//	IEnumerator Die()
//	{
//		this.gameObject.SetActive(false);
//		yield return new WaitForSeconds(spawnTime);
////		this.gameObject.SetActive(true);
//	}
}

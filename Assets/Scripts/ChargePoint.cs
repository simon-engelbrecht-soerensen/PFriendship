using UnityEngine;
using System.Collections;

public class ChargePoint : MonoBehaviour {
	public float chargeDuration;
	public float timer;
	public float goldSpawnTimer;
	[HideInInspector]
	public bool started;

	public bool used;
	public MidObject midObj;
	public bool charged;

	public int chargeGold = 10;
	private int currentChargeGold = 0;

	public float secondsPerGold;

	private bool wasStarted = false;
	private float chargeStartTime;
	public GameObject shineChest;
	void Start () 
	{
		shineChest = transform.parent.Find("Shine_chest").gameObject;
		secondsPerGold = chargeDuration / chargeGold;
	}
	

	void Update () {
		if(started && !wasStarted)
		{
			chargeStartTime = Time.time;
			wasStarted = true;
		}


		if(started)
		{
//			Time.de
			timer += Time.deltaTime;
			float timeSinceChargeStart = Time.time - chargeStartTime;
			int currentChargeGoldTarget = Mathf.FloorToInt(timeSinceChargeStart/secondsPerGold);

			midObj.goldCount += currentChargeGoldTarget - currentChargeGold;
			currentChargeGold = currentChargeGoldTarget;

			if(currentChargeGold >= chargeGold) 
			{
				charged = true;
				started = false;
				midObj.target = null;
			}
		}
		if(charged)
		{
			shineChest.SetActive(false);
			midObj.charging = false;
		}
	}
}

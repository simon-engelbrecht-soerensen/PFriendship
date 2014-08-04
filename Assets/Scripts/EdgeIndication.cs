using UnityEngine;
using System.Collections;

public class EdgeIndication : MonoBehaviour {


//	public Camera mainCam;
//	public Camera nguiCam;
//	public Transform[] chargePoints;
	private float halfScreenWidth = Screen.width/2;
	private float halfScreenHeight = Screen.height/2;
	public GameObject uiIndicator2D;
	public GameObject uiIndicator2DInst;
	public GameObject playerCountdown;
	public GameObject playerCountdownInst;
	public Vector3 screenPos;
	public Vector3 screenPosViewport;
	public Vector3 screenPosViewportClamped;
	public	Vector3 v3;
	public Vector3 between;
	public float angle;
	public Vector3 midPoint;
	private float playerTimerFloat;
	public int playerTimer;
	public GameObject uiRoot; 
	private GameObject uiRootInst;
	private PlayerDeath playerDeath;
	void Start () {
		playerTimerFloat = 5;
		playerDeath = GameObject.Find("RespawnPlayers").GetComponent<PlayerDeath>();
//		uiRootInst = Instantiate(uiRoot,new Vector3(9999,9999,9999),Quaternion.identity) as GameObject;
		uiRoot = GameObject.Find("UIRoot");
		if(playerCountdown)
		{
			playerCountdownInst = Instantiate(playerCountdown,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			playerCountdownInst.transform.parent = uiRoot.transform;
			playerCountdownInst.GetComponent<UILabel>().enabled = false;
			playerCountdownInst.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
			playerCountdownInst.GetComponent<UILabel>().fontSize = 30;
		}
		if(uiIndicator2D)
		{
			uiIndicator2DInst = Instantiate(uiIndicator2D,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			uiIndicator2DInst.transform.parent = uiRoot.transform;
			uiIndicator2DInst.GetComponent<UITexture>().enabled = false;
			uiIndicator2DInst.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
		
		}
	}

	void Update () 
	{

		if(this.GetComponent<ChargePoint>())
		{
			if(this.GetComponent<ChargePoint>().charged)
			{
				uiIndicator2DInst.SetActive(false);
			}
		}
		screenPos = Camera.main.WorldToScreenPoint (transform.position);
//		screenPosNeg = Camera.main.WorldToScreenPoint (transform.position);
		if(screenPos.x > 0 &&  screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
		{
//			Debug.Log ("inside");
			if(uiIndicator2DInst)
			{
				uiIndicator2DInst.GetComponent<UITexture>().enabled = false;
			}
			screenPosViewport = Camera.main.WorldToViewportPoint (transform.position);
			playerTimer = 5;
			playerTimerFloat = 5;
			if(playerCountdownInst)
			{
				playerCountdownInst.GetComponent<UILabel>().enabled = false;
			}
		}
		else
		{

				screenPosViewport = Camera.main.WorldToViewportPoint (transform.position);
	//			screenPosViewport.x = Mathf.Clamp(screenPosViewport.x,-10f, 10f);
	//			screenPosViewport.y = Mathf.Clamp(screenPosViewport.y,-10f, 10f);
	//			screenPosViewport.z = Mathf.Clamp(screenPosViewport.z,5f, 10f);

			if(screenPosViewport.z > 0) //object is in front of camera position in world space
			{
				screenPosViewportClamped = new Vector3(Mathf.Clamp (screenPosViewport.x, 0.04f, 0.96f), Mathf.Clamp(screenPosViewport.y, 0.04f, 0.96f), 0);
				v3 = new Vector3(screenPosViewportClamped.x * Screen.width -halfScreenWidth, screenPosViewportClamped.y * Screen.height -halfScreenHeight, 0);

				if(uiIndicator2DInst)
				{
					uiIndicator2DInst.transform.localPosition = v3;
					uiIndicator2DInst.GetComponent<UITexture>().enabled = true;
				}
				if(playerCountdownInst)
				{
				playerTimerFloat -= Time.deltaTime;
				playerTimer = (int)playerTimerFloat;

					playerCountdownInst.GetComponent<UILabel>().text = playerTimer.ToString();
					playerCountdownInst.GetComponent<UILabel>().enabled = true;
					playerCountdownInst.transform.localPosition = v3;
				

				if(playerTimer < 1)
				{
					StartCoroutine(playerDeath.OutOfVisionSpawn(this.gameObject));
	//				respawn
				}
				}

				midPoint = new Vector3(Screen.width - halfScreenWidth, Screen.height - halfScreenHeight, 0);
				//		between = screenPosViewport - screenPosViewportClamped;
				between = screenPosViewport - Camera.main.ScreenToViewportPoint(midPoint);
				//		between = Camera.main.ViewportToScreenPoint(between);
				//		between = new Vector3(between.x * Screen.width - halfScreenWidth, between.y * Screen.height - halfScreenHeight, 0);
				angle = Mathf.Atan2(between.x, between.y) * Mathf.Rad2Deg;
				Vector3 angles = Vector3.zero;
				angles.z = -angle;
				if(uiIndicator2DInst)
				{
					uiIndicator2DInst.transform.localEulerAngles = angles;
				}
			}
			else 
			{
				if(uiIndicator2DInst)
				{
					screenPosViewportClamped = new Vector3(Mathf.Clamp (screenPosViewport.x, 0.04f, 0.96f), Mathf.Clamp(screenPosViewport.y, 0.04f, 0.96f), 0);
					v3 = new Vector3(screenPosViewportClamped.x * Screen.width -halfScreenWidth, screenPosViewportClamped.y * Screen.height -halfScreenHeight, 0);
					uiIndicator2DInst.transform.localPosition = -v3;
					uiIndicator2DInst.GetComponent<UITexture>().enabled = true;
				}

				midPoint = new Vector3(Screen.width - halfScreenWidth, Screen.height - halfScreenHeight, 0);
				//		between = screenPosViewport - screenPosViewportClamped;
				between = screenPosViewport - Camera.main.ScreenToViewportPoint(midPoint);
				//		between = Camera.main.ViewportToScreenPoint(between);
				//		between = new Vector3(between.x * Screen.width - halfScreenWidth, between.y * Screen.height - halfScreenHeight, 0);
				angle = Mathf.Atan2(-between.x, -between.y) * Mathf.Rad2Deg;
				Vector3 angles = Vector3.zero;
				angles.z = -angle;
				if(uiIndicator2DInst)
				{
					uiIndicator2DInst.transform.localEulerAngles = angles;
				}
			}

//			screenPosViewport = Camera.main.WorldToViewportPoint (transform.position);
		}
//		if(screenPosViewport.x < 0 && screenPosViewport.y > 0 && screenPosViewport.y < 1)
//		{
//			testIndicator2D.rotation.eulerAngles 
//			Debug.Log ("left");
//		}
//		if(screenPosViewport.x > 1 && screenPosViewport.y > 0 && screenPosViewport.y < 1)
//		{
//			Debug.Log ("right");
//		}
//		if(screenPosViewport.y < 0 && screenPosViewport.x > 0 && screenPosViewport.x < 1)
//		{
//			Debug.Log ("down");
//		}
//		if(screenPosViewport.y > 1 && screenPosViewport.x > 0 && screenPosViewport.x < 1)
//		{
//			Debug.Log ("up");
//		}
//		if( screenPosViewport.x < 0 &&  screenPosViewport.y < 0)
//		{
//			Debug.Log ("bottomleft");
//		}
//		if( screenPosViewport.x > 1 &&  screenPosViewport.y > 1)
//		{
//			Debug.Log ("upperright");
//		}
//		if( screenPosViewport.x < 0 &&  screenPosViewport.y > 1)
//		{
//			Debug.Log ("upperleft");
//		}
//		if( screenPosViewport.x > 1 &&  screenPosViewport.y < 0)
//		{
//			Debug.Log ("bottomright");
//		}

//		testIndicator2D.LookAt(screenPosViewport);
//		print ("target is " + screenPos.x + " pixels from the left");
//		if(renderer.isVisible)
//		{
//			Debug.Log ("testtt");
//		}
//		foreach(Transform cp in chargePoints)
//		{
//			if (cp != null) {
////				Renderer[] renderers = cp.GetComponentsInChildren<Renderer>();
////			var renderers : Renderer[] = allenemies.GetComponentsInChildren.<Renderer>();
//			
//				if (!cp.gameObject.renderer.isVisible)
//				{
//
//					Debug.Log ("test");
//					// This Enemy is out of Frustrum, so we want an indicator point to its direction it.
//					
////					Vector3 worldToViewPoint = mainCam.WorldToViewportPoint (cp.position);
////					
////					// returns coming from upper left
////					
////					//worldToViewportPoint = (-0.1, 0.5, 14.8), viewportToScreenPoint =(-66.1, 361.3, 14.8)
////					
////					
////					
////	//				var screenPosClamped : Vector3  = new Vector3(Mathf.Clamp(worldToViewportPoint.x, 0.0f, 1.0f), Mathf.Clamp(worldToViewportPoint.y, 0.0f, 1.0f), 0); 
////					Vector3 screenPosClamped = new Vector3(Mathf.Clamp (worldToViewPoint.x, 0.0f, 1.0f), Mathf.Clamp(worldToViewPoint.y, 0.0f, 1.0f), 0);
////					
////					
////	//				var v3 : Vector3 = new Vector3(screenPosClamped.x * Screen.width -halfScreenWidth, screenPosClamped.y * Screen.height -halfScreenHeight, 0);
////					Vector3 v3 = new Vector3(screenPosClamped.x * Screen.width -halfScreenWidth, screenPosClamped.y * Screen.height -halfScreenHeight, 0);
////					
////					
////					//Debug.Log("enemy " + allenemies.name + " out of view at worldToViewportPoint = " + worldToViewportPoint + ", v3 =" + v3);
////					
////					testIndicator2D.localPosition = v3;
////					
////					break;
//					
//				}
//
//			}
//		}
	}
}

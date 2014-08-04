using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {
//	public GameObject player;
	public float camZoom = 50f;
	private float camZoomBase;
	public List<GameObject> players = new List<GameObject>();
	public float camXLeft;
	public float camXRight;
	public float camZTop;
	public float camZBottom;
	Camera cam;
	public float edgeAdd = 15f;
	public float noZoomEdge = 35f;	
	public bool zooming;

	public Transform midpoint;

	private Vector3 offset;

	void Start () 
	{
		camZoomBase = camZoom;
//	GetComponent<Camera>().orthographicSize = camZoom;	
//		cam = Camera.main;

		offset = transform.position - midpoint.position;

		

	}
	
	void LateUpdate () 
	{
//		Camera.main.orthographicSize = camZoom;
		Vector3 avg = CalculateAvg();
		transform.position = avg + offset;



//		float max_x = float.NegativeInfinity;
//		float max_y = float.NegativeInfinity;
//
//		for (int i = 0; i < players.Count; i++)
//		{
//			Transform playerTransform = players[i].transform;
//
//			Vector3 playerDiff_cam = transform.InverseTransformPoint(playerTransform.position - avg);
//
//			playerDiff_cam
//
//		}


		/*

		cam = Camera.main;
		Vector3 negativeCamEdge = cam.ViewportToWorldPoint(new Vector3(0,0,cam.nearClipPlane));		
		Vector3 positiveCamEdge = cam.ViewportToWorldPoint(new Vector3(1,1,cam.nearClipPlane));

		camXLeft = negativeCamEdge.x;
		camXRight = positiveCamEdge.x;
		camZTop = positiveCamEdge.z;
		camZBottom = negativeCamEdge.z;
		
		transform.position = new Vector3(avg.x -0.5f, camZoom,avg.z -5f);
		foreach(GameObject plyr in players)
		{
			if(plyr.transform.position.x < camXLeft + edgeAdd || plyr.transform.position.x > camXRight - edgeAdd || plyr.transform.position.z < camZBottom + edgeAdd || plyr.transform.position.z > camZTop - edgeAdd)
			{
//				Debug.Log ("outofbounds");	
				zooming = true;
//				camZoom += Time.deltaTime * 15;
				
			}


			else if(plyr.transform.position.x > camXLeft + noZoomEdge && plyr.transform.position.x < camXRight - noZoomEdge && plyr.transform.position.z > camZBottom + noZoomEdge && plyr.transform.position.z < camZTop - noZoomEdge)
			{
					if(camZoom > camZoomBase)
					{
						camZoom -= Time.deltaTime * 15;	
					}				
			}
//			else if (plyr.transform.position.z > camZBottom + noZoomEdge && plyr.transform.position.z < camZTop - noZoomEdge)
//			{
//				if(camZoom > camZoomBase)
//					{
//						camZoom -= Time.deltaTime * 15;	
//					}	
//			}
//			Debug.DrawLine(new Vector3(negativeCamEdge.x + edgeAdd, 0, negativeCamEdge.z + edgeAdd), new Vector3(negativeCamEdge.x + edgeAdd, 0, positiveCamEdge.z - edgeAdd));
//			Debug.DrawLine(new Vector3(negativeCamEdge.x + noZoomEdge, 0, negativeCamEdge.z + noZoomEdge), new Vector3(negativeCamEdge.x + noZoomEdge, 0, positiveCamEdge.z - noZoomEdge));

//			Debug.DrawLine(cam.ViewportToWorldPoint(new Vector3(0,0,cam.nearClipPlane)),cam.ViewportToWorldPoint(new Vector3(1,1,cam.nearClipPlane)));
		}
		*/
	}
	
	
	Vector3 CalculateAvg()
	{	
		Vector3 average = Vector3.zero;
		for (int i = 0; i < players.Count; i++)
		{
			average += players[i].gameObject.transform.position;
		}
		average /= players.Count;
		return average;
	}


}

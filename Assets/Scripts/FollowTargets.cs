using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FollowTargets : MonoBehaviour 
{
//	public Transform[] targets;
	public List<Transform> targets;
	public float zoomInTime = 1f;
	public float zoomOutTime = 0.5f;
	public float moveTime = 1f;
	public float extraSpace = 1.5f;

	public float maxZoom = 25f;
	public float minZoom = 15f;

	public float moveDistance = 30f;

	private float distance = 0f;
	private float distance_target = 0f;
	private float distance_velocity = 0f;

	public Vector3 center;
	private Vector3 center_target;
	private Vector3 center_velocity = Vector3.zero;

	void Start()
	{
		center = GetCenter();
		center_target = center;
	}

	void LateUpdate () 
	{
		if(targets == null)
		{
			Debug.Log ("no targets");
		}
		if(targets.Count != 0)
		{
			center_target = GetCenter();
			center = Vector3.SmoothDamp(center, center_target, ref center_velocity, moveTime);

			transform.position = center;

			float verticalFOV = camera.fieldOfView * Mathf.Deg2Rad * (1f/extraSpace);
			float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV * 0.5f) * camera.aspect);

			float maxDistance = 0f;
			foreach(Transform target in targets)
			{
				// Target position in camera space.
				Vector3 targetPosition_cam = transform.InverseTransformPoint(target.position);

				maxDistance = Mathf.Max(maxDistance, Mathf.Abs(targetPosition_cam.y / Mathf.Tan(verticalFOV * 0.5f)));
				maxDistance = Mathf.Max(maxDistance, Mathf.Abs(targetPosition_cam.x / Mathf.Tan(horizontalFOV * 0.5f)));
			}

			distance_target = maxDistance;

			// Smooth zooming.
			if(distance_target > distance)
			{
				distance = Mathf.SmoothDamp(distance, distance_target, ref distance_velocity, zoomOutTime);
			}
			else
			{
				distance = Mathf.SmoothDamp(distance, distance_target, ref distance_velocity, zoomInTime);
			}
	//		Debug.Log (distance_target);
			distance = Mathf.Clamp(distance, minZoom, maxZoom);
			if(distance_target > moveDistance)
			{
				transform.Translate(0f, 0f, -distance);

			}
			else
			{
				transform.Translate(0f, 0f, -distance);

			}
		}

	}

	private Vector3 GetCenter()
	{
		Vector3 center = Vector3.zero;
		foreach(Transform target in targets)
		{
			center += target.position;
		}
		
		center /= targets.Count; 
		return center;
	}
}

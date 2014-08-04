using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]  

public class IKCtrl : MonoBehaviour {
	
	protected Animator animator;
	
	public bool ikActive = true;
	public Transform rightHandObj = null;
	public Transform player;
	void Start () 
	{
		animator = GetComponent<Animator>();
	}
//	void LateUpdate()
//	{
//		rightHandObj.transform.localRotation = Quaternion.Euler(0,220,0);
//	}
	//a callback for calculating IK
	void OnAnimatorIK()
	{
		if(animator) {
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) {
				
				//weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);
//				
				//set the position and the rotation of the right hand where the external object is
				if(rightHandObj != null) {
//					animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
//					animator.SetIKRotation(AvatarIKGoal.RightHand,player.rotation);
//					animator.SetLookAtPosition(AvatarIKGoal.RightHand,player.position);
//					animator.SetIKRotation
				}					
				
			}
			
			//if the IK is not active, set the position and rotation of the hand back to the original position
			else {			
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);				
			}
		}
	}	  
}
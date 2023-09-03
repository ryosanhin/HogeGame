using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrollActionPlayer : MonoBehaviour
{
	Transform my;
	float deltaFrame;
	float y;
	
	//基本事項
	[SerializeField] float walkSpeed;
	[SerializeField] float jumpPow;
	[SerializeField] float gravity;
	bool isJump=false;
	[SerializeField] float additionalJumpMaxTime;
	float additionalJumpTime;
	[SerializeField] float additionalJumpPow;
	//基本事項
	
	//接地判定
	enum RayType{
		Ray,
		Box,
		Sphere
	}
	[Header("接地判定のRaycast")]
	[SerializeField] RayType rayType;
	[SerializeField] Vector3 rayOrigin;
	[SerializeField] Vector3 rayDirection;
	[SerializeField] float rayLength;
	[SerializeField] Vector3 rayBoxSize;
	[SerializeField] float raySphereRadius;
	int targetLayer;
	[SerializeField] int[] targetLayerNum;
	float maxDistance;
	RaycastHit hit;
	//接地判定
	
	void Start(){
		Initialize();
	}
	
	void Update(){
		deltaFrame=Time.deltaTime;
		Action();
	}
	
	void Initialize(){
		my=transform;
		additionalJumpTime=additionalJumpMaxTime;
		maxDistance+=rayLength;
		switch((int)rayType){
			case 1:
			maxDistance+=rayBoxSize.y*0.5f;
			break;
			case 2:
			maxDistance+=raySphereRadius;
			break;
		}
		foreach(int i in targetLayerNum){
			targetLayer+=1<<i;
		}
	}
	
	void Action(){
		var velocity=Vector3.right*Walk()+Vector3.up*Jump();
		my.position+=velocity;
	}
	
	float Walk(){
		float forward=0f;
		if(Input.GetKey(KeyCode.RightArrow)){
			forward++;
		}else{
			if(Input.GetKey(KeyCode.LeftArrow)){
				forward--;
			}
		}
		return forward*walkSpeed*deltaFrame;
	}
	
	float Jump(){
		bool b=IsGround();
		if(b){
			GroundCorrection(hit.distance);
			y=0f;
			isJump=false;
			additionalJumpTime=additionalJumpMaxTime;
			if(Input.GetKeyDown(KeyCode.Z)){
				y+=jumpPow;
				isJump=true;
			}
		}else{
			y-=gravity;
		}
		
		if(isJump){
			if(additionalJumpTime>0f){
				if(Input.GetKey(KeyCode.Z)){
					y+=jumpPow*additionalJumpPow+gravity;
					additionalJumpTime-=deltaFrame;
				}else{
					additionalJumpTime=0f;
				}
			}
		}
		
		return y*deltaFrame;
	}
	
	void GroundCorrection(float distance){
		Debug.Log(hit.distance);
		float f=maxDistance-distance;
		if(f<=0f){
			return;
		}
		Debug.Log("before:"+f);
		switch((int)rayType){
			case 1:
			f-=rayBoxSize.y*0.5f;
			break;
			case 2:
			f-=raySphereRadius;
			break;
		}
		Debug.Log("after:"+f);
		my.position+=Vector3.up*f;
	}
	
	bool IsGround(){
		bool isHit=false;
		var rayOriginPos=my.position+rayOrigin;
		switch((int)rayType){
			case 0:
			isHit=Physics.Raycast(rayOriginPos,rayDirection,out hit,rayLength,targetLayer);
			break;
			
			case 1:
			isHit=Physics.BoxCast(rayOriginPos,rayBoxSize*0.5f,rayDirection,out hit,Quaternion.identity,rayLength,targetLayer);
			break;
			
			case 2:
			isHit=Physics.SphereCast(rayOriginPos,raySphereRadius,rayDirection,out hit,rayLength,targetLayer);
			break;
		}
		
		return isHit;
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmosSelected(){
		var gizmoOrigin=transform.position+rayOrigin;
		switch((int)rayType){
			case 1:
			Gizmos.DrawWireCube(gizmoOrigin,rayBoxSize);
			break;
			case 2:
			Gizmos.DrawWireSphere(gizmoOrigin,raySphereRadius);
			break;
		}
		
		Gizmos.color=Color.red;
		switch((int)rayType){
			case 1:
			Gizmos.DrawWireCube(gizmoOrigin+rayDirection*rayLength,rayBoxSize);
			break;
			case 2:
			Gizmos.DrawWireSphere(gizmoOrigin+rayDirection*rayLength,raySphereRadius);
			break;
		}
		
		Gizmos.DrawLine(gizmoOrigin,gizmoOrigin+rayDirection*rayLength);
	}
	#endif
}

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
	RaycastHit hitGround;
	public bool isGround=false;
	//接地判定
	
	//横方向移動時の判定
	[Header("画面の進行方向")]
	[SerializeField] Vector3 rayDirectionForward;
	Quaternion reverseVector;
	//横方向移動時の判定
	
	//前の衝突判定
	[Header("進行方向の障害物判定のRaycast")]
	[SerializeField] Vector3 rayOriginForward;
	[SerializeField] float rayLengthForward;
	//前の衝突判定
	
	//坂道判定
	[Header("坂道判定のRaycast")]
	[SerializeField] Vector3 rayOriginHill;
	[SerializeField] float rayLengthHill;
	RaycastHit hitHill;
	public bool isHill=false;
	//坂道判定
	
	void Start(){
		Initialize();
		reverseVector.eulerAngles=Vector3.up*180f;
	}
	
	void Update(){
		deltaFrame=Time.deltaTime;
		Action();
	}
	
	void Initialize(){
		my=transform;
		additionalJumpTime=additionalJumpMaxTime;
		maxDistance+=rayLength;
		
		foreach(int i in targetLayerNum){
			targetLayer+=1<<i;
		}
	}
	
	void Action(){
		var velocity=Walk()+Vector3.up*Jump();
		my.position+=velocity;
	}
	
	Vector3 Walk(){
		float forward=0f;
		if(Input.GetKey(KeyCode.RightArrow)){
			forward++;
		}else{
			if(Input.GetKey(KeyCode.LeftArrow)){
				forward--;
			}
		}
		var tan=HillClimb(forward);
		if(IsForward(forward)){
			forward=0f;
		}
		return (Vector3.right+Vector3.up*tan)*forward*walkSpeed*deltaFrame;
	}
	
	bool IsForward(float f){
		if(f==0f){
			return false;
		}
		var rayOriginPos=my.position+rayOriginForward;
		var vct=rayDirectionForward;
		if(f==-1f)vct=reverseVector*vct;
		
		return Physics.Raycast(rayOriginPos,vct,rayLengthForward,targetLayer);
	}
	
	float HillClimb(float f){
		var rayOriginPos=my.position+rayOriginHill;
		var b=false;
		var vct=rayDirectionForward;
		
		bool ShootRay(){
			return Physics.Raycast(rayOriginPos,vct,out hitHill,rayLengthHill,targetLayer);
		}
		
		if(f==-1f)vct=reverseVector*vct;
		
		b=ShootRay();
		
		if(b && Mathf.Abs(hitHill.normal.y)!=0f){
			isHill=true;
		}else{
			vct=reverseVector*vct;
			b=ShootRay();
			if(b && Mathf.Abs(hitHill.normal.y)!=0f){
				isHill=true;
			}else{
				isHill=false;
			}
		}
		
		if(b)return -hitHill.normal.x/hitHill.normal.y;
		return 0f;
	}
	
	float Jump(){
		isGround=IsGround();
		
		if(isGround || (!isGround && isHill)){
			GroundCorrection();
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
	
	void GroundCorrection(){
		if(!isGround){
			return;
		}
		var f=maxDistance-hitGround.distance;
		Debug.Log(f);
		my.position+=Vector3.up*f;
	}
	
	bool IsGround(){
		bool isHit=false;
		var rayOriginPos=my.position+rayOrigin;
		switch((int)rayType){
			case 0:
			isHit=Physics.Raycast(rayOriginPos,rayDirection,out hitGround,rayLength,targetLayer);
			break;
			
			case 1:
			isHit=Physics.BoxCast(rayOriginPos,rayBoxSize*0.5f,rayDirection,out hitGround,Quaternion.identity,rayLength,targetLayer);
			break;
			
			case 2:
			isHit=Physics.SphereCast(rayOriginPos,raySphereRadius,rayDirection,out hitGround,rayLength,targetLayer);
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
		
		gizmoOrigin=transform.position+rayOriginHill;
		Gizmos.DrawLine(gizmoOrigin,gizmoOrigin+rayDirectionForward*rayLengthHill);
		Gizmos.DrawLine(gizmoOrigin,gizmoOrigin-rayDirectionForward*rayLengthHill);
		
		gizmoOrigin=transform.position+rayOriginForward;
		Gizmos.DrawLine(gizmoOrigin,gizmoOrigin+rayDirectionForward*rayLengthForward);
	}
	#endif
}

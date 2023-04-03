using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroController : MonoBehaviour,IControllable {
	//dont modify this values
	public float speed = 0;
	public float speedY=0;
	public float jumpDistance=0;
	//dont modify this values

	public float jumpSpeed = 9;
	//public float jumpHeight = 4f;
	public float jumpHeightLimit = 30f;


	public float fallMaxSpeed = 18f;
	public float jumpMaxSpeed = 14f;
	public float runMaxSpeed = 10f;
	public float walkMaxSpeed = 5f;
	public float airMaxSpeed = 20f;

	public float acceleration = 5f;
	public float friction = 30f;
	public float airAcceleration = 3f;
	public float decelerationTime = 1f;
	public float gravity = 20f;

	public float hitDelay=1f;

	public float brickRebound =1f;
	public float wallSlideSpeed =3f;
	public float wallJumpPower =800f;
	public float maxWallJumpSpeed =20f;
	public float wallSlideSmoothing = 20.0F;
	
	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;

	public bool isDestroyBrick=false;
	public bool isLeftBtnPress{set;get;}
	public bool isRightBtnPress{set;get;}

	//actions
	public bool isIdle{set;get;}
	public bool isWalking{set;get;}
	public bool isRunning{set;get;}
	public bool isHitWall{set;get;}
	public bool isMovingLeft{set;get;}
	public bool isMovingRight{set;get;}
	public bool isFacingRight{set;get;}
	public bool isFacingLeft{set;get;}
	public bool isLookingUp{set;get;}
	public bool isLookingDown{set;get;}
	public bool isInAir{set;get;}
	public bool isFalling{set;get;}
	public bool isHoldingAction{set;get;}
	public bool isHoldingSomething{set;get;}
	public bool isThrow{set;get;}
	public bool isJumping{set;get;}
	public bool isBouncing{set;get;}
	public bool isDoubleJumping{set;get;}
	public bool isHit{set;get;}
	public bool isDead{set;get;}

	//skills
	public bool hasDoubleJump=false;
	public bool canMoveInAir=true;

	//new
	public bool hasUnlimitedJump=false;
	public bool alwaysMoveRight=false;

	//events
	public Action HitComplete;
	private bool hitLeftSide;
	private bool hitRightSide;
	private bool hitUpSide;
	private bool hitDownSide;

	//new
	public bool ApplyGravity=true;
	public bool alwaysRun;

	public string id;

	private float oldPositionY;
	private float newPositionY;

	public virtual void Start(){
		id = UniqueIdGenerator.GenerateId();
		controller = GetComponent<CharacterController>();
		ResetState();
	}

	public void ResetState(){
		ResetStateOnLand();
		//es
		isFacingRight =true;
		isHit  =false;
		isDead = false;
		isIdle =false;
		// es
	}

	private void ResetStateOnLand(){
		//isInAir =false;
		isInAir =true;
		isJumping =false;
		isDoubleJumping =false;
		isWalking =true;
		isRunning = false;
		//isFalling = true;
		isFalling = false;
		isBouncing = false;
		jumpDistance=0;
		moveDirection = Vector3.zero;
	}

	public bool isHitRightSide{
		set{hitRightSide=value;}
		get{ return hitRightSide;}
	}

	public bool isHitLeftSide{
		set{hitLeftSide=value;}
		get{ return hitLeftSide;}
	}

	public bool isHitUpSide{
		set{hitUpSide=value;}
		get{ return hitUpSide;}
	}

	public bool isHitDownSide{
		set{hitDownSide=value;}
		get{ return hitDownSide;}
	}


	public virtual void OnControllerColliderHit(ControllerColliderHit hit){
		//Debug.Log(" check hit.moveDirection.x " + hit.moveDirection.x);
		if(hit.moveDirection.x < -0.99f){
			//hit left side
			hitLeftSide = true;
			hitRightSide =false;
			//Debug.Log("hit left");
		}else if(hit.moveDirection.x > 0.99f){
			//hit right side
			hitLeftSide = false;
			hitRightSide =true;
			//Debug.Log("hit right");
		}

		//Debug.Log("Hero Controller collided with " + hit.gameObject.tag);
		//Debug.Log(" hit.point " + hit.point);
		Debug.DrawRay ( hit.point, hit.normal,Color.red );
		if ( hit.moveDirection.y > 0.01){
			hitDownSide =false;
			hitUpSide =true;
			if(!isFalling){
				moveDirection.y -= speedY * brickRebound;
			}
			//moveDirection.y -= (jumpSpeed * brickRebound);
			//Debug.Log("hit.moveLength " + hit.moveLength);
			//moveDirection.y -=hit.moveLength;
			//Debug.Log("hit up");
			return;
		}else if ( hit.moveDirection.y < -0.99f){
			//dont do anything stupid here this is called always when character is in contact with any ground object
			//take cautions when adding new variable here
			/*isInAir =false;
			isJumping =false;
			isDoubleJumping = false;
			isBouncing = false;
			jumpDistance=0;
			moveDirection = Vector3.zero;
			isFalling =false;
			*/
			ResetStateOnLand();
			hitDownSide =true;
			hitUpSide =false;
			//Debug.Log("hit down");
			//Debug.Log("landed on: " + hit.gameObject.tag);
		}else  if (hit.normal.y < 0.707){
			//character collided with another thing (not the ground)
			//Debug.Log("character collided with another thing (not the ground): " + hit.gameObject.tag);
		}
	}

	public virtual void Update(){
		UpdateMovement();
		UpdateGravity();
		LimitSpeed();
		LimitJumpSpeed();
	}

	public virtual void LimitSpeed(){
		if(alwaysRun){
			isRunning =true;
		}

		if(isFacingRight){
			if(isRunning){
				if(speed>runMaxSpeed){
					speed=runMaxSpeed;
					//Debug.Log("reach max walking  right");
				}
			}else if(isWalking){
				if(speed>walkMaxSpeed){
					speed=walkMaxSpeed;
					//Debug.Log("reach max walking right");
				}
			}else if(isJumping || isFalling){
				if(speed>airMaxSpeed){
					speed=airMaxSpeed;
					//Debug.Log("reach max walking  right");
				}
			}
			//Debug.Log( "lmit walk speed id " + id );
		}else if(isFacingLeft){
			if(isRunning){
				if(speed<-runMaxSpeed){
					speed=-runMaxSpeed;
					//Debug.Log("reach running max left");
				}
			}else if(isWalking){
				if(speed<-walkMaxSpeed){
					speed=-walkMaxSpeed;
					//Debug.Log("reach walkingmax left");
				}
			}else if(isJumping || isFalling){
				if(speed<-airMaxSpeed){
					speed=-airMaxSpeed;
					//Debug.Log("reach running max left");
				}
			}
			//Debug.Log( "lmit run speed id " + id );
		}
	}

	private void LimitJumpSpeed(){
		if(moveDirection.y >= jumpMaxSpeed){
			//Debug.Log(" exceed going up moveDirection.y " + moveDirection.y);
			moveDirection.y = jumpMaxSpeed;
		}
		
		if(isFalling){
			if(moveDirection.y <= -fallMaxSpeed){
				//Debug.Log(" exceed going down moveDirection.y " + moveDirection.y);
				moveDirection.y = -fallMaxSpeed;
			}
		}
	}


	private void UpdateMovement(){
		if(alwaysMoveRight && !isDead){
			isIdle =false;
			MoveRight();
		}else{
			if (isLeftBtnPress && !isDead){
				isIdle =false;
				MoveLeft();
			}else if (isRightBtnPress && !isDead){
				isIdle =false;
				MoveRight();
			}else if (!isLeftBtnPress && !isRightBtnPress && !isDead){
				isIdle =true;
				isWalking =false;
				isRunning =false;
				Deceleration();
			}else if (isDead){
				isIdle =false;
				isWalking =false;
				isRunning =false;
				Deceleration();
			}
		}

		if((isJumping || isDoubleJumping || isBouncing) && !isDead){
			MoveUp();
		}

		moveDirection.x = Mathf.Lerp(moveDirection.x, speed, 1f);
		//moveDirection.x = speed;

		//dont call this per frame
		if(controller!=null){
			controller.Move( moveDirection * Time.deltaTime);
		}
		//Debug.Log("Hero x" + this.gameObject.transform.position.x);
	}

	private void UpdateGravity(){
		if(!ApplyGravity)return;

		newPositionY = this.gameObject.transform.position.y;
		if(oldPositionY != newPositionY){
			if(newPositionY > oldPositionY){
				//Debug.Log("jumping");
				isFalling =false;
			}else{
				//Debug.Log("falling");
				isFalling =true;
			}
			oldPositionY = newPositionY;
			//Debug.Log("update position y");
		}else{
			//Debug.Log("dont update position y");
		}

		float curSmooth = wallSlideSmoothing * Time.deltaTime;
		float fallSpeed;

		if(isHitWall && moveDirection.y < 0){
			fallSpeed = wallSlideSpeed * Time.deltaTime;
			moveDirection.y = Mathf.Lerp(moveDirection.y, moveDirection.y - fallSpeed, curSmooth);
		}else{
			moveDirection.y = Mathf.Lerp(moveDirection.y,moveDirection.y -(gravity * Time.deltaTime),1f);
			//moveDirection.y -=(gravity * Time.deltaTime);
		}

		speedY = moveDirection.y;
	}

	private void Deceleration(){
		if(speed > friction * Time.deltaTime){
			//speed = speed - friction * Time.deltaTime;
			speed = Mathf.Lerp(speed, speed - friction * Time.deltaTime, decelerationTime);
		}else if(speed < -friction * Time.deltaTime){
			//speed = speed + friction * Time.deltaTime;
			speed = Mathf.Lerp(speed, speed + friction * Time.deltaTime,decelerationTime);
		}else{
			speed = 0;
		}
		//Debug.Log("deceleration!...");
	}


	public virtual void Hit(){
		if(!isHit && !isDead){
			speed=0;
			isHit =true;
			Invoke("RefreshHit",hitDelay);
			//Debug.Log("got hit");
		}
	}

	private void RefreshHit(){
		isHit =false;
		//Debug.Log("hit refresh");
		if(null!= HitComplete){
			HitComplete();
		}
	}

	/*
	//0-1
	public void Bounce( float percent = 0.5f ){
		isBouncing = true;
		//moveDirection.y = 0;
		moveDirection = Vector3.zero;
		jumpDistance = jumpHeight * percent;
		isIdle =false;
		isInAir =true;
		//Debug.Log("go bounce!!");
	}*/

	public void Jump(){
		//if(!isInAir && !isFalling || (hasUnlimitedJump && !isJumping)){
		//if(!isInAir && !isFalling || !isJumping){
		if(!isJumping){
			//moveDirection.y = 0;
			moveDirection = Vector3.zero;
			jumpDistance=0;
			isJumping =true;
			isIdle =false;
			isInAir =true;
			//Debug.Log("jump!!");
		}else if(!isJumping && isInAir && hasDoubleJump && !isDoubleJumping && !isHitWall){
			//moveDirection.y = 0;
			moveDirection = Vector3.zero;
			jumpDistance=0;
			isDoubleJumping =true;
			isIdle =false;
			isInAir =true;
		}

		if(isHitWall && isInAir && moveDirection.y < 0 ){
			jumpDistance=0;
			isJumping =true;


			if(isMovingLeft){
				WallJumpRight();
				isFacingLeft =false;
				isFacingRight =true;

				isMovingLeft = false;
				isMovingRight =true;
			}else if(isMovingRight){
				WallJumpLeft();
				isFacingLeft =true;
				isFacingRight=false;

				isMovingLeft = true;
				isMovingRight =false;
			}
		}
	}

	private void MoveUp(){
		if(this.gameObject.transform.position.y > jumpHeightLimit)return;
		moveDirection.y =jumpSpeed;
		isJumping = false;
	}

	public void MoveLeft(){
		if (isInAir || canMoveInAir){
			isMovingLeft =true;
			isMovingRight =false;

			isFacingLeft =true;
			isFacingRight =false;

			if(speed > 0){
				speed = 0;
			}

			if(isInAir){
				speed= (speed - airAcceleration * Time.deltaTime);
			}else{
				speed= (speed - acceleration  * Time.deltaTime);
			}
		}
	}

	private void WallJumpLeft(){
		speed = 0;
		speed= (speed - wallJumpPower  * Time.deltaTime);
	}

	private void WallJumpRight(){
		speed = 0;
		speed= (speed + wallJumpPower  * Time.deltaTime);
	}

	public void MoveRight(){
		if (!isInAir || canMoveInAir){
			isMovingLeft =false;
			isMovingRight =true;
			//pushColliderController.SwitchGrabCollider(0);

			isFacingLeft =false;
			isFacingRight =true;

			if(speed < 0){
				speed = 0;
			}

			if(isInAir){
				speed= (speed + airAcceleration * Time.deltaTime);
			}else{
				speed= (speed + acceleration * Time.deltaTime);
			}
		}
	}


	//dont use this will accelerate movement of character when move again
	public void StopMoving(){
		isLeftBtnPress =false;
		isRightBtnPress =false;

		speed =0;
		isIdle =true;
		isWalking =false;
		isRunning =false;
		moveDirection = Vector3.zero;
	}

	private void Message(string msg){
		Debug.Log(msg);
	}
}
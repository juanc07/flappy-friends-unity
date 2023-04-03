using UnityEngine;
using System.Collections;

public interface IControllable{
	bool isLeftBtnPress{set;get;}
	bool isRightBtnPress{set;get;}

	bool isIdle{set;get;}
	bool isWalking{set;get;}
	bool isRunning{set;get;}
	bool isHitWall{set;get;}
	bool isMovingLeft{set;get;}
	bool isMovingRight{set;get;}
	bool isFacingRight{set;get;}
	bool isFacingLeft{set;get;}
	bool isInAir{set;get;}
	bool isFalling{set;get;}
	bool isHoldingAction{set;get;}
	bool isHoldingSomething{set;get;}
	bool isThrow{set;get;}
	bool isJumping{set;get;}
	bool isDoubleJumping{set;get;}


	void Jump();
	void MoveLeft();
	void MoveRight();
}

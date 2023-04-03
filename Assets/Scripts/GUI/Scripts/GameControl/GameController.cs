using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public HeroController heroController;
	private GameDataManager gameDataManager;
	private SoundManager soundManager;

	// Use this for initialization
	void Start (){
		gameDataManager = GameDataManager.GetInstance();
		soundManager = SoundManager.GetInstance();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
	// Update is called once per frame
	void Update (){
		if(gameDataManager.IsLevelComplete){
			heroController.isLeftBtnPress =false;
			heroController.isRightBtnPress =false;

			heroController.isLookingUp =false;
			heroController.isLookingDown =false;

			heroController.isRunning =false;
			heroController.isWalking =false;
			return;
		}

		float h = Input.GetAxis("Horizontal");

		if(h < 0 || Input.GetKey(KeyCode.LeftArrow)){
			heroController.isLeftBtnPress =true;
			heroController.isRightBtnPress =false;
			//Debug.Log("move left");
		}else if(h > 0 || Input.GetKey(KeyCode.RightArrow)){
			heroController.isLeftBtnPress =false;
			heroController.isRightBtnPress =true;
			//Debug.Log("move right");
		}else{
			heroController.isLeftBtnPress =false;
			heroController.isRightBtnPress =false;
		}
		
		float v = Input.GetAxis("Vertical");
		
		if(v < 0){
			//Debug.Log("down");
			heroController.isLookingUp =false;
			heroController.isLookingDown =true;
		}else if(v > 0){
			heroController.isLookingUp =true;
			heroController.isLookingDown =false;
			//Debug.Log("up");
		}else{
			heroController.isLookingUp =false;
			heroController.isLookingDown =false;
		}

		float f3 = Input.GetAxis("Fire3");
		bool downF3 = Input.GetButtonDown("Fire3");
		bool heldF3 = Input.GetButton("Fire3");
		bool upF3 = Input.GetButtonUp("Fire3");

		if(f3>0 || downF3){
			//Debug.Log("fire3!");
		}

		if(heldF3 || Input.GetKey(KeyCode.LeftShift)){
			if(!heroController.isInAir){
				heroController.isRunning =true;
				heroController.isWalking =false;
			}
		}else if(upF3){
			if(heroController.alwaysRun){
				heroController.isRunning =true;
				heroController.isWalking =false;
			}else{
				heroController.isRunning =false;
				heroController.isWalking =true;
			}
		}else{
			if(heroController.alwaysRun){
				heroController.isRunning =true;
				heroController.isWalking =false;
			}else{
				heroController.isRunning =false;
				heroController.isWalking =true;
			}
		}


		float f1 = Input.GetAxis("Fire1");
		//bool downF1 = Input.GetButtonDown("Fire1");
		bool heldF1 = Input.GetButton("Fire1");
		bool upF1 = Input.GetButtonUp("Fire1");

		//bool downJump = Input.GetButtonDown("Jump");
		bool heldJump = Input.GetButton("Jump");
		bool upJump = Input.GetButtonUp("Jump");

		if(f1>0){
			//Debug.Log("fire1!");
		}

		if(heldF1){
			//Debug.Log("held fire1!");
		}

		if (Input.GetButtonDown("Fire1")){
			//Debug.Log("downF1");
			heroController.Jump();
			if(!heroController.isDead){
				soundManager.PlaySfx2(SFX.flap3,0.60f);
			}

			if(!gameDataManager.IsLevelStart){
				gameDataManager.IsLevelStart=true;
			}
		}

		if(heldF1 || heldJump){		
			/*heroController.Jump();
			soundManager.PlaySfx2(SFX.flap2Amplify);
			if(!gameDataManager.IsLevelStart){
				gameDataManager.IsLevelStart=true;
			}*/
			//Debug.Log("heldF1");
		}

		if(upF1 || upJump ){
		//}else if(upJump ){
			//Debug.Log("upF1");
			heroController.isJumping =false;
		}

		//float f2 = Input.GetAxis("Fire2");
		bool downFire2= Input.GetButtonDown("Fire2");
		bool heldFire2 = Input.GetButton("Fire2");
		bool upFire2 = Input.GetButtonUp("Fire2");

		if(heldFire2){
			heroController.isHoldingAction=true;
			//Debug.Log("fire2!");
		}else if(upFire2){
			heroController.isHoldingAction=false;
		}

		if(downFire2 && heroController.isHoldingSomething){
			if(!heroController.isThrow){
				heroController.isThrow =true;
			}
		}

		//Debug.Log("j " + j);
		//Debug.Log("h " + h);
		//Debug.Log("v " + v);
	}
	#endif
}

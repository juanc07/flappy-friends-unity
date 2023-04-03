using UnityEngine;
using System.Collections;

public class FireBtn : MonoBehaviour {

	private HeroController heroController;
	private bool isPressed =false;
	private GameDataManager gameDataManager;
	private MobileController mobileController;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		mobileController = GameObject.FindObjectOfType(typeof(MobileController)) as MobileController;
		heroController = mobileController.heroController;
	}
	
	private void OnClick(){
		//Debug.Log("fire!!");
	}


	private void Run(){
		heroController.isRunning = true;
		heroController.isWalking = false;
	}

	private void Walk(){
		heroController.isRunning = false;
		heroController.isWalking = true;
	}


	private void Update(){
		if(gameDataManager.IsLevelComplete){
			heroController.isHoldingAction=false;
			heroController.isWalking = false;
			heroController.isRunning = false;
			return;
		}

		if(isPressed){
			if(!heroController.isInAir && !heroController.isDead){
				Run();
			}
		}else{
			heroController.isHoldingAction=false;
			if(heroController.alwaysRun){
				Run();
			}else{
				Walk();
			}
		}
	}

	private void OnPress(bool isDown){
		isPressed = isDown;
		if(isPressed && heroController.isIdle){
			if(!heroController.isHoldingSomething){
				heroController.isHoldingAction=true;
			}else if(  heroController.isHoldingSomething){
				heroController.isThrow =true;
			}
		}
	}
}

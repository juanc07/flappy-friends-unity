using UnityEngine;
using System.Collections;

public class LeftBtn : MonoBehaviour {

	private HeroController heroController;
	private bool isPressed=false;
	private GameDataManager gameDataManager;
	private MobileController mobileController;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		mobileController = GameObject.FindObjectOfType(typeof(MobileController)) as MobileController;
		heroController = mobileController.heroController;
	}

	private void Update(){
		if(gameDataManager.IsLevelComplete){
			heroController.isLeftBtnPress =false;
			return;
		}

		if(isPressed && !heroController.isDead){
			heroController.isLeftBtnPress = true;
			heroController.isRightBtnPress = false;
		}else{
			heroController.isLeftBtnPress =false;
		}
	}
	
	private void OnPress(bool isDown){
		isPressed = isDown;

		/*
		heroController.isLeftBtnPress = isDown;

		if(isPressed && !heroController.isDead){
			heroController.isWalking =true;
			heroController.isRunning =false;
		}*/
	}
}

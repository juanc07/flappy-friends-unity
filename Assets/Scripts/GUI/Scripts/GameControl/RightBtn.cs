using UnityEngine;
using System.Collections;

public class RightBtn : MonoBehaviour {
	
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
			heroController.isRightBtnPress =false;
			return;
		}

		if(isPressed && !heroController.isDead ){
			heroController.isRightBtnPress = true;
			heroController.isLeftBtnPress = false;
		}else{
			heroController.isRightBtnPress =false;
		}
	}
	
	private void OnPress(bool isDown){
		isPressed = isDown;

		/*
		heroController.isRightBtnPress = isDown;
		if(isPressed && !heroController.isDead ){
			heroController.isWalking =true;
			heroController.isRunning =false;
		}*/
	}
}

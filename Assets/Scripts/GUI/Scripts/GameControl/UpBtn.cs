using UnityEngine;
using System.Collections;

public class UpBtn : MonoBehaviour {
	
	private HeroController heroController;
	private bool isPressed=false;
	private MobileController mobileController;
	private GameDataManager gameDataManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		mobileController = GameObject.FindObjectOfType(typeof(MobileController)) as MobileController;
		heroController = mobileController.heroController;
	}
	
	private void Update(){
		if(gameDataManager.IsLevelComplete){
			heroController.isLookingUp =false;
			return;
		}

		if(isPressed && !heroController.isDead){
			heroController.isLookingUp = true;
			heroController.isLookingDown = false;
		}else{
			heroController.isLookingUp =false;
		}
	}
	
	private void OnPress(bool isDown){
		isPressed = isDown;
	}
}

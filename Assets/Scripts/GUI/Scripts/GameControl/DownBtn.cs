using UnityEngine;
using System.Collections;

public class DownBtn : MonoBehaviour {
	
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
		if(heroController==null)return;

		if(gameDataManager.IsLevelComplete){
			heroController.isLookingDown =false;
			return;
		}

		if(isPressed && !heroController.isDead){
			heroController.isLookingUp = false;
			heroController.isLookingDown = true;
		}else{
			heroController.isLookingDown =false;
		}
	}
	
	private void OnPress(bool isDown){
		isPressed = isDown;
	}
}

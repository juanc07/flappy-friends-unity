using UnityEngine;
using System.Collections;

public class JumpBtn : MonoBehaviour {

	private HeroController heroController;
	private bool isPress=false;
	private GameDataManager gameDataManager;
	private MobileController mobileController;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		mobileController = GameObject.FindObjectOfType(typeof(MobileController)) as MobileController;
		heroController = mobileController.heroController;
	}

	private void Update(){
		if(heroController==null)return;

		if(gameDataManager.IsLevelComplete){
			heroController.isJumping =false;
			return;
		}

		if(isPress){
			heroController.Jump();
		}else if(!isPress){
			heroController.isJumping =false;
		}
	}
	
	private void OnClick(){
		//heroController.Jump();
		//Debug.Log("jump");
	}


	private void OnDoubleClick (){
		//heroController.Jump();
		//Debug.Log("double jump");
	}


	private void OnPress(bool isDown){
		isPress = isDown;
	}
}

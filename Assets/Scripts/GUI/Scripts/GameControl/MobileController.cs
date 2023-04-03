using UnityEngine;
using System.Collections;

public class MobileController : MonoBehaviour {

	public GameObject mobileUI;
	public HeroController heroController;
	public bool autoHideMobileUI=false;

	//private bool isPressed=false;

	public bool disableDigitalButton=false;
	public GameObject upBtn;
	public GameObject downBtn;
	public GameObject leftBtn;
	public GameObject rightBtn;
	public GameObject jumpBtn;
	public GameObject actionBtn;

	private GameDataManager gameDataManager;
	private SoundManager soundManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		soundManager = SoundManager.GetInstance();

		#if UNITY_EDITOR
		if(autoHideMobileUI){
			mobileUI.gameObject.SetActive(false);
		}
		#endif

		if(disableDigitalButton){
			upBtn.SetActive(false);
			downBtn.SetActive(false);
			leftBtn.SetActive(false);
			rightBtn.SetActive(false);
			jumpBtn.SetActive(false);
			actionBtn.SetActive(false);
		}
	}

	void Update() {
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
		bool heldF1 = Input.GetButton("Fire1");
		bool upF1 = Input.GetButtonUp("Fire1");

		if (heldF1){
			heroController.Jump();
			if(!heroController.isDead){
				soundManager.PlaySfx2(SFX.flap3,0.60f);
			}
			//Debug.Log("jump now");
			if(!gameDataManager.IsLevelStart){
				gameDataManager.IsLevelStart=true;
			}
		}else if(upF1){
			heroController.isJumping =false;
			//Debug.Log("fall now");
		}
		#endif

		#if UNITY_ANDROID || UNITY_IPHONE
		/*if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
			isPressed =true;
		}else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
			isPressed =false;
		}*/


		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began){
				//isPressed =true;
				gameDataManager.IsLevelStart=true;
				heroController.Jump();
				if(!heroController.isDead){
					soundManager.PlaySfx2(SFX.flap3,1f);
				}
			}else if (touch.phase == TouchPhase.Stationary){
				//isPressed =true;
			}else if(touch.phase == TouchPhase.Ended){
				//isPressed =false;
				heroController.isJumping =false;
			}
		}

		/*
		if(isPressed){
			heroController.Jump();
		}else if(!isPressed){
			heroController.isJumping =false;
		}*/

		#endif
	}

}

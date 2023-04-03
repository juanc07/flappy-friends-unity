using UnityEngine;
using System.Collections;

public class MusicButtonClick : MonoBehaviour {

	private SoundManager soundManager;
	private bool isActivated = true;
	
	// Use this for initialization
	void Start () {	
		soundManager = SoundManager.GetInstance();
		soundManager.OnBGMLoaded +=OnBGMLoaded;
	}

	private void OnDestroy(){
		soundManager.OnBGMLoaded -=OnBGMLoaded;
	}

	private void OnBGMLoaded(){
		CheckButtons();
	}

	private void CheckButtons(){
		if(soundManager!=null){
			//Debug.Log( "check isBgmOn " + soundManager.isBgmOn );
			EnableDisableButton(soundManager.isBgmOn);
		}
	}

	private void OnEnable(){
		Invoke("CheckButtons",0.07f);
		//Debug.Log("on enable music");
	}
	
	private void OnClick(){
		if(soundManager.isBgmOn){
			EnableDisableButton(false);
		}else{
			EnableDisableButton(true);
		}
	}
	
	private void EnableDisableButton( bool turnOn ){
		UISprite  buttonImage =  this.gameObject.transform.GetComponentInChildren<UISprite>();
		Color32 color;
		if(turnOn){
			isActivated =true;
			if(buttonImage!=null){
				color = new Color32(255,255,255,255);
				buttonImage.color = color;
			}

			if(soundManager!=null){
				if(soundManager.isBGMLoaded){
					soundManager.UnMuteBGM();
				}
			}
		}else {
			isActivated =false;
			if(buttonImage!=null){
				color = new Color32(100,100,100,255);
				buttonImage.color = color;
			}

			if(soundManager!=null){
				if(soundManager.isBGMLoaded){
					soundManager.MuteBGM();
				}
			}
		}
	}
}

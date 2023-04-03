using UnityEngine;
using System.Collections;

public class SfxButtonClick : MonoBehaviour {

	private SoundManager soundManager;
	private bool isActivated = true;

	// Use this for initialization
	void Start () {
		soundManager = SoundManager.GetInstance();
	}
	
	private void OnClick(){
		if(soundManager.isSfxOn){
			EnableDisableButton(false);
		}else{
			EnableDisableButton(true);
		}
	}

	private void CheckButton(){
		if(soundManager!=null){
			//Debug.Log( "check isSfxOn " + soundManager.isSfxOn );
			EnableDisableButton(soundManager.isSfxOn);
		}
	}

	private void OnEnable(){
		Invoke("CheckButton",0.07f);
	}

	private void EnableDisableButton( bool turnOn ){
		UISprite  buttonImage =  this.gameObject.transform.GetComponentInChildren<UISprite>();
		Color32 color;

		if(turnOn){
			if(buttonImage!=null){
				color = new Color32(255,255,255,255);
				buttonImage.color = color;
			}

			if(soundManager!=null){
				if(soundManager.isSFXLoaded){
					soundManager.UnMuteSfx();
				}
			}
		}else {
			if(buttonImage!=null){
				color = new Color32(100,100,100,255);
				buttonImage.color = color;
			}

			if(soundManager!=null){
				if(soundManager.isSFXLoaded){
					soundManager.MuteSfx();
				}
			}
		}
	}
}

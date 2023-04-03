using UnityEngine;
using System.Collections;

public class SoundManagerCaller : MonoBehaviour {

	private SoundManager soundManager;

	// Use this for initialization
	void Start (){
		soundManager = SoundManager.GetInstance();
		soundManager.OnSFXLoaded+=PlaySFXTest;
		soundManager.OnBGMLoaded+=PlayBGM;
	}

	private void OnDestroy(){
		if(soundManager!=null){
			soundManager.OnSFXLoaded-=PlaySFXTest;
			soundManager.OnBGMLoaded-=PlayBGM;
		}
	}

	private void PlayBGM(){
		soundManager.PlayBGM(BGM.BGM1,0.18f);
	}

	private void PlaySFXTest(){
		//soundManager.PlaySfx(SFX.punch);
	}

	private void PlaySFX(){
		//soundManager.PlaySfx(SFX.BombRelease);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && soundManager.isSFXLoaded){
			//soundManager.PlaySfx(SFX.coin3);
		}			
	}
}

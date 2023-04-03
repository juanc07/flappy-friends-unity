using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {

	private GameDataManager gameDataManager;

	public GameObject levelHUDPanel;
	public GameObject levelCompletePopup;
	public GameObject retryLevelPopup;
	public GameObject optionPopup;


	//google play
	//private GPSArtOfByte gps;

	//chartboost
	//private ChartboostBridgeManager chartboostBridgeManager;
	private SoundManager soundManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		soundManager = SoundManager.GetInstance();
		soundManager.OnBGMLoaded += OnBGMLoaded;
		if(soundManager.isBGMLoaded){
			PlayBGM();
		}

		//chartboostBridgeManager =  ChartboostBridgeManager.GetInstance();

		//gps = GPSArtOfByte.GetInstance();

		AddEventListener();

		string sceneName = Application.loadedLevelName;

		if(sceneName == "Game"){
			levelHUDPanel.SetActive(true);
		}else{
			levelHUDPanel.SetActive(false);
		}
	}

	private void OnBGMLoaded(){
		PlayBGM();
	}

	private void PlayBGM(){
		if(!soundManager.isBGMPlaying){
			soundManager.PlayBGM(BGM.BGM1,0.18f);
		}
	}

	private void OnDestroy(){
		RemoveEventListener();
	}

	private void AddEventListener(){
		if(gameDataManager!=null){
			gameDataManager.OnLevelStart+=OnLevelStart;
			gameDataManager.OnLevelComplete+= OnLevelComplete;
			gameDataManager.player.OnPlayerDead+=OnPlayerDead;
		}
	}

	private void RemoveEventListener(){
		if(gameDataManager!=null){
			gameDataManager.OnLevelStart-=OnLevelStart;
			gameDataManager.OnLevelComplete-= OnLevelComplete;
			gameDataManager.player.OnPlayerDead-=OnPlayerDead;
		}
	}

	public void ShowHideOption(bool val){
		optionPopup.SetActive(val);
	}

	private void OnLevelStart(){
		//Debug.Log("level start!");
		levelHUDPanel.SetActive(true);
	}

	private void OnLevelComplete(){
		levelCompletePopup.SetActive(true);
	}

	private void OnPlayerDead(){
		//gps.SubmitScore(gameDataManager.player.Score);
		gameDataManager.player.Life--;
		//chartboostBridgeManager.ShowInterstitial();
		retryLevelPopup.SetActive(true);
	}
}

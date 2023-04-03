using UnityEngine;
using System.Collections;

public class GeneralHudManager : MonoBehaviour {

	private GameDataManager gameDataManager;
	public UILabel coinLabel;
	public UISprite coinIcon;

	public UILabel totalCoinCaptionLabel;
	public UILabel totalCoinLabel;
	public UISprite totalCoinIcon;

	//public UILabel lifeLabel;
	//public UILabel levelLabel;
	public UILabel scoreLabel;
	//public UILabel timeLabel;
	public UILabel instructionLabel;
	public UILabel messageLabel;

	//private GameTimer gameTimer;
	private GPSArtOfByte gpsArtofByte;

	// Use this for initialization
	void Start (){
		gameDataManager = GameDataManager.GetInstance();
		gpsArtofByte = GPSArtOfByte.GetInstance();

		//gameTimer = GameObject.FindObjectOfType(typeof(GameTimer)) as GameTimer;
		addListener();

		ShowHideScore(0);
		ShowHideCoin(0);
		ShowHideMessage(0);
		UpdateCoinHud();
		OnTotalCoinUpdate();
		UpdateLevel();
		gameDataManager.SetScore(0);
		gameDataManager.SetHp(3);
		gameDataManager.SetCoin(0);

		int life = gameDataManager.player.Life;
		if(life <= 0){
			gameDataManager.player.Life =3;
			//Debug.Log("renew life");
		}
	}

	private void OnDestroy(){
		removeListener();
	}

	private void addListener(){
		if(gameDataManager!=null){
			gameDataManager.player.OnCoinUpdate+=UpdateCoinHud;
			gameDataManager.player.OnTotalCoinUpdate+=OnTotalCoinUpdate;
			gameDataManager.OnLevelStart+=OnLevelStart;
			gameDataManager.player.OnPlayerDead+=OnPlayerDead;
			//gameDataManager.player.OnHpUpdate+=OnHpUpdate;
			//gameDataManager.player.OnPlayerRevive+=OnHpUpdate;
			gameDataManager.player.OnScoreUpdate+=OnScoreUpdate;
			//gameDataManager.player.OnLevelUpdate+=OnLevelUpdate;

			gpsArtofByte.OnGPSConnectionFailed += OnGPSConnectionFailed;
			gpsArtofByte.OnGPSConnectionComplete += OnGPSConnectionComplete;
			gpsArtofByte.OnConnecting+=OnConnecting;
		}
	}

	private void removeListener(){
		if(gameDataManager!=null){
			gameDataManager.player.OnCoinUpdate-=UpdateCoinHud;
			gameDataManager.player.OnTotalCoinUpdate-=OnTotalCoinUpdate;
			gameDataManager.OnLevelStart-=OnLevelStart;
			//gameDataManager.player.OnHpUpdate-=OnHpUpdate;
			gameDataManager.player.OnPlayerDead+=OnPlayerDead;
			//gameDataManager.player.OnPlayerRevive-=OnHpUpdate;
			gameDataManager.player.OnScoreUpdate-=OnScoreUpdate;
			//gameDataManager.player.OnLevelUpdate-=OnLevelUpdate;

			gpsArtofByte.OnGPSConnectionFailed -= OnGPSConnectionFailed;
			gpsArtofByte.OnGPSConnectionComplete -= OnGPSConnectionComplete;
			gpsArtofByte.OnConnecting-=OnConnecting;
		}
	}

	private void OnGPSConnectionFailed(){
		ShowHideMessage(1,"Connection Failed, please \ncheck your internet connection!");
	}

	private void OnGPSConnectionComplete(){
		ShowHideMessage(0);
	}

	private void OnConnecting(){
		ShowHideMessage(1,"Connecting please wait...");
	}

	private void OnPlayerDead(){
		if(gameDataManager!=null){
			ShowHideScore(0);
			ShowHideCoin(0);
		}
	}

	private void OnLevelUpdate(){
		UpdateLevel();
	}

	private void UpdateLevel(){
		//levelLabel.text = ConvertLevelToWorld();
	}

	private string ConvertLevelToWorld(){
		int level = gameDataManager.GetLevel();
		string worldLevel="";

		if(level <10){
			worldLevel = "world 1-" +level;
		}else if(level > 10 && level <20){
			worldLevel = "world 2-" +level;
		}else if(level > 20 && level <30){
			worldLevel = "world 3-" +level;
		}else{
			worldLevel = "world ?-" +level;
		}
		return worldLevel;
	}

	private void OnHpUpdate(){
		//lifeLabel.text = "x " + gameDataManager.player.HP.ToString("000");
	}

	private void OnTotalCoinUpdate(){
		//Debug.Log("OnTotalCoinUpdate total coin: " + gameDataManager.player.TotalCoin);
		totalCoinLabel.text = "x " + gameDataManager.player.TotalCoin.ToString("000");
	}

	private void UpdateCoinHud(){
		//Debug.Log("UpdateCoinHud coin: " + gameDataManager.player.Coin);
		coinLabel.text = "x " + gameDataManager.player.Coin.ToString("000");
	}

	private void OnScoreUpdate(){
		//scoreLabel.text = gameDataManager.player.Score.ToString("0000");
		if(scoreLabel!=null){
			scoreLabel.text = gameDataManager.player.Score.ToString();
		}
	}

	private void OnLevelStart(){
		if(!gameDataManager.player.IsDead){
			ShowHideScore(1);
			ShowHideCoin(1);
			ShowHideInstruction(0);
			ShowHideMessage(0);
		}
	}

	private void ShowHideInstruction(float val){
		if(instructionLabel!=null){
			instructionLabel.alpha = val;
		}
	}

	private void ShowHideScore(float val){
		if(scoreLabel!=null){
			scoreLabel.alpha = val;
		}
	}

	private void ShowHideCoin(float val){
		if(coinLabel!=null){
			coinLabel.alpha = val;
		}

		if(coinIcon!=null){
			coinIcon.alpha = val;
		}

		if(totalCoinCaptionLabel!=null){
			totalCoinCaptionLabel.alpha = val;
		}

		if(totalCoinLabel!=null){
			totalCoinLabel.alpha = val;
		}

		if(totalCoinIcon!=null){
			totalCoinIcon.alpha = val;
		}
	}

	private void ShowHideMessage(float val, string message =""){
		if(messageLabel!=null){
			messageLabel.text= message;
			messageLabel.alpha =val;
		}
	}

	private void Update(){
		//timeLabel.text = gameTimer.timerDisplay;
	}
}

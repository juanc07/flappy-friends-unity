using UnityEngine;
using System.Collections;

public class GameOverUIManager :EventListener {

	public UILabel scoreLabel;
	public UILabel bestScoreLabel;
	private GameDataManager gameDataManager;
	public UILabel messageLabel;
	public UILabel scoreCaptionLabel;
	public UILabel bestCaptionLabel;
	public UILabel newLabel;

	public GameObject fbButton;

	//google play
	private GPSArtOfByte gps;

	public override void Start ()
	{
		base.Start ();
		gameDataManager = GameDataManager.GetInstance();
		gps = GPSArtOfByte.GetInstance();
		ShowHideNewLabel(0);
		UpdateScore();
	}

	public override void OnDestroy ()
	{
		base.OnDestroy ();
	}

	public override void AddEventListener ()
	{
		base.AddEventListener ();
		if(gameDataManager!=null){
			gameDataManager.player.OnHiScoreUpdate+=OnHiScoreUpdate;
			gameDataManager.player.OnLifeUpdate+=OnLifeUpdate;
		}
	}

	public override void RemoveEventListener ()
	{
		base.RemoveEventListener ();
		if(gameDataManager!=null){
			gameDataManager.player.OnHiScoreUpdate-=OnHiScoreUpdate;
			gameDataManager.player.OnLifeUpdate-=OnLifeUpdate;
		}
	}

	private void OnLifeUpdate(){
		if(gameDataManager!=null){
			UpdateScore();
		}
	}

	private void OnHiScoreUpdate(){
		if(gameDataManager!=null){
			bestScoreLabel.text = gameDataManager.player.HiScore.ToString();
		}
	}

	private void ShowHideNewLabel(float val){
		newLabel.alpha = val;
	}

	private void UpdateScore(){
		gameDataManager.player.TotalScore+=gameDataManager.player.Score;
		int life = gameDataManager.player.Life;

		//save gold
		gameDataManager.player.TotalCoin+=gameDataManager.player.Coin;
		gps.SubmitItem(gameDataManager.player.GetBoughtAnimals());
		gps.SubmitGold(gameDataManager.player.TotalCoin);

		if(life > 0){
			fbButton.gameObject.SetActive(false);
			bestCaptionLabel.alpha = 0;
			bestScoreLabel.alpha = 0;
			scoreCaptionLabel.text ="score";
			scoreLabel.text = gameDataManager.player.Score.ToString();
			messageLabel.text = life + " more \nflap left!";
		}else{
			if(gameDataManager.player.HasSetHighScore){
				ShowHideNewLabel(1f);
				gameDataManager.player.HasSetHighScore =false;
			}
			fbButton.gameObject.SetActive(true);
			bestCaptionLabel.alpha = 1f;
			bestScoreLabel.alpha = 1f;
			scoreCaptionLabel.text ="total score";
			scoreLabel.text = gameDataManager.player.TotalScore.ToString();
			messageLabel.text = "Game Over";
			bestScoreLabel.text = gameDataManager.player.HiScore.ToString();
			gps.SubmitScore(gameDataManager.player.TotalScore);
		}
	}
}

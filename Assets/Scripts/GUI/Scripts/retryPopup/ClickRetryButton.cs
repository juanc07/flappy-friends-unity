using UnityEngine;
using System.Collections;

public class ClickRetryButton : MonoBehaviour {

	private GameDataManager gameDataManager;
	private ScenePreloader scenePreloader;

	// Use this for initialization
	void Start () {
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
		gameDataManager = GameDataManager.GetInstance();
	}
	
	private void OnClick(){
		//string urlString = "market://details?id=" + "com.gigadrillgames.slappybird";
		//Application.OpenURL(urlString);

		if(gameDataManager.player.IsDead){
			gameDataManager.ResetLevel();
			this.gameObject.SetActive(false);
			gameDataManager.player.IsDead = false;

			if(gameDataManager.player.Life <= 0){
				gameDataManager.player.TotalScore = 0;
			}
		}
		scenePreloader.LoadScene(ScenePreloader.Scenes.Game);
	}
}

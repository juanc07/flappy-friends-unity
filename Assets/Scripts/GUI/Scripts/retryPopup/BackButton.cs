using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {
	private ScenePreloader scenePreloader;
	private GameDataManager gameDataManager;

	// Use this for initialization
	void Start () {
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
		gameDataManager = GameDataManager.GetInstance();
	}
	
	private void OnClick(){
		gameDataManager.ResetLevel();			
		gameDataManager.player.IsDead = false;
		gameDataManager.player.Life = 3;
		gameDataManager.player.TotalScore = 0;
		scenePreloader.LoadScene(ScenePreloader.Scenes.Shop);
		//scenePreloader.LoadScene(ScenePreloader.Scenes.Title);
	}
}

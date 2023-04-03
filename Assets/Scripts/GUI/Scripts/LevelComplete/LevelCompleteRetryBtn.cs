using UnityEngine;
using System.Collections;

public class LevelCompleteRetryBtn : MonoBehaviour {

	private ScenePreloader scenePreloader;
	private GameDataManager gameDataManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
	}
	
	private void OnClick(){
		gameDataManager.ResetLevel();
		scenePreloader.LoadScene(ScenePreloader.Scenes.Game);
	}
}

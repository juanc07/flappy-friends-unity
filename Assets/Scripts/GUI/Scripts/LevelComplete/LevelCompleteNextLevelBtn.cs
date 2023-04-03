using UnityEngine;
using System.Collections;

public class LevelCompleteNextLevelBtn : MonoBehaviour {
	
	private ScenePreloader scenePreloader;
	private GameDataManager gameDataManager;
	
	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
	}
	
	private void OnClick(){
		gameDataManager.ResetLevel();
		gameDataManager.UpdateLevel();
		scenePreloader.LoadScene(ScenePreloader.Scenes.Game);
	}
}

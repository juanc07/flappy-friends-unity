using UnityEngine;
using System.Collections;

public class NavigationButtonClick : MonoBehaviour {
	
	private ScenePreloader scenePreloader;
	//private ChartboostBridgeManager chartboostBridgeManager;
	public ScenePreloader.Scenes sceneToShow = ScenePreloader.Scenes.Game;
	
	void Start () {
		//chartboostBridgeManager = ChartboostBridgeManager.GetInstance();
		//chartboostBridgeManager.CacheInterstitial();
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
	}
	
	private void OnClick(){
		scenePreloader.LoadScene(sceneToShow);
	}
}

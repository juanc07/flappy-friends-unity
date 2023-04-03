using UnityEngine;
using System.Collections;

public class PlayButtonClick : MonoBehaviour {

	private ScenePreloader scenePreloader;
	public ScenePreloader.Scenes sceneToShow = ScenePreloader.Scenes.Game;

	void Start () {
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
	}

	private void OnClick(){
		//scenePreloader.LoadScene(ScenePreloader.Scenes.Game);
		scenePreloader.LoadScene(sceneToShow);
	}
}

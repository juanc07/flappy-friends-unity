using UnityEngine;
using System.Collections;

public class TitlePlayButton : MonoBehaviour {

	private ScenePreloader scenePreloader;

	// Use this for initialization
	void Start () {
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
	}
	
	private void OnClick(){
		scenePreloader.LoadScene(ScenePreloader.Scenes.Game);
	}
}

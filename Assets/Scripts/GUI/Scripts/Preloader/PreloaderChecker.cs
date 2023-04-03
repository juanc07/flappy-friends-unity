using UnityEngine;
using System.Collections;

public class PreloaderChecker : MonoBehaviour {
	private ScenePreloader scenePreloader;
	public GameObject preloaderUIPrefab;
	private AdmobManager admobManager;

	// Use this for initialization
	void Start (){
		if(Application.platform == RuntimePlatform.Android){
			#if UNITY_ANDROID || UNITY_IPHONE
				admobManager = AdmobManager.GetInstance();
				admobManager.ShowBanner();
			#endif
		}

		QualitySettings.SetQualityLevel(1);
		scenePreloader  = GameObject.FindObjectOfType<ScenePreloader>();
		if(scenePreloader==null){
			GameObject preloaderUI=  Instantiate(preloaderUIPrefab) as GameObject;
			preloaderUI.name = "PreloaderUI";
			scenePreloader = preloaderUI.GetComponent<ScenePreloader>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

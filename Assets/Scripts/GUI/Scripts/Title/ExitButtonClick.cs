using UnityEngine;
using System.Collections;

public class ExitButtonClick : MonoBehaviour {
	#if UNITY_ANDROID
	private FlurryAnalyticsManager flurryManager;

	// Use this for initialization
	void Start () {
		if(Application.platform == RuntimePlatform.Android){
			flurryManager = FlurryAnalyticsManager.GetInstance();
		}
	}
	
	private void OnClick(){
		if(Application.platform == RuntimePlatform.Android){
			flurryManager.EndFlurry();
		}
		Application.Quit();
	}
	#endif
}


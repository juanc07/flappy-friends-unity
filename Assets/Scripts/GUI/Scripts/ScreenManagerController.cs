using UnityEngine;
using System.Collections;


public class ScreenManagerController : MonoBehaviour {
	#if UNITY_ANDROID 
	private FlurryAnalyticsManager flurryManager;
	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;
		if(Application.platform == RuntimePlatform.Android){
			flurryManager = FlurryAnalyticsManager.GetInstance();
			flurryManager.SetDebug(0);
			//bird
			//flurryManager.StartFlurry("RXV2295S37XDMRT4XWN5");
			//cow
			//flurryManager.StartFlurry("55KS9X92J7Y4JN4SF5G2");

			//new slappy bird 3d
			//flurryManager.StartFlurry("P5JJW2GY4XGYGH7BMCHG");
			flurryManager.StartFlurry("JYGGVNGH9XXJ943DQGHT");
		}
	}
	#endif
}

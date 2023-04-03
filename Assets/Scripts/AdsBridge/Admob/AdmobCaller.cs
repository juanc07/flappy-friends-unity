using UnityEngine;
using System.Collections;

public class AdmobCaller : MonoBehaviour {

	private AdmobManager admobManager;

	#if UNITY_ANDROID || UNITY_IPHONE
	// Use this for initialization
	void Start () {
		admobManager = AdmobManager.GetInstance();
		admobManager.ShowBanner();
	}
	#endif
}

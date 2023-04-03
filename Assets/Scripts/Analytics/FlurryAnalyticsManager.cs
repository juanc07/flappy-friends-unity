using UnityEngine;
using System.Collections;
using System;
#if UNITY_ANDROID
public class FlurryAnalyticsManager : MonoBehaviour {

	private static FlurryAnalyticsManager instance;
	private static GameObject container;

	private static AndroidJavaObject jo;
	private bool isStarted =false;
	private bool isDebug = true;
	
	//check for ios
	//private bool paused;

	public static FlurryAnalyticsManager GetInstance(){
		if(instance==null){
			#if UNITY_ANDROID
			if(Application.platform == RuntimePlatform.Android){			
				jo = new AndroidJavaObject("com.gigadrillgames.flurryplugin.FlurryPlugin");
			}
			#endif
			container = new GameObject();
			container.name = "FlurryAnalyticsManager";
			instance = container.AddComponent(typeof(FlurryAnalyticsManager)) as FlurryAnalyticsManager;
			DontDestroyOnLoad(instance.gameObject);
		}

		return instance;
	}

	public void StartFlurry(string flurryid){
		if(isStarted) return;

		if(flurryid.Equals("",StringComparison.Ordinal)){		
			Message("Warning please input your flurry API Key");
			return;
		}
		
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("StartFlurry",flurryid);
			isStarted =true;
			//Debug.Log("Started flurry");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	public void EndFlurry(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){	
			jo.CallStatic("EndFlurry");
			//Debug.Log(" EndFlurry called!");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	public void RecordPageView(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("RecordPageView");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	public void RecordUserId(string userid){
		if(userid.Equals("",StringComparison.Ordinal)){		
			Message("Warning please input your userid");
			return;
		}
		
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("RecordUserId",userid);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}


	public void RecordAge(int age){
		if(age==0){		
			Message("Warning please input age");
			return;
		}
		
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("RecordAge",age);
		}
		#endif
	}
	
	public void RecordGender(byte gender){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("RecordGender",gender);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	public void SetDebug(int debug){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("SetDebug",debug);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	public void ShowMessage(string message){
		if(Application.platform == RuntimePlatform.Android){
			jo.Call("ShowMessage", message);
		}else{
			Message("warning: must run in actual android device");
		}
	}

	/*void Update() {
		if (Input.GetKey("escape")){
			Message("press back to quit!");
			EndFlurry();
			Application.Quit();
		}	
	}*/

	/*void OnApplicationQuit() {
		EndFlurry();
		Debug.Log(" OnApplicationQuit call EndFlurry!");
	}*/

	/*
	void OnApplicationPause(bool pauseStatus) {
		//paused = pauseStatus;
		//Message("On ApplicationPause!!");
		//EndFlurry();
	}*/

	private void Message(string message){
		Debug.Log(message);
	}
}

#endif

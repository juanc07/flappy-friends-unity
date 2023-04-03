using UnityEngine;
using System.Collections;
using System;


#if UNITY_ANDROID

public class FlurryPluginCaller : MonoBehaviour {

	private static FlurryPluginCaller instance;
	private static AndroidJavaObject jo;
	public string apiKey = "";
	private bool isDebug = true;

	//check for ios
	private bool paused;

	//presentation vars
	private string message="";
	private Rect windowRect = new Rect(300, 180, 210, 150);
	private GUIStyle titleStyle = new GUIStyle();
	//presentation vars

	private void Start(){
		titleStyle.fontSize = 20;
		titleStyle.fontStyle = FontStyle.Bold;
	}

	private void Awake(){
		if(instance==null){
			instance = this;
		}
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo = new AndroidJavaObject("com.gigadrillgames.flurryplugin.FlurryPlugin");
		}
		#endif
	}

	public void StartFlurry(string flurryid){
		if(apiKey.Equals("",StringComparison.Ordinal)){		
			Message("Warning please input your flurry API Key");
			message="Warning please input your flurry API Key";
			return;
		}

		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("StartFlurry",flurryid);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	private void DoMyWindow(int windowID) {
		GUI.Label(new Rect(5, 30, 200, 50), "Message: " + message );
		//GUI.DragWindow(new Rect(0, 0, 10000, 10000));
	}

	public void EndFlurry(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){	
			jo.CallStatic("EndFlurry");
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
			message="Warning please input your userid";
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
			message="Warning please input age";
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

	private void Message(string message){
		Debug.Log(message);
	}

	void OnApplicationQuit() {
		EndFlurry();
	}

	void OnApplicationPause(bool pauseStatus) {
		paused = pauseStatus;
	}

	private void OnGUI(){
		windowRect = GUI.Window(0, windowRect, DoMyWindow, "Message:");

		GUI.Label(new Rect(30, 10, 200, 20), "Flurry Analytic Plugin Demo",titleStyle);

		if (GUI.Button(new Rect(300, 40, 200, 50), "is show Debug")){
			if(isDebug){
				isDebug =false;
				SetDebug(0);
			}else{
				isDebug = true;
				SetDebug(1);
			}
		}

		if (GUI.Button(new Rect(30, 40, 200, 50), "Start Session")){
			StartFlurry(apiKey);
		}

		if (GUI.Button(new Rect(30, 100, 200, 50), "Record Page View")){
			RecordPageView();
		}

		if (GUI.Button(new Rect(30, 160, 200, 50), "Record Userid")){
			RecordUserId("testUserid");
		}

		if (GUI.Button(new Rect(30, 220, 200, 50), "Record age")){
			RecordAge(99);
		}

		if (GUI.Button(new Rect(30, 280, 200, 50), "Record gender")){
			RecordGender(1);
		}

		if (GUI.Button(new Rect(300, 100, 200, 50), "End Session")){
			EndFlurry();
		}
	}
}
#endif

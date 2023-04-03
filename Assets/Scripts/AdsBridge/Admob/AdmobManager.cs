using UnityEngine;
using System.Collections;
using System;

public class AdmobManager : MonoBehaviour {
	#if UNITY_ANDROID || UNITY_IPHONE
	//private bool isAdActive=false;
	private int showAdCount=0;
	private int adRefreshRate=10;

	private bool isBannerCreated =false;
	private bool hasShownAd =false;
	private bool isLimitToOneAd =true;
	//admob plugin area
	// Defines string values for supported ad sizes.
	public class AdSize
	{
		private string adSize;
		private AdSize(string value)
		{
			this.adSize = value;
		}
		
		public override string ToString()
		{
			return adSize;
		}
		
		public static AdSize Banner = new AdSize("BANNER");
		public static AdSize MediumRectangle = new AdSize("IAB_MRECT");
		public static AdSize IABBanner = new AdSize("IAB_BANNER");
		public static AdSize Leaderboard = new AdSize("IAB_LEADERBOARD");
		public static AdSize SmartBanner = new AdSize("SMART_BANNER");
	}


	// These are the ad callback events that can be hooked into.
	public static event Action ReceivedAd = delegate {};
	public static event Action<string> FailedToReceiveAd = delegate {};
	public static event Action ShowingOverlay = delegate {};
	public static event Action DismissedOverlay = delegate {};
	public static event Action LeavingApplication = delegate {};


	//admob plugin area




	private static AdmobManager instance;
	private static GameObject container;

	public static AdmobManager GetInstance(){
		if(instance == null){
			container = new GameObject();
			container.name = "AdmobManager";
			instance = container.AddComponent(typeof(AdmobManager)) as AdmobManager;
			#if UNITY_ANDROID || UNITY_IPHONE				
				SetCallbackHandlerName(container.name);				
			#endif
			DontDestroyOnLoad(instance.gameObject);
		}

		return instance;
	}


	//plugin area

	// Create a banner view and add it into the view hierarchy.
	public static void CreateBannerView(string publisherId, AdSize adSize, bool positionAtTop)
	{
		AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
		pluginClass.CallStatic("createBannerView",
		                       new object[4] {activity, publisherId, adSize.ToString(), positionAtTop});
	}
	
	// Request a new ad for the banner view without any extras.
	public static void RequestBannerAd(bool isTesting)
	{
		AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
		pluginClass.CallStatic("requestBannerAd", new object[1] {isTesting});
	}
	
	// Request a new ad for the banner view with extras.
	public static void RequestBannerAd(bool isTesting, string extras)
	{
		AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
		pluginClass.CallStatic("requestBannerAd", new object[2] {isTesting, extras});
	}

	#if UNITY_ANDROID || UNITY_IPHONE
	// Set the name of the callback handler so the right component gets ad callbacks.
	public static void SetCallbackHandlerName(string callbackHandlerName)
	{
		AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
		pluginClass.CallStatic("setCallbackHandlerName", new object[1] {callbackHandlerName});
	}
	#endif
	// Hide the banner view from the screen.
	public static void HideBannerView()
	{
		AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
		pluginClass.CallStatic("hideBannerView");
	}
	
	// Show the banner view on the screen.
	public static void ShowBannerView(){
		AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
		pluginClass.CallStatic("showBannerView");
	}
	
	public void OnReceiveAd(string unusedMessage)
	{
		ReceivedAd();
		hasShownAd =true;
	}
	
	public void OnFailedToReceiveAd(string message)
	{
		FailedToReceiveAd(message);
	}
	
	public void OnPresentScreen(string unusedMessage)
	{
		ShowingOverlay();
	}
	
	public void OnDismissScreen(string unusedMessage)
	{
		DismissedOverlay();
	}
	
	public void OnLeaveApplication(string unusedMessage)
	{
		LeavingApplication();
	}
	//plugin area


	public void ShowBanner(){
		if(isLimitToOneAd && hasShownAd) return;		
		if(showAdCount==0 || (showAdCount % adRefreshRate) == 0){
			CreateBanner();
			RequestBannerAd(true);
		}
		showAdCount++;
	}

	private void CreateBanner(){
		if(!isBannerCreated){
			isBannerCreated =true;
			CreateBannerView("a152fadab5ab3eb",AdSize.Banner,true);
		}
	}
	
	void OnEnable()
	{
		//print("Registering for AdMob Events");
		ReceivedAd += HandleReceivedAd;
		FailedToReceiveAd += HandleFailedToReceiveAd;
		ShowingOverlay += HandleShowingOverlay;
		DismissedOverlay += HandleDismissedOverlay;
		LeavingApplication += HandleLeavingApplication;
	}
	
	void OnDisable()
	{
		//print("Unregistering for AdMob Events");
		ReceivedAd -= HandleReceivedAd;
		FailedToReceiveAd -= HandleFailedToReceiveAd;
		ShowingOverlay -= HandleShowingOverlay;
		DismissedOverlay -= HandleDismissedOverlay;
		LeavingApplication -= HandleLeavingApplication;
	}

	public void HandleReceivedAd()
	{
		//print("HandleReceivedAd event received");
		//isAdActive =true;
	}
	
	public void HandleFailedToReceiveAd(string message)
	{
		//print("HandleFailedToReceiveAd event received with message:");
		//isAdActive =false;
		//print(message);
	}
	
	public void HandleShowingOverlay()
	{
		//print("HandleShowingOverlay event received");
	}
	
	public void HandleDismissedOverlay()
	{
		//print("HandleDismissedOverlay event received");
	}
	
	public void HandleLeavingApplication()
	{
		//print("HandleLeavingApplication event received");
	}
	#endif
}

using UnityEngine;
using System.Collections;
using System;
using Chartboost;


public class ChartboostBridgeManager : MonoBehaviour
{
	private static GameObject container;
	private static ChartboostBridgeManager instance;

	private int adShowRate=5;
	private int showAdCount=1;

	public static ChartboostBridgeManager GetInstance(){
		if(instance == null){
			container = new GameObject();
			container.name = "ChartboostBridgeManager";
			instance = container.AddComponent(typeof(ChartboostBridgeManager)) as ChartboostBridgeManager;
			DontDestroyOnLoad(instance.gameObject);
		}
		return instance;
	}

	#if UNITY_ANDROID || UNITY_IPHONE
	
	public void Update()
	{
		#if UNITY_ANDROID
		// Handle the Android back button
		if (Input.GetKeyUp(KeyCode.Escape)) {
			// Check if Chartboost wants to respond to it
			if (CBBinding.onBackPressed()) {
				// If so, return and ignore it
				return;
			} else {
				// Otherwise, handle it ourselves -- let's close the app
				Application.Quit();
			}
		}
		#endif
	}
	
	void OnEnable()
	{
		// Initialize the Chartboost plugin
		#if UNITY_ANDROID
		// Remember to set the Android app ID and signature in the file `/Plugins/Android/res/values/strings.xml`
		CBBinding.init();
		#elif UNITY_IPHONE
		// Replace these with your own app ID and signature from the Chartboost web portal
		CBBinding.init( "52f70b2d2d42da3a44ed81b3", "76c2c70dc220cf48de09339f7db76026bc275e77" );
		#endif
		AddChartboostEventListener();
	}
	
	
	void OnDisable(){
		RemoveChartboostEventListener();
	}

	private void AddChartboostEventListener(){
		// Listen to all impression-related events
		CBManager.didFailToLoadInterstitialEvent += didFailToLoadInterstitialEvent;
		CBManager.didDismissInterstitialEvent += didDismissInterstitialEvent;
		CBManager.didCloseInterstitialEvent += didCloseInterstitialEvent;
		CBManager.didClickInterstitialEvent += didClickInterstitialEvent;
		CBManager.didCacheInterstitialEvent += didCacheInterstitialEvent;
		CBManager.didShowInterstitialEvent += didShowInterstitialEvent;
		CBManager.didFailToLoadMoreAppsEvent += didFailToLoadMoreAppsEvent;
		CBManager.didDismissMoreAppsEvent += didDismissMoreAppsEvent;
		CBManager.didCloseMoreAppsEvent += didCloseMoreAppsEvent;
		CBManager.didClickMoreAppsEvent += didClickMoreAppsEvent;
		CBManager.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
		CBManager.didShowMoreAppsEvent += didShowMoreAppsEvent;
	}

	private void RemoveChartboostEventListener(){
		CBManager.didFailToLoadInterstitialEvent -= didFailToLoadInterstitialEvent;
		CBManager.didDismissInterstitialEvent -= didDismissInterstitialEvent;
		CBManager.didCloseInterstitialEvent -= didCloseInterstitialEvent;
		CBManager.didClickInterstitialEvent -= didClickInterstitialEvent;
		CBManager.didCacheInterstitialEvent -= didCacheInterstitialEvent;
		CBManager.didShowInterstitialEvent -= didShowInterstitialEvent;
		CBManager.didFailToLoadMoreAppsEvent -= didFailToLoadMoreAppsEvent;
		CBManager.didDismissMoreAppsEvent -= didDismissMoreAppsEvent;
		CBManager.didCloseMoreAppsEvent -= didCloseMoreAppsEvent;
		CBManager.didClickMoreAppsEvent -= didClickMoreAppsEvent;
		CBManager.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
		CBManager.didShowMoreAppsEvent -= didShowMoreAppsEvent;
	}

	public void ShowInterstitial(){
		int checker = showAdCount%adShowRate;
		if(checker==0 && showAdCount> 1){
			CBBinding.showInterstitial( "default" );
			//Debug.Log("show chart boost ads");
		}
		showAdCount++;
	}

	public void CacheInterstitial(){
		CBBinding.cacheInterstitial( "default" );
	}

	public void CacheMoreApps(){
		CBBinding.cacheMoreApps();
	}

	public void ShowMoreApps(){
		CBBinding.showMoreApps();
	}

	//events for chartboost
	void didFailToLoadInterstitialEvent( string location )
	{
		Debug.Log( "didFailToLoadInterstitialEvent: " + location );
	}
	
	void didDismissInterstitialEvent( string location )
	{
		Debug.Log( "didDismissInterstitialEvent: " + location );
	}
	
	void didCloseInterstitialEvent( string location )
	{
		Debug.Log( "didCloseInterstitialEvent: " + location );
	}
	
	void didClickInterstitialEvent( string location )
	{
		Debug.Log( "didClickInterstitialEvent: " + location );
	}
	
	void didCacheInterstitialEvent( string location )
	{
		Debug.Log( "didCacheInterstitialEvent: " + location );
	}
	
	void didShowInterstitialEvent( string location )
	{
		Debug.Log( "didShowInterstitialEvent: " + location );
	}
	
	void didFailToLoadMoreAppsEvent()
	{
		Debug.Log( "didFailToLoadMoreAppsEvent" );
	}
	
	void didDismissMoreAppsEvent()
	{
		Debug.Log( "didDismissMoreAppsEvent" );
	}
	
	void didCloseMoreAppsEvent()
	{
		Debug.Log( "didCloseMoreAppsEvent" );
	}
	
	void didClickMoreAppsEvent()
	{
		Debug.Log( "didClickMoreAppsEvent" );
	}
	
	void didCacheMoreAppsEvent()
	{
		Debug.Log( "didCacheMoreAppsEvent" );
	}
	
	void didShowMoreAppsEvent()
	{
		Debug.Log( "didShowMoreAppsEvent" );
	}

	#endif
}

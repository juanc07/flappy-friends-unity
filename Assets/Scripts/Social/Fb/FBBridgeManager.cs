using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FBBridgeManager : MonoBehaviour
{
	private GameDataManager gameDataManager;
	private static FBBridgeManager instance;
	private static GameObject container;
	public bool isLogin =false;

	private Action FaceBookInit;
	public event Action OnFaceBookInit{
		add{FaceBookInit+=value;}
		remove{FaceBookInit-=value;}
	}

	private Action FaceBookLogin;
	public event Action OnFaceBookLogin{
		add{FaceBookLogin+=value;}
		remove{FaceBookLogin-=value;}
	}

	public static FBBridgeManager GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name = "FBBridgeManager";
			instance = container.AddComponent(typeof(FBBridgeManager)) as FBBridgeManager;
			DontDestroyOnLoad(instance.gameObject);
		}

		return instance;
	}

	#region FB.Init() example
	
	public bool isInit = false;
	
	private void CallFBInit()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}
	
	private void OnInitComplete()
	{
		if(!isInit){
			//Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
			isInit = true;
			gameDataManager = GameDataManager.GetInstance();
			if(null!=FaceBookInit){
				FaceBookInit();
			}
		}
	}
	
	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}
	
	#endregion
	
	#region FB.Login() example
	
	private void CallFBLogin()
	{
		FB.Login("email,publish_actions", LoginCallback);
	}
	
	void LoginCallback(FBResult result)
	{
		if (result.Error != null)
			lastResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn) {
			lastResponse = "Login cancelled by Player";
		}else{
			if(!isLogin){
				if(null!=FaceBookLogin){
					isLogin =true;
					FaceBookLogin();
				}
			}
		}
	}
	
	private void CallFBLogout()
	{
		FB.Logout();
	}
	#endregion
	
	#region FB.PublishInstall() example
	
	private void CallFBPublishInstall()
	{
		FB.PublishInstall(PublishComplete);
	}
	
	private void PublishComplete(FBResult result)
	{
		Debug.Log("publish response: " + result.Text);
	}
	
	#endregion
	
	#region FB.AppRequest() Friend Selector
	
	private string FriendSelectorTitle = "Flappy Friends";
	private string FriendSelectorMessage = "Hey, Play Flappy Friends game";
	private string FriendSelectorFilters = "[\"all\",\"app_users\",\"app_non_users\"]";
	private string FriendSelectorData = "{}";
	private string FriendSelectorExcludeIds = "";
	private string FriendSelectorMax = "40";
	
	private void CallAppRequestAsFriendSelector()
	{
		// If there's a Max Recipients specified, include it
		int? maxRecipients = null;
		if (FriendSelectorMax != "")
		{
			try
			{
				maxRecipients = Int32.Parse(FriendSelectorMax);
			}
			catch (Exception e)
			{
				status = e.Message;
			}
		}
		
		// include the exclude ids
		string[] excludeIds = (FriendSelectorExcludeIds == "") ? null : FriendSelectorExcludeIds.Split(',');
		
		FB.AppRequest(
			message: FriendSelectorMessage,
			filters: FriendSelectorFilters,
			excludeIds: excludeIds,
			maxRecipients: maxRecipients,
			data: FriendSelectorData,
			title: FriendSelectorTitle,
			callback: Callback
			);
	}
	#endregion
	
	#region FB.AppRequest() Direct Request
	
	public string DirectRequestTitle = "";
	public string DirectRequestMessage = "Herp";
	private string DirectRequestTo = "";
	
	private void CallAppRequestAsDirectRequest()
	{
		if (DirectRequestTo == "")
		{
			throw new ArgumentException("\"To Comma Ids\" must be specificed", "to");
		}
		FB.AppRequest(
			message: DirectRequestMessage,
			to: DirectRequestTo.Split(','),
			title: DirectRequestTitle,
			callback: Callback
			);
	}
	
	#endregion
	
	#region FB.Feed() example
	
	private string FeedToId = "yeah";
	private string FeedLink = "yeah";
	private string FeedLinkName = "yeah";
	private string FeedLinkCaption = "yeah";
	private string FeedLinkDescription = "yeah";
	private string FeedPicture = "yeah";
	private string FeedMediaSource = "yeah";
	private string FeedActionName = "yeah";
	private string FeedActionLink = "yeah";
	private string FeedReference = "yeah";
	private bool IncludeFeedProperties = false;
	private Dictionary<string, string[]> FeedProperties = new Dictionary<string, string[]>();
	
	private void CallFBFeed()
	{
		Dictionary<string, string[]> feedProperties = null;
		if (IncludeFeedProperties)
		{
			feedProperties = FeedProperties;
		}

		Debug.Log( " check call fb feed  FeedLinkDescription " + FeedLinkDescription );

		FB.Feed(
			toId: FeedToId,
			link: FeedLink,
			linkName: FeedLinkName,
			linkCaption: FeedLinkCaption,
			linkDescription: FeedLinkDescription,
			picture: FeedPicture,
			mediaSource: FeedMediaSource,
			actionName: FeedActionName,
			actionLink: FeedActionLink,
			reference: FeedReference,
			properties: feedProperties,
			callback: Callback
			);


		/*FB.Feed(
			toId: FeedToId,
			link: FeedLink,
			linkName: FeedLinkName,
			linkCaption: FeedLinkCaption,
			linkDescription: FeedLinkDescription,
			picture: FeedPicture,
			mediaSource: FeedMediaSource,
			actionName: FeedActionName,
			actionLink: FeedActionLink,
			reference: FeedReference,
			properties: feedProperties,
			callback: Callback
			);*/
			
	}
	
	#endregion
	
	#region FB.Canvas.Pay() example
	
	public string PayProduct = "";
	
	private void CallFBPay()
	{
		FB.Canvas.Pay(PayProduct);
	}
	
	#endregion
	
	#region FB.API() example
	
	public string ApiQuery = "";
	
	private void CallFBAPI()
	{
		FB.API(ApiQuery, Facebook.HttpMethod.GET, Callback);
	}
	
	#endregion
	
	#region FB.GetDeepLink() example
	
	private void CallFBGetDeepLink()
	{
		FB.GetDeepLink(Callback);
	}
	
	#endregion
	
	#region FB.AppEvent.LogEvent example
	
	public float PlayerLevel = 1.0f;
	
	public void CallAppEventLogEvent()
	{
		var parameters = new Dictionary<string, object>();
		parameters[Facebook.FBAppEventParameterName.Level] = "Player Level";
		FB.AppEvents.LogEvent(Facebook.FBAppEventName.AchievedLevel, PlayerLevel, parameters);
		PlayerLevel++;
	}
	
	#endregion
	
	#region GUI
	
	private string status = "Ready";
	
	private string lastResponse = "";
	private Texture2D lastResponseTexture;
	
	void Awake(){		
		FeedProperties.Add("key1", new[] { "valueString1" });
		FeedProperties.Add("key2", new[] { "valueString2", "http://www.facebook.com" });
	}

	#region FB API
	public void Init(){
		CallFBInit();
		status = "FB.Init() called with " + FB.AppId;
		Message(status);
	}

	public void CallAPI(){
		status = "API called";
		CallFBAPI();
		Message(status);
	}

	public void Login(){
		CallFBLogin();
		status = "Login called";
		Message(status);
	}

	public void Logout(){
		CallFBLogout();
		status = "Logout called";
		Message(status);
	}

	public void PublishInstall(){
		if(!isLogin) return;
		CallFBPublishInstall();
		status = "Install Published";
		Message(status);
	}

	public void InviteFriends(){
		if(!isLogin) return;
		try
		{
			CallAppRequestAsFriendSelector();
			status = "Friend Selector called";
			Message(status);
		}
		catch (Exception e)
		{
			status = e.Message;
			Message(status);
		}
	}

	public void DirectInvite(){
		if(!isLogin) return;
		try
		{
			CallAppRequestAsDirectRequest();
			status = "Direct Request called";
			Message(status);
		}
		catch (Exception e)
		{
			status = e.Message;
			Message(status);
		}
	}

	public void DialogFeed(){
		if(!isLogin) return;
		try
		{
			FeedToId = "";
			FeedLink = "https://www.facebook.com/fluffyfriendship";
			FeedLinkName = "Fluffy Friends";
			FeedLinkCaption = "Beat my Score on Fluffy Friends game";
			FeedLinkDescription = "Awesome!, I got a score of " + gameDataManager.player.TotalScore.ToString();
			FeedPicture = "";
			FeedMediaSource = "";
			FeedActionName = "Play Fluffy Friends on FaceBook";
			FeedActionLink = "https://apps.facebook.com/fluffyfriends/";


			CallFBFeed();
			status = "Feed dialog called";
			Message(status);
		}
		catch (Exception e)
		{
			status = e.Message;
			Message(status);
		}
	}


	public void ScreenShot(){
		if(!isLogin) return;
		status = "Take screenshot";		
		StartCoroutine(TakeScreenshot());
		Message(status);
	}

	public void FbDeepLink(){
		if(!isLogin) return;
		CallFBGetDeepLink();
		Message("Deep link fb");
	}

	public void FBLogEvent(){
		if(!isLogin) return;
		status = "Logged FB.AppEvent";
		CallAppEventLogEvent();
		Message(status);
	}

	private void Message(string message){
		//Debug.Log( "fb status " + message );
	}
	#endregion
	
	void Callback(FBResult result)
	{
		lastResponseTexture = null;
		if (result.Error != null)
			lastResponse = "Error Response:\n" + result.Error;
		else if (!ApiQuery.Contains("/picture"))
			lastResponse = "Success Response:\n" + result.Text;
		else
		{
			lastResponseTexture = result.Texture;
			lastResponse = "Success Response:\n";
		}
	}
	
	private IEnumerator TakeScreenshot() 
	{
		yield return new WaitForEndOfFrame();
		
		var width = Screen.width;
		var height = Screen.height;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();
		
		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "FlappyFriends.png");
		wwwForm.AddField("message", "Hi guys!,  look at my Flappy friends stunt");
		
		FB.API("me/photos", Facebook.HttpMethod.POST, Callback, wwwForm);
	}	
	#endregion
}

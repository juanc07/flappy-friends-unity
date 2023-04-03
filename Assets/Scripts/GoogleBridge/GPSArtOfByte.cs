using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class GPSArtOfByte : MonoBehaviour {
	
	public string ScoreLeaderboardId{set;get;}
	public string connectionStatus = "Not Connected";
	
	private static GameObject container;
	private static GPSArtOfByte instance;
	
	public enum ConnectionState{
		Connected			 = 0,
		ConnectionFailed	 = 1,
		Disconnected		 = 2
	}
	public ConnectionState connectionState = ConnectionState.Disconnected;
	
	private long cacheScore = 0;
	private long cacheHiScore = 0;
	private long cacheGold = 0;
	private List<int> cacheItems = new List<int>();	

	private bool isFirstLoad = false;
	private bool isItemLoaded =false;
	
	private Action Connecting;
	public event Action OnConnecting{
		add{Connecting+=value;}
		remove{Connecting-=value;}
	}
	
	private Action GPSConnectionComplete;
	public event Action OnGPSConnectionComplete{
		add{GPSConnectionComplete+=value;}
		remove{GPSConnectionComplete-=value;}
	}
	
	private Action GPSConnectionFailed;
	public event Action OnGPSConnectionFailed{
		add{GPSConnectionFailed+=value;}
		remove{GPSConnectionFailed-=value;}
	}
	
	private Action GPSDisconnect;
	public event Action OnGPSDisconnect{
		add{GPSDisconnect+=value;}
		remove{GPSDisconnect-=value;}
	}
	
	
	private GameDataManager gameDataManager;
	//private FlurryAnalyticsManager flurryAnalyticManager;
	
	public static GPSArtOfByte GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name = "GPSArtOfByte";
			instance = container.AddComponent(typeof(GPSArtOfByte)) as GPSArtOfByte;
			DontDestroyOnLoad(instance.gameObject);
		}
		
		return instance;
	}
	
	
	protected void InitConnectionCallbacks()
	{
		Ugs.Client.OnConnected += () =>
		{
			connectionState = ConnectionState.Connected;
			connectionStatus = "Connected";

			LoadScore();
			SubmitScore(cacheScore);
			SaveItems();

			if(null!=GPSConnectionComplete){
				GPSConnectionComplete();
			}
		};
		
		Ugs.Client.OnConnectionFailed += () =>
		{
			//if(connectionState == ConnectionState.Disconnected){
				connectionState = ConnectionState.ConnectionFailed;
				if(null!=GPSConnectionFailed){
					GPSConnectionFailed();
				}
			//}
			
			var result = Ugs.Client.ConnectionResult;
			connectionStatus = "ConnectionFailed: " + result + " (" + result.ErrorCode() + ")";
			connectionStatus += "\n" + Ugs.Client.ConnectionResult.Details();
			//Debug.LogWarning("Failed to connect to Google Play Games.");
		};
		
		Ugs.Client.OnDisconnected += () =>
		{
			connectionState = ConnectionState.Disconnected;
			connectionStatus = "Disconnected";
			
			if(null!=GPSDisconnect){
				GPSDisconnect();
			}
			//Debug.Log("Disconnected from Google Play Games");
		};
		
		CloudSaveEvents();
	}
	
	private void CloudSaveEvents(){
		Ugs.CloudSave.OnStateSaved += (Ugs.State ugsState) =>
		{
			UpdateScore();
			//Debug.Log("OnStateSaved from Google Cloud Save");
		};
		
		Ugs.CloudSave.OnStateSavingFailed += (int val) =>
		{
			//Debug.Log("OnStateSavingFailed from Google Cloud Save");
		};
		
		Ugs.CloudSave.OnStateDeleted += (int val ) =>
		{
			//Debug.Log("OnStateDeleted from Google Cloud Save");
		};
		
		Ugs.CloudSave.OnStateDeletionFailed += (int val) =>
		{
			//Debug.Log("OnStateDeletionFailed from Google Cloud Save");
		};
		
		Ugs.CloudSave.OnStatesLoaded += () =>
		{
			UpdateScore();
			//Debug.Log("OnStatesLoaded from Google Cloud Save");
		};
		
		Ugs.CloudSave.OnStatesLoadingFailed += () =>
		{
			//Debug.Log("OnStatesLoadingFailed from Google Cloud Save");
		};
	}
	
	// Use this for initialization
	void Start (){
		//flurryAnalyticManager = FlurryAnalyticsManager.GetInstance();
		
		Ugs.Config.AppStateEnabled = true;
		Ugs.Config.GamesEnabled = true;
		
		InitConnectionCallbacks();
		
		Ugs.Game.OnLeaderboardsLoaded += () =>
		{
			//Debug.Log("OnLeaderboardsLoaded");
			//foreach(var leaderboard in Ugs.Game.Leaderboards)
			//Debug.Log("  " + leaderboard);
		};
		
		Ugs.Game.OnLeaderboardsLoadingFailed += () =>
		{
			//Debug.Log("OnLeaderboardsLoadingFailed");
			//Debug.LogWarning("Leaderboards loading failed");
		};
		
		Ugs.Game.OnLeaderboardScoresLoaded += (leaderboard) =>
		{
			//Debug.Log("OnLeaderboardScoresLoaded");
			//Debug.Log("Leaderboard Scores loaded for " + leaderboard.DisplayName +
			//          " [" + leaderboard.Data.Span + ", " + leaderboard.Data.Collection + "]");
			//foreach(var score in leaderboard.Data.Scores)
			//Debug.Log("  [" + score.Rank + "] " + score.ScoreHolderName + " - " + score.Score);
		};
		
		Ugs.Game.OnLeaderboardScoresLoadingFailed += () =>
		{
			//Debug.Log("OnLeaderboardScoresLoadingFailed");
			//Debug.LogWarning("Leaderboard scores loading failed");
		};
		
		//Ugs.Client.Connect();
		//Ugs.Game.LoadLeaderboards();
		//Ugs.Game.ShowLeaderboards();
		//Ugs.Game.ShowLeaderboard(ScoreLeaderboardId.Trim());
		
		//Ugs.Game.SubmitScore(ScoreLeaderboardId.Trim(), score);
		
		/*
		 Ugs.Game.LoadScores(ScoreLeaderboardId.Trim(),
					Ugs.LeaderboardSpan.Weekly,
					Ugs.LeaderboardCollection.Public,
					Ugs.LeaderboardSeed.Top);*/
		
		//Ugs.Client.Disconnect();
		
		//Ugs.Client.SignOut();
	}
	
	public void SignIn(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			Ugs.Client.SignIn();
		}
		#endif
	}

	//silent
	public void Connect(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			Ugs.Client.Connect();
		}
		#endif
	}

	public void GPSSignIn(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){			
			Ugs.Client.SignIn();
		}
		#endif
	}

	public void GPSConnect(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			Ugs.Client.Connect();
		}
		#endif
	}

	public void ShowLeaderBoard(){
		#if UNITY_ANDROID
			if(Application.platform == RuntimePlatform.Android){
				Ugs.Game.ShowLeaderboard(ScoreLeaderboardId.Trim());
			}
		#endif
	}
	
	public void CheckBeforeAnyAction(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){			
			if(connectionState == ConnectionState.Disconnected){
				if(null!=Connecting){
					Connecting();
				}
			}			

			if(connectionState == ConnectionState.Disconnected){
				SignIn();
			}else{
				Connect();
				Ugs.Game.ShowLeaderboard(ScoreLeaderboardId.Trim());
			}
		}
		#endif
	}

	public void LoadGame(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){			
			if(connectionState == ConnectionState.Disconnected){
				if(null!=Connecting){
					Connecting();
				}
			}			

			if(connectionState == ConnectionState.Disconnected){
				SignIn();
			}else{
				Connect();
			}
		}
		#endif
	}
	
	public void SubmitScore(long score){
		cacheScore =  score;
		if(connectionState == ConnectionState.Connected){
			if(cacheScore > cacheHiScore){
				Ugs.Game.SubmitScore(ScoreLeaderboardId.Trim(), score);
				SaveScore();
			}
		}
	}

	public void SubmitItem(List<int> items){
		if(items.Count > 0){
			cacheItems =  items;
			if(connectionState == ConnectionState.Connected){
				SaveItems();
			}
		}
	}

	private void SaveItems(){
		Ugs.Config.AppStateEnabled = true;

		int len = cacheItems.Count;
		if(len > 0){
			StringBuilder sb = new StringBuilder();

			for(int index=0;index<len;index++){
				sb.Append("-"+cacheItems[index].ToString());
			}

			//Debug.Log("items to save to google cloud " + sb.ToString());
			Ugs.CloudSave.SaveState(2,sb.ToString());	
		}
	}
	
	private void SaveScore(){
		Ugs.Config.AppStateEnabled = true;
		Ugs.CloudSave.SaveState(0,cacheScore.ToString());	
	}
	
	public void SubmitGold(long totalGold){
		cacheGold =  totalGold;
		if(connectionState == ConnectionState.Connected){
			SaveGold();
		}
	}
	
	private void SaveGold(){
		Ugs.Config.AppStateEnabled = true;
		Ugs.CloudSave.SaveState(1,cacheGold.ToString());
	}
	
	private void LoadScore(){
		Ugs.Config.AppStateEnabled = true;
		Ugs.CloudSave.LoadStates();
	}
	
	private void UpdateScore()
	{
		if(gameDataManager==null){
			gameDataManager = GameDataManager.GetInstance();
		}
		
		string statesText = "";
		if (!Ugs.Client.IsConnected)
			return;
		foreach(var state in Ugs.CloudSave.States)
		{
			if(state.Key == 0){
				cacheHiScore = long.Parse(state.StringData);
				if(gameDataManager.player.HiScore < (int)cacheHiScore){
					gameDataManager.player.HiScore = (int)cacheHiScore;
					gameDataManager.player.HasSetHighScore =false;
				}
			}
			
			if(state.Key == 1){
				if(!isFirstLoad){
					long gold = long.Parse(state.StringData);
					gameDataManager.player.TotalCoin += (int)gold;
					isFirstLoad =true;
					SubmitGold(gameDataManager.player.TotalCoin);
				}
			}

			if(state.Key == 2){
				if(!isItemLoaded){
					isItemLoaded = true;
					string itemsLoaded =(state.StringData);
					Debug.Log("check loaded items " + itemsLoaded);
					string[] itemSet = itemsLoaded.Split('-');
					int len = itemSet.Length;
					for(int index=0;index<len;index++){
						int itemId;
						Debug.Log("pasing id " + itemSet.GetValue(index));
						if( Int32.TryParse((string)itemSet.GetValue(index), out itemId) ){
							itemId = Int32.Parse((string)itemSet.GetValue(index)); 
							Debug.Log("parsing successful adding  item id: " + itemId);
							gameDataManager.player.AddItem(itemId);
						}
					}
				}
			}

			statesText += "State[" + state.Key + "] = " + state.StringData + "\n";
		}
		if (statesText == "")
			statesText = "States list is empty";
		
		Debug.Log( "UpdateScore statesText: " + statesText );
	}
}

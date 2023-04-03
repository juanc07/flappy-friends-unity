using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

public static class UgsExtensions
{
	public static int ErrorCode(this Ugs.ConnectionResult connectionResult)
	{
		return (int)connectionResult % 1000;
	}
	
	public static string Details(this Ugs.ConnectionResult connectionResult)
	{
		switch(connectionResult)
		{
		case Ugs.ConnectionResult.CONNECTION_CALL_FAILED:
			return "Android plugin not available (also happens when running in Editor) OR Connection is already in progress.";
		case Ugs.ConnectionResult.SERVICE_MISSING:
			return "Google Play services is missing on this device. Can be installed from Google Play.";
		case Ugs.ConnectionResult.SERVICE_VERSION_UPDATE_REQUIRED:
			return "The installed version of Google Play services is out of date. Can be installed from Google Play.";
		case Ugs.ConnectionResult.SERVICE_DISABLED:
			return "The installed version of Google Play services has been disabled on this device.";
		case Ugs.ConnectionResult.SIGN_IN_REQUIRED:
			return "Sign in wasn't complete. Either canceled by user or failed to authorize. Check that:" +
				" you have internet connection; " +
				" application id is correct; " +
				" apk is signed with the same keystore that was used for linked app fingerprint; " +
				" your account is present in the list of testers.";
		case Ugs.ConnectionResult.INVALID_ACCOUNT:
			return "The client attempted to connect to the service with an invalid account name specified.";
		case Ugs.ConnectionResult.RESOLUTION_REQUIRED:
			return "Completing the connection requires some form of resolution. You shouldn't see this error, it should be handled inside the plugin, so it's most likely a plugin bug.";
		case Ugs.ConnectionResult.NETWORK_ERROR:
			return "A network error occurred. Check your internet connection.";
		case Ugs.ConnectionResult.INTERNAL_ERROR:
			return "An internal error occurred.";
		case Ugs.ConnectionResult.SERVICE_INVALID:
			return "The version of the Google Play services installed on this device is not authentic. Correct version can be installed from Google Play.";
		case Ugs.ConnectionResult.DEVELOPER_ERROR:
			return "The application is misconfigured. Check Google Play Setup window for errors and warnings.";
		case Ugs.ConnectionResult.LICENSE_CHECK_FAILED:
			return "The application is not licensed to the user.";
		}
		return "Unknown connection result.";
	}
}


namespace Ugs
{
	public enum ConnectionResult
	{
		SUCCESS = 0,
		
		UNKNOWN = 1000,
		SERVICE_MISSING = 1001,
		SERVICE_VERSION_UPDATE_REQUIRED = 1002,
		SERVICE_DISABLED = 1003,
		SIGN_IN_REQUIRED = 1004,
		INVALID_ACCOUNT = 1005,
		RESOLUTION_REQUIRED = 1006,
		NETWORK_ERROR = 1007,
		INTERNAL_ERROR = 1008,
		SERVICE_INVALID = 1009,
		DEVELOPER_ERROR = 1010,
		LICENSE_CHECK_FAILED = 1011,
		CONNECTION_RESOLVER_INCONSISTENCY = 1098,
		CONNECTION_CALL_FAILED = 1099,
		
		GPGUnknown = 2000,
		GPGInvalidAuthenticationError = 2001,
		GPGNetworkUnavailableError = 2002,
  		GPGServiceMethodFailedError = 2003,
  		GPGRevisionStaleError = 2004,	
	}
	
	public static class Client
	{
		#region Logging
		private static void DebugLog(string message)
		{
			if (!Config.Verbose)
				return;
			
			Debug.Log("[Gpg.Client] " + message);
		}
		#endregion
		
		#region Callbacks
		public static event Action OnConnected;
		private static void InvokeOnConnected()
		{
			var c = OnConnected;
			if (c != null)
				c();
		}
		
		public static event Action OnConnectionFailed;
		private static void InvokeOnConnectionFailed()
		{
			var c = OnConnectionFailed;
			if (c != null)
				c();
		}
		
		public static event Action OnDisconnected;
		private static void InvokeOnDisconnected()
		{
			var c = OnDisconnected;
			if (c != null)
				c();
		}
		#endregion
		
		public static ConnectionResult ConnectionResult { get; private set; }
		
		internal static bool GamesCallsAllowed
		{
			get
			{
				return (IsConnected || IsConnecting) ? _gamesEnabled : Config.GamesEnabled;
			}
		}
		internal static bool AppStateCallsAllowed
		{
			get
			{
				return (IsConnected || IsConnecting) ? _appStateEnabled : Config.AppStateEnabled;
			}
		}
		
		private static bool _appStateEnabled;
		private static bool _gamesEnabled;
		private static bool _loadDataOnConnect;
		
		private static List<Action> _scheduledActions = new List<Action>();
		private static bool _scheduledAction;
		private static bool _scheduling;
		
		internal static void ClearData()
		{
			Client.Clear();
			Game.Clear();
			CloudSave.Clear();
			Multiplayer.Clear();
			Resources.Clear();
		}
		
		internal static void Clear()
		{
			_scheduledActions.Clear();
			ConnectionResult = ConnectionResult.SUCCESS;
		}
		
		internal static bool Schedule(bool load, bool save, Action action)
		{
			var lazySignIn = save ? Config.LazySignInOnWrites : Config.LazySignIn;
			
			if (!lazySignIn || _scheduledAction || _scheduling || IsConnected)
				return false;
			
			_scheduling = true;
			
			var dataLoad = load && _loadDataOnConnect;
			if (!dataLoad)
				_scheduledActions.Add(action);
			
			var processed = false;
			if (!IsConnecting)
				processed = Connect(false);
			
			_scheduling = false;
			
			return !dataLoad || processed;
		}
		
		private static void FlushScheduledActions()
		{
			_scheduledAction = true;
			foreach(var scheduledAction in _scheduledActions)
			{
				try
				{
					scheduledAction();
				}
				catch(Exception ex)
				{
					DebugLog("Scheduled action failed:\n" + ex);
				}
			}
			_scheduledAction = false;
			_scheduledActions.Clear();
		}


#if UNITY_ANDROID
		#region Android Setup
		private static AndroidJavaObject _androidAdapterJavaObject;
		private static AndroidJavaObject _gamesClientJavaObject;
		private static AndroidJavaObject _appStateClientJavaObject;
		private static GameObject _androidAdapterGameObject;
		private static UgsAdapter _androidAdapterComponent;
		
		private static void InitAndroidAdapterIfNeeded()
		{
			if (_androidAdapterGameObject == null ||
				_androidAdapterComponent == null)
			{
				if (_androidAdapterGameObject != null)
					GameObject.DestroyImmediate(_androidAdapterGameObject);
					
				_androidAdapterGameObject = new GameObject("UgsAndroidAdapter");
				GameObject.DontDestroyOnLoad(_androidAdapterGameObject);
				_androidAdapterComponent = _androidAdapterGameObject.AddComponent<UgsAdapter>();
				
				_androidAdapterComponent.OnConnected += Client._OnConnected;
				_androidAdapterComponent.OnDisconnected += Client._OnDisconnected;
				_androidAdapterComponent.OnConnectionFailed += Client._OnConnectionFailed;
				
				_androidAdapterComponent.OnImageLoaded += Resources._OnImageLoaded;
				
				_androidAdapterComponent.OnAchievementsLoaded += Game._OnAchievementsLoaded;
				_androidAdapterComponent.OnGamesLoaded += Game._OnGamesLoaded;
				_androidAdapterComponent.OnLeaderboardMetadataLoaded += Game._OnLeaderboardMetadataLoaded;
				_androidAdapterComponent.OnLeaderboardScoresLoaded += Game._OnLeaderboardScoresLoaded;
				
				_androidAdapterComponent.OnStatesListLoaded = CloudSave._OnStatesListLoaded;
				_androidAdapterComponent.OnStateLoaded = CloudSave._OnStateLoaded;
				_androidAdapterComponent.OnStateSaved = CloudSave._OnStateSaved;
				_androidAdapterComponent.OnStateConflicted = CloudSave._OnStateConflicted;
				_androidAdapterComponent.OnStateDeleted = CloudSave._OnStateDeleted;
				
				_androidAdapterComponent.OnPlayersSelected = Multiplayer._OnPlayersSelected;
				_androidAdapterComponent.OnInvitationSelected = Multiplayer._OnInvitationSelected;
				_androidAdapterComponent.OnInvitablePlayersLoaded = Multiplayer._OnInvitablePlayersLoaded;
				_androidAdapterComponent.OnMoreInvitablePlayersLoaded = Multiplayer._OnMoreInvitablePlayersLoaded;
				_androidAdapterComponent.OnInvitationReceived = Multiplayer._OnInvitationReceived;
				_androidAdapterComponent.OnInvitationsLoaded = Multiplayer._OnInvitationsLoaded;
				_androidAdapterComponent.OnRoomJoined = Multiplayer._OnRoomJoined;
				_androidAdapterComponent.OnRoomLeft = Multiplayer._OnRoomLeft;
				_androidAdapterComponent.OnRoomConnected = Multiplayer._OnRoomConnected;
				_androidAdapterComponent.OnRoomCreated = Multiplayer._OnRoomCreated;
				_androidAdapterComponent.OnRealTimeMessageSent = Multiplayer._OnReliableMessageSent;
				_androidAdapterComponent.OnRealTimeMessageReceived = Multiplayer._OnMessageReceived;
				_androidAdapterComponent.OnConnectedToRoom = Multiplayer._OnConnectedToRoom;
				_androidAdapterComponent.OnDisconnectedFromRoom = Multiplayer._OnDisconnectedFromRoom;
				_androidAdapterComponent.OnPeerDeclined = Multiplayer._OnPeerDeclined;
				_androidAdapterComponent.OnPeerInvitedToRoom = Multiplayer._OnPeerInvitedToRoom;
				_androidAdapterComponent.OnPeerJoined = Multiplayer._OnPeerJoined;
				_androidAdapterComponent.OnPeerLeft = Multiplayer._OnPeerLeft;
				_androidAdapterComponent.OnPeersConnected = Multiplayer._OnPeersConnected;
				_androidAdapterComponent.OnPeersDisconnected = Multiplayer._OnPeersDisconnected;
				_androidAdapterComponent.OnRoomAutoMatching = Multiplayer._OnRoomAutoMatching;
				_androidAdapterComponent.OnRoomConnecting = Multiplayer._OnRoomConnecting;
				_androidAdapterComponent.OnWaitingRoomFinished = Multiplayer._OnWaitingRoomFinished;
				_androidAdapterComponent.OnWaitingRoomCanceled = Multiplayer._OnWaitingRoomCanceled;
				_androidAdapterComponent.OnWaitingRoomLeft = Multiplayer._OnWaitingRoomLeft;
			}
			
			if (_androidAdapterJavaObject == null)
			{
				_androidAdapterJavaObject = new AndroidJavaObject("com.artofbytes.gpg.android.GpgAndroidAdapter",
					_androidAdapterGameObject.name, _gamesEnabled, _appStateEnabled);
				_appStateClientJavaObject = _androidAdapterJavaObject.Call<AndroidJavaObject>("getAppStateClient");
				_gamesClientJavaObject = _androidAdapterJavaObject.Call<AndroidJavaObject>("getGamesClient");
			}
		}
		
		internal static AndroidJavaObject AndroidAdapter
		{
			get
			{
				InitAndroidAdapterIfNeeded();
				return _androidAdapterJavaObject;
			}
		}
		
		internal static AndroidJavaObject GamesClient
		{
			get
			{
				InitAndroidAdapterIfNeeded();
				return _gamesClientJavaObject;
			}
		}
		
		internal static AndroidJavaObject AppStateClient
		{
			get
			{
				InitAndroidAdapterIfNeeded();
				return _appStateClientJavaObject;
			}
		}
		
		#endregion
		
		public static string CurrentAccountName { get { return _gamesClientJavaObject != null ? _gamesClientJavaObject.Call<string>("getCurrentAccountName") : ""; } }
		public static bool IsConnected { get { return _androidAdapterJavaObject != null && _androidAdapterJavaObject.Call<bool>("isConnected"); } }
		public static bool IsConnecting { get { return _androidAdapterJavaObject != null && _androidAdapterJavaObject.Call<bool>("isConnecting"); } }

		private static bool Connect(bool silent)
		{
			if (_appStateEnabled != Config.AppStateEnabled ||
			    _gamesEnabled != Config.GamesEnabled)
			{
				_appStateEnabled = Config.AppStateEnabled;
				_gamesEnabled = Config.GamesEnabled;
				_androidAdapterJavaObject = null;
				_gamesClientJavaObject = null;
				_appStateClientJavaObject = null;
			}
			
			_loadDataOnConnect = Config.LoadDataOnConnect;
			if (!AndroidAdapter.Call<bool>("connect", silent))
			{
				ConnectionResult = ConnectionResult.CONNECTION_CALL_FAILED;
				InvokeOnConnectionFailed();
				FlushScheduledActions();
				return false;
			}
			return true;
		}

		public static void Connect()
		{
			Connect(true);
		}
		public static void SignIn()
		{
			Connect(false);
		}

		internal static void _OnConnected(string status)
		{
			ConnectionResult = ConnectionResult.SUCCESS;
			InvokeOnConnected();
			FlushScheduledActions();
			
			if (_loadDataOnConnect)
			{
				if (_appStateEnabled)
				{
					CloudSave.LoadStates();
				}
				if (_gamesEnabled)
				{
					Game.LoadGameDetails();
					Game.LoadAchievements();
					Game.LoadLeaderboards();
				}
			}
		}

		internal static void _OnConnectionFailed(string status)
		{
			ConnectionResult = ConnectionResult.UNKNOWN;
			int statusCode = 0;
			var statusParts = status.Split(' ');
			if (statusParts.Length > 0 && int.TryParse(status, out statusCode))
			{
				try
				{
					ConnectionResult = (ConnectionResult)(statusCode + 1000);
				}
				catch(Exception ex)
				{
					DebugLog("Unknown connection result " + statusCode + ", error:\n" + ex);
					ConnectionResult = ConnectionResult.UNKNOWN;
				}
			}
			InvokeOnConnectionFailed();
			FlushScheduledActions();
		}
		
		public static void Disconnect()
		{
			if (!AndroidAdapter.Call<bool>("disconnect"))
			{
				ClearData();
				InvokeOnDisconnected();
			}
		}
		internal static void _OnDisconnected(string status)
		{
			ClearData();
			InvokeOnDisconnected();
		}
		
		public static void SignOut()
		{
			AndroidAdapter.Call("signOut");
			Disconnect();
		}
#endif
#if UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern void GpgInit(string adapterName, bool verbose, bool gamesEnabled, bool appStateEnabled);

		[DllImport ("__Internal")]
		private static extern void GpgConnect(bool silent);
		
		[DllImport ("__Internal")]
		private static extern void GpgDisconnect();
		
		[DllImport ("__Internal")]
		private static extern void GpgSignOut();
		
		[DllImport ("__Internal")]
		private static extern bool GpgIsConnected();
		
		[DllImport ("__Internal")]
		private static extern string GpgGetCurrentAccountName();
		
		[DllImport ("__Internal")]
		private static extern bool GpgIsConnecting();


		private static GameObject _iosAdapterGameObject;
		private static UgsAdapter _iosAdapterComponent;
		private static bool _iosInited = false;

		private static void InitIosAdapterIfNeeded()
		{
			if (_iosAdapterGameObject == null ||
			    _iosAdapterComponent == null)
			{
				if (_iosAdapterGameObject != null)
					GameObject.DestroyImmediate(_iosAdapterGameObject);
				
				_iosAdapterGameObject = new GameObject("UgsIosAdapter");
				GameObject.DontDestroyOnLoad(_iosAdapterGameObject);
				_iosAdapterComponent = _iosAdapterGameObject.AddComponent<UgsAdapter>();
				
				_iosAdapterComponent.OnConnected += Client._OnConnected;
				_iosAdapterComponent.OnDisconnected += Client._OnDisconnected;
				_iosAdapterComponent.OnConnectionFailed += Client._OnConnectionFailed;

				_iosAdapterComponent.OnAchievementsLoaded += Game._OnAchievementsLoaded;

				//_iosAdapterComponent.OnGamesLoaded += Game._OnGamesLoaded;

				_iosAdapterComponent.OnLeaderboardMetadataLoaded += Game._OnLeaderboardMetadataLoaded;
				_iosAdapterComponent.OnLeaderboardScoresLoaded += Game._OnLeaderboardScoresLoaded;
				
				_iosAdapterComponent.OnStatesListLoaded = CloudSave._OnStatesListLoaded;
				_iosAdapterComponent.OnStateLoaded = CloudSave._OnStateLoaded;
				_iosAdapterComponent.OnStateSaved = CloudSave._OnStateSaved;
				_iosAdapterComponent.OnStateConflicted = CloudSave._OnStateConflicted;
				_iosAdapterComponent.OnStateDeleted = CloudSave._OnStateDeleted;
				_iosAdapterComponent.OnStateResolved = CloudSave._OnStateResolved;

				//_iosAdapterComponent.OnImageLoaded += Resources._OnImageLoaded;
			}

			if (!_iosInited)
			{
				_iosInited = true;
				GpgInit(_iosAdapterGameObject.name, Config.Verbose, _gamesEnabled, _appStateEnabled);
			}
		}

		public static string CurrentAccountName { get { return GpgGetCurrentAccountName(); } }
		public static bool IsConnected { get { return GpgIsConnected(); } }
		public static bool IsConnecting { get { return GpgIsConnecting(); } }

		private static bool Connect(bool silent)
		{
			if (_appStateEnabled != Config.AppStateEnabled ||
			    _gamesEnabled != Config.GamesEnabled)
			{
				_appStateEnabled = Config.AppStateEnabled;
				_gamesEnabled = Config.GamesEnabled;
				_iosInited = false;
			}
			
			_loadDataOnConnect = Config.LoadDataOnConnect;
			InitIosAdapterIfNeeded();
			GpgConnect(silent);
			return true;
		}

		public static void Connect()
		{
			Connect(true);
		}
		
		public static void SignIn()
		{
			Connect(false);
		}

		internal static void _OnConnected(string status)
		{
			ConnectionResult = ConnectionResult.SUCCESS;
			InvokeOnConnected();
			FlushScheduledActions();
			
			if (_loadDataOnConnect)
			{
				if (_appStateEnabled)
				{
					CloudSave.LoadStates();
				}
				if (_gamesEnabled)
				{
					Game.LoadGameDetails();
					Game.LoadAchievements();
					Game.LoadLeaderboards();
				}
			}
		}
		
		internal static void _OnConnectionFailed(string status)
		{
			ConnectionResult = ConnectionResult.GPGUnknown;
			int statusCode = 0;
			var statusParts = status.Split(' ');
			if (statusParts.Length > 0 && int.TryParse(status, out statusCode))
			{
				try
				{
					ConnectionResult = (ConnectionResult)(statusCode + 2000);
				}
				catch(Exception ex)
				{
					DebugLog("Unknown connection result " + statusCode + ", error:\n" + ex);
					ConnectionResult = ConnectionResult.GPGUnknown;
				}
			}
			InvokeOnConnectionFailed();
			FlushScheduledActions();
		}

		public static void Disconnect()
		{
			if (!IsConnected && !IsConnecting)
				return;
			GpgDisconnect();
		}
		
		internal static void _OnDisconnected(string status)
		{
			ClearData();
			InvokeOnDisconnected();
		}

		public static void SignOut()
		{
			if (!IsConnected && !IsConnecting)
				return;
			GpgSignOut();
		}
#endif

#if !UNITY_ANDROID && !UNITY_IOS
		public static string CurrentAccountName { get { return string.Empty; } }
		public static bool IsConnected { get { return false; } }
		public static bool IsConnecting { get { return false; } }
		
		private static bool Connect(bool silent)
		{
			_appStateEnabled = false;
			_gamesEnabled = false;
			_loadDataOnConnect = false;
		
			return false;
		}
		
		public static void Connect()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void SignIn()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void Disconnect()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void SignOut()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
#endif
	}
}

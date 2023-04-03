using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public delegate byte [] ConflictsResolver(int stateKey, byte[] local, byte [] remote);
	
	public static class CloudSave
	{
		#region Logging
		private static void DebugLog(string message)
		{
			if (!Config.Verbose)
				return;
			
			Debug.Log("[Gpg.CloudSave] " + message);
		}
		#endregion

		#region Callbacks
		public static event Action OnStatesLoaded;
		private static void InvokeOnStatesLoaded()
		{
			var c = OnStatesLoaded;
			if (c != null)
				c();
		}
		
		public static event Action OnStatesLoadingFailed;
		private static void InvokeOnStatesLoadingFailed()
		{
			var c = OnStatesLoadingFailed;
			if (c != null)
				c();
		}
		
		public static event Action<State> OnStateSaved;
		private static void InvokeOnStateSaved(State state)
		{
			var c = OnStateSaved;
			if (c != null)
				c(state);
		}
		
		public static event Action<int> OnStateSavingFailed;
		private static void InvokeOnStateSavingFailed(int stateKey)
		{
			var c = OnStateSavingFailed;
			if (c != null)
				c(stateKey);
		}
		
		public static event Action<int> OnStateDeleted;
		private static void InvokeOnStateDeleted(int stateKey)
		{
			var c = OnStateDeleted;
			if (c != null)
				c(stateKey);
		}
		
		public static event Action<int> OnStateDeletionFailed;
		private static void InvokeOnStateDeletionFailed(int stateKey)
		{
			var c = OnStateDeletionFailed;
			if (c != null)
				c(stateKey);
		}
		#endregion
		
		public static IEnumerable<State> States { get { return _states; } }
		private static List<State> _states = new List<State>();
		
		public static byte[] ResolveUsingLocal(int stateKey, byte[] local, byte[] remote)
		{
			return local;
		}
		
		public static byte[] ResolveUsingRemote(int stateKey, byte[] local, byte[] remote)
		{
			return remote;
		}
		
		public static ConflictsResolver ConflictsResolver = ResolveUsingRemote;
		
		internal static void Clear()
		{
			_states.Clear();
		}

		
		private enum Operation
		{
			None,
			LoadAll,
		}
		
		private static Operation _operation = Operation.None;
		private static void FailStateOperation(int stateKey = -1)
		{
			if (_operation == Operation.LoadAll)
				InvokeOnStatesLoadingFailed();
			
			_operation = Operation.None;
		}
		

		
#if UNITY_ANDROID
		private static int _maxCloudSaveKeys = 0;
		public static int MaxCloudSaveKeys
		{
			get
			{
				if (_maxCloudSaveKeys == 0)
					_maxCloudSaveKeys = Client.AppStateClient.Call<int>("getMaxNumKeys");
				return _maxCloudSaveKeys;
			}
		}
		
		private static int _maxCloudSaveStateSize;
		public static int MaxCloudSaveStateSize
		{
			get 
			{
				if (_maxCloudSaveStateSize == 0)
					_maxCloudSaveStateSize = Client.AppStateClient.Call<int>("getMaxStateSize");
				return _maxCloudSaveStateSize;
			}
		}

		
		public static void LoadStates()
		{
			if (!Client.AppStateCallsAllowed)
			{
				DebugLog("Failed to load states, Gpg.Config.AppStateEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadStates))
				return;
			
			_operation = Operation.LoadAll;
			_states.Clear();
			if (!Client.AndroidAdapter.Call<bool>("listStates"))
			{
				DebugLog("Failed to load states, probably not connected to Google Play Game");
				_states.Clear();
				FailStateOperation();
			}
		}

		private static bool LoadState(int key)
		{
			return Client.AndroidAdapter.Call<bool>("loadState", key);
		}
		
		internal static void _OnStatesListLoaded(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				_states.Clear();
				FailStateOperation();
				return;
			}
			
			var states = Client.AndroidAdapter.Call<AndroidJavaObject>("getStates");
			if (states != null)
			{
				_states.Clear();
				var statesCount = states.Call<int>("getCount");
				for (var i = 0; i < statesCount; ++i)
				{
					var state = states.Call<AndroidJavaObject>("get", i);
					if (state == null)
						break;
					_states.Add(new State(state));
				}
			}
			
			FinalizeStateOperation();
		}

		private static void FinalizeStateOperation(State state = null)
		{
			if (_operation == Operation.LoadAll)
			{
				foreach (var s in _states)
				{
					if (!s.HasConflict)
						continue;
					if (!LoadState(s.Key))
					{
						FailStateOperation(s.Key);
					}
					return;
				}
				InvokeOnStatesLoaded();
			}
			
			_operation = Operation.None;
		}
		
		public static void SaveState(int stateKey, string data)
		{
			SaveState(stateKey, State.StringToByteArray(data));
		}
		
		public static void SaveState(int stateKey, byte [] data)
		{
			if (!Client.AppStateCallsAllowed)
			{
				DebugLog("Failed to save state, Gpg.Config.AppStateEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => SaveState(stateKey, data)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("updateState", stateKey, data))
			{
				DebugLog("Failed to save state, probably not connected to Google Play Game");
				FailStateOperation(stateKey);
				return;
			}
		}
		
		internal static void _OnStateSaved(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from save callback");
			}
			
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnStateSavingFailed(stateKey);
				return;
			}
			
			var localData = Client.AndroidAdapter.Call<byte[]>("getStateLocalData", stateKey);
			State newState = null;
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					state.SetData(localData);
					newState = state;
					break;
				}
			}
			if (newState == null)
			{
				newState = new State(stateKey, localData);
				_states.Add(newState);
			}
			InvokeOnStateSaved(newState);
		}
		
		internal static void _OnStateLoaded(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from load callback");
			}
			
			if (!Utils.IsStatusOk(status))
			{
				FailStateOperation(stateKey);
				return;
			}
			
			var localData = Client.AndroidAdapter.Call<byte[]>("getStateLocalData", stateKey);
			State newState = null;
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					state.SetData(localData);
					newState = state;
					break;
				}
			}
			if (newState == null)
			{
				newState = new State(stateKey, localData);
				_states.Add(newState);
			}
			FinalizeStateOperation(newState);
		}
		
		internal static void _OnStateConflicted(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from conflict callback");
				return;
			}
			
			var localData = Client.AndroidAdapter.Call<byte[]>("getStateLocalData", stateKey);
			var remoteData = Client.AndroidAdapter.Call<byte[]>("getStateConflictData", stateKey);
			var resolvedVersion = Client.AndroidAdapter.Call<string>("getStateConflictToken", stateKey);
			
			var knownState = false;
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					state.SetConflict(localData, remoteData, resolvedVersion);
					knownState = true;
					break;
				}
			}
			if (!knownState)
			{
				_states.Add(new State(stateKey, localData, remoteData, resolvedVersion));
			}
			
			if (ConflictsResolver == null)
				ConflictsResolver = ResolveUsingRemote;
			var resolvedData = ConflictsResolver(stateKey, localData, remoteData);
			
			if (!Client.AndroidAdapter.Call<bool>("resolveState", stateKey, resolvedVersion, resolvedData))
			{
				FailStateOperation(stateKey);
				return;
			}
		}
		
		public static void DeleteState(int stateKey)
		{
			if (!Client.AppStateCallsAllowed)
			{
				DebugLog("Failed to delete state, Gpg.Config.AppStateEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => DeleteState(stateKey)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("deleteState", stateKey))
			{
				DebugLog("Failed to delete state, probably not connected to Google Play Game");
				InvokeOnStateDeletionFailed(stateKey);
			}
		}
		
		internal static void _OnStateDeleted(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from deletion callback");
			}
			
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnStateDeletionFailed(stateKey);
				return;
			}
			
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					_states.Remove(state);
					break;
				}
			}
			InvokeOnStateDeleted(stateKey);
		}
#elif UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern int GpgGetMaxCloudSaveKeys();
		
		[DllImport ("__Internal")]
		private static extern int GpgGetMaxCloudSaveStateSize();
		
		[DllImport ("__Internal")]
		private static extern void GpgLoadStates();
		
		[DllImport ("__Internal")]
		private static extern void GpgLoadState(int key);
		
		[DllImport ("__Internal")]
		private static extern void GpgSaveState(int key, byte [] data, int size);
		
		[DllImport ("__Internal")]
		private static extern void GpgResolveState(int key, byte [] data, int size);
		
		[DllImport ("__Internal")]
		private static extern void GpgDeleteState(int key);

		[DllImport ("__Internal")]
		private static extern int GpgGetLocalState(byte [] buffer, int bufferSize);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetRemoteState(byte [] buffer, int bufferSize);


		private static byte [] _buffer = new byte[128 * 1024];

		private static byte [] GetLocalState()
		{
			int size = GpgGetLocalState(_buffer, _buffer.Length);
			if (size == 0)
				return new byte[0];
			var result = new byte[size];
			for (var i = 0; i < size; ++i)
				result[i] = _buffer[i];
			return result;
		}
		
		private static byte [] GetRemoteState()
		{
			int size = GpgGetRemoteState(_buffer, _buffer.Length);
			if (size == 0)
				return new byte[0];
			var result = new byte[size];
			for (var i = 0; i < size; ++i)
				result[i] = _buffer[i];
			return result;
		}

		private static Queue<int> _statesToLoad = new Queue<int>();

		public static int MaxCloudSaveKeys
		{
			get
			{
				return GpgGetMaxCloudSaveKeys();
			}
		}
		
		public static int MaxCloudSaveStateSize
		{
			get 
			{
				return GpgGetMaxCloudSaveStateSize();
			}
		}
		
		public static void LoadStates()
		{
			if (!Client.AppStateCallsAllowed)
			{
				DebugLog("Failed to load states, Gpg.Config.AppStateEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadStates))
				return;
			
			_operation = Operation.LoadAll;
			_states.Clear();
			_statesToLoad.Clear();
			_statesToLoad.Enqueue(0);
			_statesToLoad.Enqueue(1);
			_statesToLoad.Enqueue(2);
			_statesToLoad.Enqueue(3);
			FinalizeStateOperation();
		}

		internal static void _OnStatesListLoaded(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				_states.Clear();
				FailStateOperation();
				return;
			}

			_statesToLoad.Clear();
			for (var i = 1; ; ++i)
			{
				var key = 0;
				if (!Utils.TryGetStatusArg(status, i, ref key))
					break;
				_statesToLoad.Enqueue(key);
			}

			FinalizeStateOperation();
		}

		private static bool LoadState(int key)
		{
			GpgLoadState(key);
			return true;
		}

		private static void FinalizeStateOperation(State state = null)
		{
			if (_operation == Operation.LoadAll)
			{
				if (_statesToLoad.Count > 0)
				{
					LoadState(_statesToLoad.Dequeue());
					return;
				}

				foreach (var s in _states)
				{
					if (s.HasConflict)
					{
						s.SetData(ConflictsResolver(s.Key, s.Data, s.ConflictData));
						GpgResolveState(s.Key, s.Data, s.Data.Length);
						return;
					}
				}
				InvokeOnStatesLoaded();
			}
			
			_operation = Operation.None;
		}




		public static void SaveState(int stateKey, string data)
		{
			SaveState(stateKey, State.StringToByteArray(data));
		}
		
		public static void SaveState(int stateKey, byte [] data)
		{
			if (!Client.AppStateCallsAllowed)
			{
				DebugLog("Failed to save state, Gpg.Config.AppStateEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => SaveState(stateKey, data)))
				return;

			GpgSaveState(stateKey, data, data.Length);
		}

		public static void DeleteState(int stateKey)
		{
			GpgDeleteState(stateKey);
		}

		internal static void _OnStateLoaded(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from callback");
			}
			
			var statusCode = 0;
			if (Utils.TryGetStatusArg(status, 0, ref statusCode) && statusCode == -1)
			{
				FinalizeStateOperation();
				return;
			}
			
			if (!Utils.IsStatusOk(status))
			{
				FailStateOperation(stateKey);
				return;
			}
			
			var localData = GetLocalState();
			State newState = null;
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					state.SetData(localData);
					newState = state;
					break;
				}
			}
			if (newState == null)
			{
				newState = new State(stateKey, localData);
				_states.Add(newState);
			}
			FinalizeStateOperation(newState);
		}

		internal static void _OnStateSaved(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from callback");
			}
			
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnStateSavingFailed(stateKey);
				return;
			}
			
			var localData = GetLocalState();
			State newState = null;
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					state.SetData(localData);
					newState = state;
					break;
				}
			}
			if (newState == null)
			{
				newState = new State(stateKey, localData);
				_states.Add(newState);
			}
			
			InvokeOnStateSaved(newState);
		}
		
		internal static void _OnStateConflicted(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from callback");
			}

			if (!Utils.IsStatusOk(status))
			{
				InvokeOnStateSavingFailed(stateKey);
				return;
			}
			
			var localData = GetLocalState();
			var remoteData = GetRemoteState();
			var resolvedData = ConflictsResolver(stateKey, localData, remoteData);
			GpgResolveState(stateKey, resolvedData, resolvedData.Length);
		}
		
		internal static void _OnStateResolved(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from callback");
			}
			
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnStateSavingFailed(stateKey);
				return;
			}
			
			var localData = GetLocalState();
			State newState = null;
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					state.SetData(localData);
					newState = state;
					break;
				}
			}
			if (newState == null)
			{
				newState = new State(stateKey, localData);
				_states.Add(newState);
			}
			
			InvokeOnStateSaved(newState);
		}

		internal static void _OnStateDeleted(string status)
		{
			var stateKey = -1;
			if (!Utils.TryGetStatusArg(status, 1, ref stateKey))
			{
				DebugLog("Failed to get stateKey from deletion callback");
			}
			
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnStateDeletionFailed(stateKey);
				return;
			}
			
			foreach(var state in _states)
			{
				if (state.Key == stateKey)
				{
					_states.Remove(state);
					break;
				}
			}
			InvokeOnStateDeleted(stateKey);
		}

#else
		public static int MaxCloudSaveKeys
		{
			get
			{
				Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
				return 0;
			}
		}
		
		public static int MaxCloudSaveStateSize
		{
			get 
			{
				Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
				return 0;
			}
		}
		
		public static void LoadStates()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void SaveState(int stateKey, string data)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void SaveState(int stateKey, byte [] data)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void DeleteState(int stateKey)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
#endif
	}
}

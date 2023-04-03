using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public class Game
	{
		#region Details
		public string Id { get; private set; }
		public string DisplayName { get; private set; }
		public string Description { get; private set; }
		public string PrimaryCategory { get; private set; }
		public string SecondaryCategory { get; private set; }
		public string DeveloperName { get; private set; }
		public string FeaturedImageUri { get; private set; }
		public string HiResImageUri { get; private set; }
		public string IconImageUri { get; private set; }
		
		public override string ToString()
		{
			var result = "[Game " + Id;
			result += " displayName = " + DisplayName;
			result += "]";
			return result;
		}
		
#if UNITY_ANDROID
		internal Game()
		{
		}
		
		internal Game(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getApplicationId");
			DisplayName = obj.Call<string>("getDisplayName");
			Description = obj.Call<string>("getDescription");
			PrimaryCategory = obj.Call<string>("getPrimaryCategory");
			SecondaryCategory = obj.Call<string>("getSecondaryCategory");
			DeveloperName = obj.Call<string>("getDeveloperName");
			
			FeaturedImageUri = string.Empty;
			if (obj.Call<bool>("hasFeaturedImageUri"))
			{
				var uri = obj.Call<AndroidJavaObject>("getFeaturedImageUri");
				if (uri != null)
					FeaturedImageUri = uri.Call<string>("toString");
			}
			
			HiResImageUri = string.Empty;
			try
			{
				var uri = obj.Call<AndroidJavaObject>("getHiResImageUri");
				if (uri != null)
					HiResImageUri = uri.Call<string>("toString");
			}
			catch
			{
				HiResImageUri = string.Empty;
			}
			
			IconImageUri = string.Empty;
			try
			{
				var uri = obj.Call<AndroidJavaObject>("getIconImageUri");
				if (uri != null)
					IconImageUri = uri.Call<string>("toString");
			}
			catch
			{
				IconImageUri = string.Empty;
			}
		}
#endif
		#endregion
		
		
		
		
		
		#region Logging
		private static void DebugLog(string message)
		{
			if (!Config.Verbose)
				return;
			
			Debug.Log("[Gpg.Game] " + message);
		}
		#endregion
		
		#region Callbacks
		public static event Action OnGameDetailsLoaded;
		private static void InvokeOnGameDetailsLoaded()
		{
			var c = OnGameDetailsLoaded;
			if (c != null)
				c();
		}
		
		public static event Action OnGameDetailsLoadingFailed;
		private static void InvokeOnGameDetailsLoadingFailed()
		{
			var c = OnGameDetailsLoadingFailed;
			if (c != null)
				c();
		}
		
		public static event Action OnAchievementsLoaded;
		private static void InvokeOnAchievementsLoaded()
		{
			var c = OnAchievementsLoaded;
			if (c != null)
				c();
		}
		
		public static event Action OnAchievementsLoadingFailed;
		private static void InvokeOnAchievementsLoadingFailed()
		{
			var c = OnAchievementsLoadingFailed;
			if (c != null)
				c();
		}
		
		public static event Action<Achievement> OnAchievementChanged;
		private static void InvokeOnAchievementChanged(Achievement achievement)
		{
			var c = OnAchievementChanged;
			if (c != null)
				c(achievement);
		}
		
		public static event Action OnLeaderboardsLoaded;
		private static void InvokeOnLeaderboardsLoaded()
		{
			var c = OnLeaderboardsLoaded;
			if (c != null)
				c();
		}
		
		public static event Action OnLeaderboardsLoadingFailed;
		private static void InvokeOnLeaderboardsLoadingFailed()
		{
			var c = OnLeaderboardsLoadingFailed;
			if (c != null)
				c();
		}
		
		public static event Action<Leaderboard> OnLeaderboardScoresLoaded;
		private static void InvokeOnLeaderboardScoresLoaded(Leaderboard leaderboard)
		{
			var c = OnLeaderboardScoresLoaded;
			if (c != null)
				c(leaderboard);
		}
		
		public static event Action OnLeaderboardScoresLoadingFailed;
		private static void InvokeOnLeaderboardScoresLoadingFailed()
		{
			var c = OnLeaderboardScoresLoadingFailed;
			if (c != null)
				c();
		}
		#endregion
				
		public static IEnumerable<Achievement> Achievements { get { return _achievements; } }
		public static IEnumerable<Leaderboard> Leaderboards { get { return _leaderboards; } }
		public static Game Details { get { return _details; } }
		
		private static List<Achievement> _achievements = new List<Achievement>();
		private static List<Leaderboard> _leaderboards = new List<Leaderboard>();
		private static Game _details = new Game();
		
		internal static void Clear()
		{
			_achievements.Clear();
			_leaderboards.Clear();
			_details = new Game();
		}
		
	

#if UNITY_ANDROID
		private static AndroidJavaObject AndroidAdapter { get { return Client.AndroidAdapter; } }
		private static AndroidJavaObject GamesClient { get { return Client.GamesClient; } }
		
		#region Achievements
		public static void LoadAchievements()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load achievements, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadAchievements))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadAchievements"))
			{
				DebugLog("Failed to load achievements, probably not connected to Google Play Game");
				_achievements.Clear();
				InvokeOnAchievementsLoadingFailed();
			}
		}
		internal static void _OnAchievementsLoaded(string status)
		{
			_achievements.Clear();
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnAchievementsLoadingFailed();
				return;
			}
			
			var achievements = Client.AndroidAdapter.Call<AndroidJavaObject>("getAchievements");
			if (achievements != null)
			{
				var achievementsCount = achievements.Call<int>("getCount");
				for (var i = 0; i < achievementsCount; ++i)
				{
					var achievement = achievements.Call<AndroidJavaObject>("get", i);
					if (achievement == null)
						break;
					_achievements.Add(new Achievement(achievement));
				}
			}
			InvokeOnAchievementsLoaded();
		}
		
		public static void ShowAchievements()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show achievements, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, ShowAchievements))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("showAchievements"))
				DebugLog("Failed to show achievements, probably not connected to Google Play Game");
		}
		
		public static void IncrementAchievement(string id, int steps)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to increment achievement, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => IncrementAchievement(id, steps)))
				return;
			
			if (!Client.IsConnected)
			{
				DebugLog("Failed to increment achievement, not connected to Google Play Game");
				return;
			}
			Client.GamesClient.Call("incrementAchievement", id, steps);
			foreach(var achievement in _achievements)
			{
				if (achievement.Id == id && steps > 0)
				{
					achievement.Increment(steps);
					InvokeOnAchievementChanged(achievement);
					break;
				}
			}
		}
		
		public static void RevealAchievement(string id)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to reveal achievement, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => RevealAchievement(id)))
				return;
			
			if (!Client.IsConnected)
			{
				DebugLog("Failed to reveal achievement, not connected to Google Play Game");
				return;
			}
			Client.GamesClient.Call("revealAchievement", id);
			foreach(var achievement in _achievements)
			{
				if (achievement.Id == id)
				{
					achievement.Reveal();
					InvokeOnAchievementChanged(achievement);
					break;
				}
			}
		}
		
		public static void UnlockAchievement(string id)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to unlock achievement, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => UnlockAchievement(id)))
				return;
			
			if (!Client.IsConnected)
			{
				DebugLog("Failed to unlock achievement, not connected to Google Play Game");
				return;
			}
			Client.GamesClient.Call("unlockAchievement", id);
			foreach(var achievement in _achievements)
			{
				if (achievement.Id == id)
				{
					achievement.Unlock();
					InvokeOnAchievementChanged(achievement);
					break;
				}
			}
		}
		#endregion
		
		#region Leaderboards
		public static void LoadLeaderboards()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load leaderboards, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadLeaderboards))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadLeaderboardMetadata"))
			{
				DebugLog("Failed to load leaderboards, probably not connected to Google Play Game");
				_leaderboards.Clear();
				InvokeOnLeaderboardsLoadingFailed();
			}
		}
		
		internal static void _OnLeaderboardMetadataLoaded(string status)
		{
			_leaderboards.Clear();
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnLeaderboardsLoadingFailed();
				return;
			}
			
			var leaderboards = Client.AndroidAdapter.Call<AndroidJavaObject>("getLeaderboards");
			if (leaderboards != null)
			{
				var leaderboardsCount = leaderboards.Call<int>("getCount");
				for (var i = 0; i < leaderboardsCount; ++i)
				{
					var leaderboard = leaderboards.Call<AndroidJavaObject>("get", i);
					if (leaderboard == null)
						break;
					_leaderboards.Add(new Leaderboard(leaderboard));
				}
			}
			InvokeOnLeaderboardsLoaded();
		}
		
		public static void ShowLeaderboards()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show leaderboards, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, ShowLeaderboards))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("showAllLeaderboards"))
				DebugLog("Failed to show leaderboards, probably not connected to Google Play Game");
		}
		
		public static void ShowLeaderboard(string leaderboardId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show leaderboard " + leaderboardId + ", Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, () => ShowLeaderboard(leaderboardId)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("showLeaderboard", leaderboardId))
				DebugLog("Failed to show leaderboard " + leaderboardId + ", probably not connected to Google Play Game");
		}
		
		public static void SubmitScore(string leaderboardId, long score)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to submit score, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => SubmitScore(leaderboardId, score)))
				return;
			
			if (!Client.IsConnected)
			{
				DebugLog("Failed to submit score, not connected to Google Play Game");
				return;
			}
			Client.GamesClient.Call("submitScore", leaderboardId, score);
		}
		
		public static void SubmitScore(string leaderboardId, TimeSpan timeSpan)
		{
			SubmitScore(leaderboardId, (long)timeSpan.TotalMilliseconds);
		}
		
		public static void LoadScores(string leaderboardId,
			LeaderboardSpan span,
			LeaderboardCollection collection,
			LeaderboardSeed seed,
			int maxResults = 25)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load score, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, () => LoadScores(leaderboardId, span, collection, seed, maxResults)))
				return;
			
			var method = (seed == LeaderboardSeed.Top) ? "loadTopScores" : "loadPlayerCenteredScores";
			if (!Client.AndroidAdapter.Call<bool>(method, leaderboardId, (int)span, (int)collection, maxResults))
			{
				DebugLog("Failed to load score, probably not connected to Google Play Game or another LoadScores request is in progress");
				InvokeOnLeaderboardScoresLoadingFailed();
				return;
			}
		}
		
		internal static void _OnLeaderboardScoresLoaded(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnLeaderboardScoresLoadingFailed();
				return;
			}
			
			Leaderboard leaderboard = null;
						
			var sources = Client.AndroidAdapter.Call<AndroidJavaObject>("getLeaderboardScoresSource");
			if (sources != null)
			{
				var sourcesCount = sources.Call<int>("getCount");
				for (var i = 0; i < sourcesCount; ++i)
				{
					var source = sources.Call<AndroidJavaObject>("get", i);
					if (source == null)
						continue;
					leaderboard = new Leaderboard(source);
					break;
				}
			}
			
			if (leaderboard == null)
			{
				InvokeOnLeaderboardScoresLoadingFailed();
				return;
			}
			
			var newLeaderboard = true;
			foreach(var l in _leaderboards)
			{
				if (l.Id == leaderboard.Id)
				{
					newLeaderboard = false;
					leaderboard = l;
					break;
				}
			}
			
			if (newLeaderboard)
			{
				_leaderboards.Add(leaderboard);
			}
			
			var span = (LeaderboardSpan)AndroidAdapter.Call<int>("getLeaderboardScoresSpan");
			var collection = (LeaderboardCollection)AndroidAdapter.Call<int>("getLeaderboardScoresCollection");
				
			leaderboard.Data = new LeaderboardData(span, collection);
			
			var scores = Client.AndroidAdapter.Call<AndroidJavaObject>("getLeaderboardScoresData");
			if (scores != null)
			{
				var scoresCount = scores.Call<int>("getCount");
				for (var i = 0; i < scoresCount; ++i)
				{
					var score = scores.Call<AndroidJavaObject>("get", i);
					if (score == null)
						continue;
					leaderboard.Data.Scores.Add(new LeaderboardScore(score));
					break;
				}
			}
			
			InvokeOnLeaderboardScoresLoaded(leaderboard);
		}
		#endregion
		
		#region Details
		public static void LoadGameDetails()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load game details, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadGameDetails))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadGame"))
			{
				DebugLog("Failed to load game details, probably not connected to Google Play Game");
				_details = new Game();
				InvokeOnGameDetailsLoadingFailed();
			}
		}
		
		private static void ReadGameDetails(AndroidJavaObject obj)
		{
			_details = new Game();
			if (obj == null)
				return;
			
			_details = new Game(obj);
		}
		
		internal static void _OnGamesLoaded(string status)
		{
			_details = new Game();
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnGameDetailsLoadingFailed();
				return;
			}
			
			var games = Client.AndroidAdapter.Call<AndroidJavaObject>("getGames");
			if (games != null)
			{
				var gamesCount = games.Call<int>("getCount");
				for (var i = 0; i < gamesCount; ++i)
				{
					var game = games.Call<AndroidJavaObject>("get", i);
					if (game == null)
						continue;
					ReadGameDetails(game);
					break;
				}
			}
			InvokeOnGameDetailsLoaded();
		}
		#endregion
		
#elif UNITY_IPHONE
		#region Achievements
		[DllImport ("__Internal")]
		private static extern void GpgLoadAchievements();
		
		[DllImport ("__Internal")]
		private static extern void GpgShowAchievements();
		
		[DllImport ("__Internal")]
		private static extern void GpgIncrementAchievement(string achievementId, int steps);
		
		[DllImport ("__Internal")]
		private static extern void GpgRevealAchievement(string achievementId);
		
		[DllImport ("__Internal")]
		private static extern void GpgUnlockAchievement(string achievementId);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetAchievementsCount();
		
		
		
		
		public static void LoadAchievements()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load achievements, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadAchievements))
				return;

			GpgLoadAchievements();
		}
		
		internal static void _OnAchievementsLoaded(string status)
		{
			_achievements.Clear();
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnAchievementsLoadingFailed();
				return;
			}
			
			var achievementsCount = GpgGetAchievementsCount();
			for (var i = 0; i < achievementsCount; ++i)
			{
				_achievements.Add(new Achievement(i));
			}
			InvokeOnAchievementsLoaded();
		}
		
		public static void ShowAchievements()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show achievements, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, ShowAchievements))
				return;

			GpgShowAchievements();
		}
		
		public static void IncrementAchievement(string id, int steps)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to increment achievement, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => IncrementAchievement(id, steps)))
				return;

			GpgIncrementAchievement(id, steps);
		}
		
		public static void RevealAchievement(string id)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to reveal achievement, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => RevealAchievement(id)))
				return;

			GpgRevealAchievement(id);
		}
		
		public static void UnlockAchievement(string id)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to unlock achievement, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => UnlockAchievement(id)))
				return;

			GpgUnlockAchievement(id);
		}
		#endregion

		#region Leaderboards
		[DllImport ("__Internal")]
		private static extern void GpgLoadLeaderboards();
		
		[DllImport ("__Internal")]
		private static extern void GpgShowLeaderboards();
		
		[DllImport ("__Internal")]
		private static extern void GpgShowLeaderboard(string leaderboardId);
		
		[DllImport ("__Internal")]
		private static extern void GpgSubmitScore(string leaderboardId, long score);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetLeaderboardsCount();
		
		[DllImport ("__Internal")]
		private static extern int GpgLoadScores(string leaderboardId, int span, int collection, int seed, int maxResults);
		
		public static void LoadLeaderboards()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load leaderboards, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadLeaderboards))
				return;

			GpgLoadLeaderboards();
		}

		internal static void _OnLeaderboardMetadataLoaded(string status)
		{
			_leaderboards.Clear();
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnLeaderboardsLoadingFailed();
				return;
			}
			
			var leaderboardsCount = GpgGetLeaderboardsCount();
			for (var i = 0; i < leaderboardsCount; ++i)
			{
				_leaderboards.Add(new Leaderboard(i));
			}
			InvokeOnLeaderboardsLoaded();
		}
		
		public static void ShowLeaderboards()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show leaderboards, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, ShowLeaderboards))
				return;

			GpgShowLeaderboards();
		}
		
		public static void ShowLeaderboard(string leaderboardId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show leaderboard " + leaderboardId + ", Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, () => ShowLeaderboard(leaderboardId)))
				return;

			GpgShowLeaderboard(leaderboardId);
		}
		
		public static void SubmitScore(string leaderboardId, long score)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to submit score, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, true, () => SubmitScore(leaderboardId, score)))
				return;
			
			GpgSubmitScore(leaderboardId, score);
		}
		
		public static void SubmitScore(string leaderboardId, TimeSpan timeSpan)
		{
			SubmitScore(leaderboardId, (long)timeSpan.TotalMilliseconds);
		}
		
		public static void LoadScores(string leaderboardId,
		                              LeaderboardSpan span,
		                              LeaderboardCollection collection,
		                              LeaderboardSeed seed,
		                              int maxResults = 25)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load score, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, () => LoadScores(leaderboardId, span, collection, seed, maxResults)))
				return;

			GpgLoadScores(leaderboardId, (int)span, (int)collection, (int)seed, maxResults);
		}

		internal static void _OnLeaderboardScoresLoaded(string status)
		{
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnLeaderboardScoresLoadingFailed();
				return;
			}

			var leaderboardId = "";
			Utils.TryGetStatusArg(status, 1, ref leaderboardId);
			if (string.IsNullOrEmpty(leaderboardId))
			{
				InvokeOnLeaderboardScoresLoadingFailed();
				return;
			}

			foreach(var leaderboard in Leaderboards)
			{
				if (leaderboard.Id == leaderboardId)
				{
					leaderboard.Data = new LeaderboardData();
					InvokeOnLeaderboardScoresLoaded(leaderboard);
					return;
				}
			}

			InvokeOnLeaderboardScoresLoadingFailed();		
		}
		#endregion
		
		#region Details
		public static void LoadGameDetails()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load game details, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadGameDetails))
				return;

			InvokeOnGameDetailsLoaded();
		}
		#endregion
#else
		#region Achievements
		public static void LoadAchievements()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void ShowAchievements()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void IncrementAchievement(string id, int steps)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void RevealAchievement(string id)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void UnlockAchievement(string id)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		#endregion
		
		#region Leaderboards
		public static void LoadLeaderboards()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void ShowLeaderboards()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void ShowLeaderboard(string leaderboardId)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void SubmitScore(string leaderboardId, long score)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		
		public static void SubmitScore(string leaderboardId, TimeSpan timeSpan)
		{
			SubmitScore(leaderboardId, (long)timeSpan.TotalMilliseconds);
		}
		
		public static void LoadScores(string leaderboardId,
		                              LeaderboardSpan span,
		                              LeaderboardCollection collection,
		                              LeaderboardSeed seed,
		                              int maxResults = 25)
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		#endregion
		
		#region Details
		public static void LoadGameDetails()
		{
			Debug.LogWarning("Google Play Games plugin currently supports only Android platform");
		}
		#endregion
#endif
	}
}

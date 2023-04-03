using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public enum LeaderboardSpan
	{
		Daily = 0,
		Weekly = 1,
		AllTime = 2,
	}
	
	public enum LeaderboardScoreOrder
	{
		SmallerIsBetter = 0,
		LargerIsBetter = 1,
	}
	
	public enum LeaderboardCollection
	{
		Public = 0,
		Social = 1,
	}
	
	public enum LeaderboardSeed
	{
		Top = 0,
		PlayerCentered = 1,
	}


	public class LeaderboardScore
	{
		public long Rank { get; private set; }
		public long Score { get; private set; }
		public string FormattedRank { get; private set; }
		public string FormattedScore { get; private set; }

		public string ScoreHolderName { get; private set; }
		public Player ScoreHolder { get; private set; }
		public string ScoreHolderIconImageUri { get; private set; }
		public string ScoreHolderHiResImageUri { get; private set; }

		public DateTime Timestamp { get; private set; }
		
#if UNITY_ANDROID
		internal LeaderboardScore(AndroidJavaObject obj)
		{
			Rank = obj.Call<long>("getRank");
			Score = obj.Call<long>("getRawScore");

			FormattedRank = obj.Call<string>("getDisplayRank");
			FormattedScore = obj.Call<string>("getDisplayScore");

			ScoreHolderName = obj.Call<string>("getScoreHolderDisplayName");
			ScoreHolder = new Player(obj.Call<AndroidJavaObject>("getScoreHolder"));

			Timestamp = Utils.TimeStampToDateTime(obj.Call<long>("getTimestampMillis"));

			try
			{
				ScoreHolderIconImageUri = string.Empty;
				var uri = obj.Call<AndroidJavaObject>("getScoreHolderIconImageUri");
				if (uri != null)
					ScoreHolderIconImageUri = uri.Call<string>("toString");
			}
			catch
			{
				ScoreHolderIconImageUri = string.Empty;
			}
			
			try
			{
				ScoreHolderHiResImageUri = string.Empty;
				var uri = obj.Call<AndroidJavaObject>("getScoreHolderHiResImageUri");
				if (uri != null)
					ScoreHolderHiResImageUri = uri.Call<string>("toString");
			}
			catch
			{
				ScoreHolderHiResImageUri = string.Empty;
			}
		}
#endif

#if UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern long GpgGetScoreValue(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetScoreAvatarUrl(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetScoreFormattedRank(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetScoreFormattedScore(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetScoreDisplayName(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetScorePlayerId(int index);
		
		[DllImport ("__Internal")]
		private static extern long GpgGetScoreRank(int index);
		
		[DllImport ("__Internal")]
		private static extern long GpgGetScoreWriteTimestamp(int index);
		
		internal LeaderboardScore(int index)
		{
			Rank = GpgGetScoreRank(index);
			Score = GpgGetScoreValue(index);
			FormattedRank = GpgGetScoreFormattedRank(index);
			FormattedScore = GpgGetScoreFormattedScore(index);
			
			ScoreHolderName = GpgGetScoreDisplayName(index);
			ScoreHolder = new Player(GpgGetScorePlayerId(index), ScoreHolderName);
			ScoreHolderIconImageUri = ScoreHolderHiResImageUri = GpgGetScoreAvatarUrl(index);

			Timestamp = Utils.TimeStampToDateTime(GpgGetScoreWriteTimestamp(index));
		}
#endif
	}

	public class LeaderboardData
	{
		public LeaderboardSpan Span { get; private set; }
		public LeaderboardCollection Collection { get; private set; }
		public IList<LeaderboardScore> Scores { get; private set; }
		
		internal LeaderboardData(LeaderboardSpan span, LeaderboardCollection collection)
		{
			Span = span;
			Collection = collection;
			Scores = new List<LeaderboardScore>();
		}

#if UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern int GpgGetScoresCount();
		
		[DllImport ("__Internal")]
		private static extern int GpgGetScoresCollection();
		
		[DllImport ("__Internal")]
		private static extern int GpgGetScoresSeed();
		
		[DllImport ("__Internal")]
		private static extern int GpgGetScoresSpan();

		internal LeaderboardData()
		{
			Span = (LeaderboardSpan)GpgGetScoresSpan();
			Collection = (LeaderboardCollection)GpgGetScoresCollection();
			Scores = new List<LeaderboardScore>();

			var scoresCount = GpgGetScoresCount();
			for (var i = 0; i < scoresCount; ++i)
			{
				Scores.Add(new LeaderboardScore(i));
			}
		}
#endif
	}
	
	public class Leaderboard
	{
		public string Id { get; private set; }
		public string DisplayName { get; private set; }
		public string IconImageUri { get; private set; }
		public LeaderboardScoreOrder ScoreOrder { get; private set; }
		public LeaderboardData Data { get; internal set; }
		
		public override string ToString()
		{
			var result = "[Leaderboard " + DisplayName;
			result += ", " + ScoreOrder;
			result += "]";
			return result;
		}
		
#if UNITY_ANDROID
		internal Leaderboard(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getLeaderboardId");
			DisplayName = obj.Call<string>("getDisplayName");
			ScoreOrder = (LeaderboardScoreOrder)obj.Call<int>("getScoreOrder");

			try
			{
				IconImageUri = string.Empty;
				var iconImageUri = obj.Call<AndroidJavaObject>("getIconImageUri");
				if (iconImageUri != null)
					IconImageUri = iconImageUri.Call<string>("toString");
			}
			catch
			{
				IconImageUri = string.Empty;
			}
		}
#endif

#if UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern string GpgGetLeaderboardId(int index);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetLeaderboardScoreOrder(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetLeaderboardTitle(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetLeaderboardIconImageUrl(int index);
		
		internal Leaderboard(int index)
		{
			Id = GpgGetLeaderboardId(index);
			DisplayName = GpgGetLeaderboardTitle(index);
			ScoreOrder = (GpgGetLeaderboardScoreOrder(index) == 0) ? LeaderboardScoreOrder.LargerIsBetter : LeaderboardScoreOrder.SmallerIsBetter;
			IconImageUri = GpgGetLeaderboardIconImageUrl(index);
		}
#endif
	}
}

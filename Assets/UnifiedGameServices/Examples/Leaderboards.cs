using UnityEngine;
using System.Collections;

public class Leaderboards : Example
{
	public string ScoreLeaderboardId = "";
	public string TimeLeaderboardId = "";
	public string MoneyLeaderboardId = "";
	
	void Start()
	{
		Ugs.Config.AppStateEnabled = false;
		Ugs.Config.GamesEnabled = true;
		
		InitConnectionCallbacks();
		
		Ugs.Game.OnLeaderboardsLoaded += () =>
		{
			Debug.Log("Leaderboards loaded:");
			foreach(var leaderboard in Ugs.Game.Leaderboards)
				Debug.Log("  " + leaderboard);
		};
		
		Ugs.Game.OnLeaderboardsLoadingFailed += () =>
		{
			Debug.LogWarning("Leaderboards loading failed");
		};
		
		Ugs.Game.OnLeaderboardScoresLoaded += (leaderboard) =>
		{
			Debug.Log("Leaderboard Scores loaded for " + leaderboard.DisplayName +
				" [" + leaderboard.Data.Span + ", " + leaderboard.Data.Collection + "]");
			foreach(var score in leaderboard.Data.Scores)
				Debug.Log("  [" + score.Rank + "] " + score.ScoreHolderName + " - " + score.Score);
		};
		
		Ugs.Game.OnLeaderboardScoresLoadingFailed += () =>
		{
			Debug.LogWarning("Leaderboard scores loading failed");
		};
	}
	
	void OnGUI()
	{
		BeginGUI();
		
		if (!Ugs.Client.IsConnected)
		{
			LoginScreen();
		}
		else
		{
			LeaderboardsScreen();
		}
		
		EndGUI();
	}
	
	private void LeaderboardsScreen()
	{
		if (GUILayout.Button("Load Leaderboards"))
		{
			Ugs.Game.LoadLeaderboards();
		}
		
		if (GUILayout.Button("Show All Leaderboards"))
		{
			Ugs.Game.ShowLeaderboards();
		}
		
		if (ScoreLeaderboardId.Trim() != "")
		{
			BeginButtonsRow(3);
		
			if (GUILayout.Button("Submit Score"))
			{
				long score = 1 + (long)(100.0f / Random.Range(0.01f, 100.0f));
				Debug.Log("Submitting score: " + score);
				Ugs.Game.SubmitScore(ScoreLeaderboardId.Trim(), score);
			}
			
			if (GUILayout.Button("Show"))
			{
				Ugs.Game.ShowLeaderboard(ScoreLeaderboardId.Trim());
			}
			
			if (GUILayout.Button("Load Top"))
			{
				Ugs.Game.LoadScores(ScoreLeaderboardId.Trim(),
					Ugs.LeaderboardSpan.Weekly,
					Ugs.LeaderboardCollection.Public,
					Ugs.LeaderboardSeed.Top);
			}
		
			EndButtonsRow();
		}
		
		if (TimeLeaderboardId.Trim() != "")
		{
			BeginButtonsRow(3);
			
			if (GUILayout.Button("Submit Time"))
			{
				long time = (long)Mathf.Sqrt(Random.Range(0.001f, 100.0f)) * 10000;
				Debug.Log("Submitting time: " + time);
				Ugs.Game.SubmitScore(TimeLeaderboardId.Trim(), time);
			}
			if (GUILayout.Button("Show"))
			{
				Ugs.Game.ShowLeaderboard(TimeLeaderboardId.Trim());
			}
			if (GUILayout.Button("Load Top"))
			{
				Ugs.Game.LoadScores(TimeLeaderboardId.Trim(),
					Ugs.LeaderboardSpan.Weekly,
					Ugs.LeaderboardCollection.Public,
					Ugs.LeaderboardSeed.Top);
			}
			
			EndButtonsRow();
		}
		
		
		if (MoneyLeaderboardId.Trim() != "")
		{
			BeginButtonsRow(3);
			
			if (GUILayout.Button("Submit Money"))
			{
				long money = 1 + (long)(100.0f / Random.Range(0.01f, 100.0f)) * 1000000;
				Debug.Log("Submitting money: " + money);
				Ugs.Game.SubmitScore(MoneyLeaderboardId.Trim(), money);
			}
			if (GUILayout.Button("Show"))
			{
				Ugs.Game.ShowLeaderboard(MoneyLeaderboardId.Trim());
			}
			if (GUILayout.Button("Load Top"))
			{
				Ugs.Game.LoadScores(MoneyLeaderboardId.Trim(),
					Ugs.LeaderboardSpan.Weekly,
					Ugs.LeaderboardCollection.Public,
					Ugs.LeaderboardSeed.Top);
			}
			
			EndButtonsRow();
		}
		
		if (GUILayout.Button("Disconnect"))
		{
			Ugs.Client.Disconnect();
		}
		
		if (GUILayout.Button("Sign Out"))
		{
			Ugs.Client.SignOut();
		}
	}
}

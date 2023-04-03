using UnityEngine;
using System.Collections;

public class Achievements : Example
{
	public string RegularAchievementId = "";
	public string HiddenAchievementId = "";
	public string IncrementalAchievementId = "";
	
	void Start()
	{
		Ugs.Config.AppStateEnabled = false;
		Ugs.Config.GamesEnabled = true;
		
		InitConnectionCallbacks();
		
		Ugs.Game.OnAchievementsLoaded += () =>
		{
			Debug.Log("Achievements loaded:");
			foreach(var achievement in Ugs.Game.Achievements)
				Debug.Log("  " + achievement);
		};
		
		Ugs.Game.OnAchievementsLoadingFailed += () =>
		{
			Debug.LogWarning("Achievements loading failed");
		};
		
		Ugs.Game.OnAchievementChanged += (achievement) =>
		{
			Debug.Log("Achievement changed: " + achievement);
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
			AchievementsScreen();
		}
		
		EndGUI();
	}
	
	void AchievementsScreen()
	{
		if (GUILayout.Button("Show Achievements"))
		{
			Ugs.Game.ShowAchievements();
		}
		
		if (GUILayout.Button("Load Achievements"))
		{
			Ugs.Game.LoadAchievements();
		}
		
		if (HiddenAchievementId.Trim() != "" && GUILayout.Button("Reveal Achievement"))
		{
			Ugs.Game.RevealAchievement(HiddenAchievementId.Trim());
		}
		
		if (RegularAchievementId.Trim() != "" && GUILayout.Button("Unlock Achievement"))
		{
			Ugs.Game.UnlockAchievement(RegularAchievementId.Trim());
		}
		
		if (IncrementalAchievementId.Trim() != "" && GUILayout.Button("Increment Achievement"))
		{
			Ugs.Game.IncrementAchievement(IncrementalAchievementId.Trim(), 1);
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

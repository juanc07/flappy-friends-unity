using UnityEngine;
using System.Collections;

public class GameDetails : Example
{
	void Start()
	{
		Ugs.Config.AppStateEnabled = false;
		Ugs.Config.GamesEnabled = true;
		
		InitConnectionCallbacks();
		
		Ugs.Game.OnGameDetailsLoaded += () =>
		{
			var iconUri = Ugs.Game.Details.HiResImageUri;
			Debug.Log("Loading game icon from: " + iconUri);
			Ugs.Resources.LoadImage(iconUri, (texture) => gameIcon = texture );
		};
	}
	
	private Texture2D gameIcon;
	
	void OnGUI()
	{
		BeginGUI();
		
		if (!Ugs.Client.IsConnected)
		{
			LoginScreen();
		}
		else
		{
			GameDetailsScreen();
		}
		
		EndGUI();
	}
	
	private void GameDetailsScreen()
	{
		GUILayout.Label("Game: " + Ugs.Game.Details.DisplayName);
		
		if (GUILayout.Button("Disconnect"))
		{
			Ugs.Client.Disconnect();
		}
		
		if (GUILayout.Button("Sign Out"))
		{
			Ugs.Client.SignOut();
		}
		
		if (gameIcon != null)
		{
			var iconSize = Mathf.Min(Screen.width, Screen.height - 3 * GUI.skin.button.fixedHeight);
			GUI.DrawTexture(
				new Rect(Screen.width / 2 - iconSize / 2, Screen.height - iconSize, iconSize, iconSize),
				gameIcon,
				ScaleMode.ScaleToFit,
				false);
		}
	}
}

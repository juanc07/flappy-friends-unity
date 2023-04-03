using UnityEngine;
using System.Collections;

public class CloudSave : Example
{
	private string statesText = "";
	
	void UpdateStateText()
	{
		statesText = "";
		if (!Ugs.Client.IsConnected)
			return;
		foreach(var state in Ugs.CloudSave.States)
		{
			statesText += "State[" + state.Key + "] = " + state.StringData + "\n";
		}
		if (statesText == "")
			statesText = "States list is empty";
	}
	
	void Start()
	{
		Ugs.Config.AppStateEnabled = true;
		Ugs.Config.GamesEnabled = false;
		
		InitConnectionCallbacks();
		
		Ugs.CloudSave.OnStatesLoaded += () =>
		{
			Debug.Log("States loaded:");
			foreach(var state in Ugs.CloudSave.States)
				Debug.Log("  " + state + ", data: " + state.StringData);
			UpdateStateText();
		};
		
		Ugs.CloudSave.OnStatesLoadingFailed += () =>
		{
			Debug.LogWarning("States loading failed");
		};
		
		Ugs.CloudSave.OnStateSaved += (state) =>
		{
			Debug.Log("State saved: " + state + ", data: " + state.StringData);
			UpdateStateText();
		};
		
		Ugs.CloudSave.OnStateSavingFailed += (stateKey) =>
		{
			Debug.Log("State " + stateKey + " save failed");
		};
		
		Ugs.CloudSave.OnStateDeleted += (stateKey) =>
		{
			Debug.Log("State " + stateKey + " deleted");
			UpdateStateText();
		};
		
		Ugs.CloudSave.OnStateDeletionFailed += (stateKey) =>
		{
			Debug.Log("State " + stateKey + "deletion failed");
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
			CloudSaveScreen();
		}
		
		EndGUI();
	}
	
	private void CloudSaveScreen()
	{
		GUILayout.Label("States:");
		GUILayout.TextField(statesText);
		
		if (GUILayout.Button("Load States"))
		{
			Ugs.CloudSave.LoadStates();
		}

		GUILayout.Label("Save state:");
		BeginButtonsRow(4);
		if (GUILayout.Button("0"))
			Ugs.CloudSave.SaveState(0, System.DateTime.Now.ToString());
		if (GUILayout.Button("1"))
			Ugs.CloudSave.SaveState(1, System.DateTime.Now.ToString());
		if (GUILayout.Button("2"))
			Ugs.CloudSave.SaveState(2, System.DateTime.Now.ToString());
		if (GUILayout.Button("3"))
			Ugs.CloudSave.SaveState(3, System.DateTime.Now.ToString());
		EndButtonsRow();

		GUILayout.Label("Delete state:");
		BeginButtonsRow(4);
		if (GUILayout.Button("0"))
			Ugs.CloudSave.DeleteState(0);
		if (GUILayout.Button("1"))
			Ugs.CloudSave.DeleteState(1);
		if (GUILayout.Button("2"))
			Ugs.CloudSave.DeleteState(2);
		if (GUILayout.Button("3"))
			Ugs.CloudSave.DeleteState(3);
		EndButtonsRow();

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

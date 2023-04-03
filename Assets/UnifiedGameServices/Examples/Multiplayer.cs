using UnityEngine;
using System;
using System.Collections.Generic;

public class Multiplayer : Example
{
	private Dictionary<Ugs.Participant, int> othersScores = new Dictionary<Ugs.Participant, int>();
	private int myScore = 0;
	
	void Start()
	{
		Ugs.Config.AppStateEnabled = false;
		Ugs.Config.GamesEnabled = true;
		
		InitConnectionCallbacks();
		
		Ugs.Multiplayer.OnInvitationReceived += (invitation) =>
		{
			Debug.Log("Gpg invitation received: " + invitation);
		};
		
		Ugs.Multiplayer.OnRoomCreated += ResetRoom;
		Ugs.Multiplayer.OnRoomJoined += ResetRoom;
	}
	
	private void ResetRoom(Ugs.Room room)
	{
		Debug.Log ("Gpg entered room " + room);
		
		othersScores.Clear();
		myScore = 0;
		
		room.OnPeerConnected += (peer) =>
		{
			othersScores.Add(peer, 0);
			room.SendUnreliableMessage(peer.Id, BitConverter.GetBytes(myScore));
		};
		
		room.OnUnreliableMessageReceived += (sender, message) =>
		{
			Debug.Log ("Gpg received message from " + sender);
			othersScores[sender] = BitConverter.ToInt32(message.Data, 0);
		};
		
		room.SendUnreliableMessageToAll(BitConverter.GetBytes(myScore));
	}
	
	void OnGUI()
	{
		BeginGUI();
		
		if (!Ugs.Client.IsConnected)
		{
			LoginScreen();
		}
		else if (Ugs.Multiplayer.Room == null)
		{
			MenuScreen();
		}
		else if (Ugs.Multiplayer.Room.Status != Ugs.RoomStatus.Active)
		{
			LobbyScreen();
		}
		else
		{
			GameScreen();
		}
		
		EndGUI();
	}
	
	private void MenuScreen()
	{
		if (GUILayout.Button("Quick 1v1 Game"))
		{
			Ugs.Multiplayer.QuickGame(1, 1);
		}
		if (GUILayout.Button("Show Players Select"))
		{
			Ugs.Multiplayer.PickPlayers(1, 3);
		}
		if (GUILayout.Button("Pick Invitation"))
		{
			Ugs.Multiplayer.PickInvitation();
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
	
	private void LobbyScreen()
	{
		if (GUILayout.Button("Show Waiting Room"))
		{
			Ugs.Multiplayer.ShowWaitingRoom(1);
		}
		if (GUILayout.Button("Leave Room"))
		{
			Ugs.Multiplayer.LeaveRoom();
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
	
	private void GameScreen()
	{
		var message = "Scores:\n\n  Me : " + myScore;
		foreach(var s in othersScores)
		{
			message += "\n  " + s.Key.DisplayName + "(" + s.Key.Status + ") : " + s.Value;
		}
		GUILayout.TextField(message);
		
		if (GUILayout.Button("Add Score"))
		{
			myScore += 1 + (int)(UnityEngine.Random.value * 10);
			Ugs.Multiplayer.Room.SendUnreliableMessageToAll(BitConverter.GetBytes(myScore));
		}
		
		if (GUILayout.Button("Leave Room"))
		{
			Ugs.Multiplayer.LeaveRoom();
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

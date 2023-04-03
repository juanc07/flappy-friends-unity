using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public static class Multiplayer
	{
		#region Logging
		private static void DebugLog(string message)
		{
			if (!Config.Verbose)
				return;
			
			Debug.Log("[Gpg.Multiplayer] " + message);
		}
		#endregion
		
		
		#region Callbacks
		public static event Action<Invitation> OnInvitationReceived;
		private static void InvokeOnInvitationReceived(Invitation invitation)
		{
			var c = OnInvitationReceived;
			if (c != null)
				c(invitation);
		}
		
		public static event Action<Invitation> OnInvitationSelected;
		private static void InvokeOnInvitationSelected(Invitation invitation)
		{
			var c = OnInvitationSelected;
			if (c != null)
				c(invitation);
		}		
		
		public static event Action OnInvitationsLoadingFailed;
		private static void InvokeOnInvitationsLoadingFailed()
		{
			var c = OnInvitationsLoadingFailed;
			if (c != null)
				c();
		}
		
		public static event Action OnInvitationsLoaded;
		private static void InvokeOnInvitationsLoaded()
		{
			var c = OnInvitationsLoaded;
			if (c != null)
				c();
		}
		
		public static event Action OnPlayersLoadingFailed;
		private static void InvokeOnPlayersLoadingFailed()
		{
			var c = OnPlayersLoadingFailed;
			if (c != null)
				c();
		}
		
		public static event Action OnPlayersLoaded;
		private static void InvokeOnPlayersLoaded()
		{
			var c = OnPlayersLoaded;
			if (c != null)
				c();
		}
		
		public static event Action OnPlayersSelectionFailed;
		private static void InvokeOnPlayersSelectionFailed()
		{
			var c = OnPlayersSelectionFailed;
			if (c != null)
				c();
		}
		
		public static event Action<List<string>, int, int> OnPlayersSelected;
		private static void InvokeOnPlayersSelected(List<string> players, int minAutoMatchPlayers, int maxAutoMatchPlayers)
		{
			var c = OnPlayersSelected;
			if (c != null)
				c(players, minAutoMatchPlayers, maxAutoMatchPlayers);
		}
		
		public static event Action<Room> OnRoomCreated;
		private static void InvokeOnRoomCreated(Room room)
		{
			var c = OnRoomCreated;
			if (c != null)
				c(room);
		}
		
		public static event Action<Room> OnRoomJoined;
		private static void InvokeOnRoomJoined(Room room)
		{
			var c = OnRoomJoined;
			if (c != null)
				c(room);
		}
		
		public static event Action<Room> OnRoomLeft;
		private static void InvokeOnRoomLeft(Room room)
		{
			var c = OnRoomLeft;
			if (c != null)
				c(room);
		}
		
		public static event Action<Room> OnRoomConnected;
		private static void InvokeOnRoomConnected(Room room)
		{
			var c = OnRoomConnected;
			if (c != null)
				c(room);
		}
		
		public static event Action OnWaitingRoomFinished;
		private static void InvokeOnWaitingRoomFinished()
		{
			var c = OnWaitingRoomFinished;
			if (c != null)
				c();
		}
		
		public static event Action OnWaitingRoomCanceled;
		private static void InvokeOnWaitingRoomCanceled()
		{
			var c = OnWaitingRoomCanceled;
			if (c != null)
				c();
		}
		#endregion
		
		public static IEnumerable<Invitation> Invitations { get { return _invitations; } }
		public static IEnumerable<Player> Players { get { return _players; } }
		public static Room Room { get { return _rooms.Count == 0 ? null : _rooms[_rooms.Count - 1]; } }
		public static IEnumerable<Room> Rooms { get { return _rooms; } }
		
		private static List<Invitation> _invitations = new List<Invitation>();
		private static List<Player> _players = new List<Player>();
		private static List<Room> _rooms = new List<Room>();
		
		internal static void Clear()
		{
			_invitations.Clear();
			_players.Clear();
			_rooms.Clear();
		}
		
#if UNITY_ANDROID
		internal static void _OnInvitationReceived(string status)
		{
			DebugLog("_OnInvitationReceived: " + status);
			
			if (!Utils.IsStatusOk(status))	
				return;
			
			var invitation = Client.AndroidAdapter.Call<AndroidJavaObject>("getReceivedInvitation");
			if (invitation == null)
				return;
			
			InvokeOnInvitationReceived(new Invitation(invitation));
		}
		
		private static Action<Invitation> _onInvitaionSelected;
		
		public static void ShowInvitations()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show invitations, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, ShowInvitations))
				return;
			
			_onInvitaionSelected = (invitation) => InvokeOnInvitationSelected(invitation);
			
			if (!Client.AndroidAdapter.Call<bool>("showInvitationInbox"))
			{
				DebugLog("Failed to show invitations, probably not connected to Google Play Game");
			}
		}
		
		public static void PickInvitation()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show invitations, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, PickInvitation))
				return;
			
			_onInvitaionSelected = (invitation) => JoinRoom(invitation.Id);
			
			if (!Client.AndroidAdapter.Call<bool>("showInvitationInbox"))
			{
				DebugLog("Failed to show invitations, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnInvitationSelected(string status)
		{
			if (!Utils.IsStatusOk(status))	
				return;
			
			var invitation = Client.AndroidAdapter.Call<AndroidJavaObject>("getSelectedInvitation");
			if (invitation == null)
				return;
			
			if (_onInvitaionSelected != null)
				_onInvitaionSelected(new Invitation(invitation));
		}
		
		public static void LoadInvitations()
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load invitations, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, LoadInvitations))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadInvitations"))
			{
				DebugLog("Failed to load invitations, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnInvitationsLoaded(string status)
		{
			_invitations.Clear();
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnInvitationsLoadingFailed();
				return;
			}
			
			var invitations = Client.AndroidAdapter.Call<AndroidJavaObject>("getInvitations");
			if (invitations != null)
			{
				var invitationsCount = invitations.Call<int>("getCount");
				for (var i = 0; i < invitationsCount; ++i)
				{
					var invitation = invitations.Call<AndroidJavaObject>("get", i);
					if (invitation == null)
						break;
					_invitations.Add(new Invitation(invitation));
				}
			}
			InvokeOnInvitationsLoaded();
		}
		
		public static void DeclineInvitation(string invitationId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to decline invitation, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (!Client.AndroidAdapter.Call<bool>("declineRoomInvitation", invitationId))
			{
				DebugLog("Failed to decline invitation, probably not connected to Google Play Game");
			}
		}
		
		public static void DismissInvitation(string invitationId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to dismiss invitation, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (!Client.AndroidAdapter.Call<bool>("dismissRoomInvitation", invitationId))
			{
				DebugLog("Failed to dismiss invitation, probably not connected to Google Play Game");
			}
		}
		
		
		private static Action<List<string>, int, int> _onPlayersSelected;
		
		public static void PickPlayers(int minPlayers, int maxPlayers, long exclusiveMask = 0)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show players selection, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, () => PickPlayers(minPlayers, maxPlayers)))
				return;
			
			_onPlayersSelected += (players, minAuto, maxAuto) => CreateRoom(players, minAuto, maxAuto, exclusiveMask);
			
			if (!Client.AndroidAdapter.Call<bool>("showSelectPlayers", minPlayers, maxPlayers))
			{
				DebugLog("Failed to show players selection, probably not connected to Google Play Game or another LoadScores request is in progress");
			}
		}
		
		public static void ShowPlayersSelect(int minPlayers, int maxPlayers)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show players selection, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(false, false, () => ShowPlayersSelect(minPlayers, maxPlayers)))
				return;
			
			_onPlayersSelected += (players, minAuto, maxAuto) => InvokeOnPlayersSelected(players, minAuto, maxAuto);
			
			if (!Client.AndroidAdapter.Call<bool>("showSelectPlayers", minPlayers, maxPlayers))
			{
				DebugLog("Failed to show players selection, probably not connected to Google Play Game or another LoadScores request is in progress");
			}
		}
		
		internal static void _OnPlayersSelected(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				InvokeOnPlayersSelectionFailed();
				return;
			}
			
			int minAutoMatchPlayers = 0;
			int maxAutoMatchPlayers = 0;
			
			if (!Utils.TryGetStatusArg(status, 1, ref minAutoMatchPlayers))
				minAutoMatchPlayers = 0;
			if (!Utils.TryGetStatusArg(status, 2, ref maxAutoMatchPlayers))
				maxAutoMatchPlayers = 0;
			
			if (_onPlayersSelected != null)
				_onPlayersSelected(
					Client.AndroidAdapter.Call<AndroidJavaObject>("getSelectedPlayers").AsStringsList(),
					minAutoMatchPlayers,
					maxAutoMatchPlayers);
		}
		
		public static void LoadPlayers(int pageSize = 25, bool forceReload = true)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load players, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => LoadPlayers(pageSize, forceReload)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadInvitablePlayers", pageSize, forceReload))
			{
				DebugLog("Failed to load players, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnInvitablePlayersLoaded(string status)
		{
			_players.Clear();
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnPlayersLoadingFailed();
				return;
			}
			
			_players.AddArray(Client.AndroidAdapter.Call<AndroidJavaObject>("getInvitablePlayers"));
			InvokeOnPlayersLoaded();
		}
		
		public static void LoadMorePlayers(int pageSize = 25)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to load more players, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => LoadMorePlayers(pageSize)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadMoreInvitablePlayers", pageSize))
			{
				DebugLog("Failed to load more players, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnMoreInvitablePlayersLoaded(string status)
		{
			_players.Clear();
			if (!Utils.IsStatusOk(status))	
			{
				InvokeOnPlayersLoadingFailed();
				return;
			}
			
			_players.AddArray(Client.AndroidAdapter.Call<AndroidJavaObject>("getInvitablePlayers"));
			InvokeOnPlayersLoaded();
		}
		
		
		public static void QuickGame(int minAutoMatchPlayers, int maxAutoMatchPlayers, long exclusiveBitMask = 0)
		{
			CreateRoom(null, minAutoMatchPlayers, maxAutoMatchPlayers, exclusiveBitMask);
		}
		
		public static void CreateRoom(IEnumerable<string> players, int minAutoMatchPlayers, int maxAutoMatchPlayers, long exclusiveBitMask = 0)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to create room, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => CreateRoom(players, minAutoMatchPlayers, maxAutoMatchPlayers, exclusiveBitMask)))
				return;
			
			var count = 0;
			if (players != null)
			{
				foreach(var p in players)
				{
					if (p != null)
						count++;
					else
						count++;
				}
			}
			var playersArray = new string[count];
			if (players != null)
			{
				var i = 0;
				foreach(var p in players)
				{
					if (i < count)
						playersArray[i] = p ?? "";
					++i;
				}
			}
				
			if (!Client.AndroidAdapter.Call<bool>("createRoom", minAutoMatchPlayers, maxAutoMatchPlayers, exclusiveBitMask, playersArray))
			{
				DebugLog("Failed to create room, probably not connected to Google Play Game");
			}
		}
		
		private static Room FindRoom(string roomId)
		{
			foreach(var r in _rooms)
			{
				if (r.Id == roomId)
				{
					return r;
				}
			}
			return null;
		}
		
		private static Room FindRoom(Room room)
		{
			foreach(var r in _rooms)
			{
				if (r.Id == room.Id)
				{
					r.InitFrom(room);
					return r;
				}
			}
			return room;
		}
		
		private static Room AddRoom(Room room)
		{
			if (room == null)
				return null;
			
			foreach(var r in _rooms)
			{
				if (r.Id == room.Id)
				{
					r.InitFrom(room);
					return r;
				}
			}
			
			_rooms.Add(room);
			return room;
		}
		
		private static void RemoveRoom(Room room)
		{
			if (room != null)
				RemoveRoom(room.Id);
		}
		
		private static void RemoveRoom(string roomId)
		{
			foreach(var r in _rooms)
			{
				if (r.Id == roomId)
				{
					_rooms.Remove(r);
					InvokeOnRoomLeft(r);
					return;
				}
			}
		}
		
		internal static void _OnRoomCreated(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				string statusCode = "";
				Utils.TryGetStatusArg(status, 0, ref statusCode);
				DebugLog("Failed to create room, statusCode: " + statusCode);
				return;
			}
			
			var room = GetRoom("getCreatedRoom");
			DebugLog("Room created: " + room);
			
			AddRoom(room);
			InvokeOnRoomCreated(room);
		}
		
		public static void JoinRoom(Invitation invitation)
		{
			if (invitation != null)
				JoinRoom(invitation.Id);
		}
		
		public static void JoinRoom(string invitationId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to join room, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => JoinRoom(invitationId)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("joinRoom", invitationId))
			{
				DebugLog("Failed to join room, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnRoomJoined(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				string statusCode = "";
				Utils.TryGetStatusArg(status, 0, ref statusCode);
				DebugLog("Failed to join room, statusCode: " + statusCode);
				return;
			}
			
			var room = GetRoom("getCreatedRoom");
			DebugLog("Room joined: " + room);
			
			AddRoom(room);
			InvokeOnRoomJoined(room);
		}
		
		public static void ShowWaitingRoom(int minParticipantsToStart)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to show waiting room, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => ShowWaitingRoom(minParticipantsToStart)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("showRealTimeWaitingRoom", minParticipantsToStart))
			{
				DebugLog("Failed to show waiting room, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnWaitingRoomFinished(string status)
		{
			var room = GetRoom("getRealtimeWaitingRoom");
			DebugLog("Room waiting finished: " + room);
			InvokeOnWaitingRoomFinished();
		}
		
		private static Room GetRoom(string getter)
		{
			var roomObj = Client.AndroidAdapter.Call<AndroidJavaObject>(getter);
			if (roomObj == null)
				return null;
			return new Room(roomObj);
		}
		
		internal static void _OnWaitingRoomCanceled(string status)
		{
			Room room = GetRoom("getRealtimeWaitingRoom");
			DebugLog("Room waiting canceled: " + room);
			InvokeOnWaitingRoomCanceled();
		}
		
		internal static void _OnWaitingRoomLeft(string status)
		{
			Room room = GetRoom("getRealtimeWaitingRoom");
			DebugLog("Room waiting left: " + room);
			if (room != null)
				LeaveRoom(room.Id);
		}
		
		internal static void _OnRoomConnected(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				string statusCode = "";
				Utils.TryGetStatusArg(status, 0, ref statusCode);
				DebugLog("Failed to connect room, statusCode: " + statusCode);
				return;
			}
			
			var room = GetRoom("getConnectedRoom");
			DebugLog("Room connected: " + room);
			
			AddRoom(room);
			InvokeOnRoomConnected(room);
		}
		
		public static void LeaveRoom()
		{
			LeaveRoom(Room);
		}
		
		public static void LeaveRoom(Room room)
		{
			if (room == null)
			{
				DebugLog("Can't leave null room");
				return;
			}
			
			LeaveRoom(room.Id);
		}
		
		public static void LeaveRoom(string roomId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to leave room, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => LeaveRoom(roomId)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("leaveRoom", roomId))
			{
				DebugLog("Failed to leave room, probably not connected to Google Play Game");
			}
		}
		
		internal static void _OnRoomLeft(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				string statusCode = "";
				Utils.TryGetStatusArg(status, 0, ref statusCode);
				DebugLog("Failed to leave room, statusCode: " + statusCode);
				return;
			}
			
			string roomId = "";
			if (!Utils.TryGetStatusArg(status, 1, ref roomId))
			{
				DebugLog("Failed to get left room id");
				return;
			}
			
			DebugLog("Room left: " + roomId);
			
			RemoveRoom(roomId);
		}
		
		
		
		
		private static List<string> GetStatusUpdatedParticipantIds()
		{
			return Client.AndroidAdapter.Call<AndroidJavaObject>("getStatusUpdatedParticipantIds").AsStringsList();
		}
		
		internal static void _OnConnectedToRoom(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			room.InvokeOnConnectedToRoom();
		}
		
		internal static void _OnDisconnectedFromRoom(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			room.InvokeOnDisconnectedFromRoom();
		}
		
		internal static void _OnPeerDeclined(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			var participants = GetStatusUpdatedParticipantIds();
			room.InvokeOnPeerDeclined(participants);
		}
		
		internal static void _OnPeerInvitedToRoom(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			var participants = GetStatusUpdatedParticipantIds();
			room.InvokeOnPeerInvitedToRoom(participants);
		}
		
		internal static void _OnPeerJoined(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			var participants = GetStatusUpdatedParticipantIds();
			room.InvokeOnPeerJoined(participants);
		}
		
		internal static void _OnPeerLeft(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			var participants = GetStatusUpdatedParticipantIds();
			room.InvokeOnPeerLeft(participants);
		}
		
		internal static void _OnPeersConnected(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			var participants = GetStatusUpdatedParticipantIds();
			room.InvokeOnPeersConnected(participants);
		}
		
		internal static void _OnPeersDisconnected(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			var participants = GetStatusUpdatedParticipantIds();
			room.InvokeOnPeersDisconnected(participants);
		}
		
		internal static void _OnRoomAutoMatching(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			room.InvokeOnRoomAutoMatching();
		}
		
		internal static void _OnRoomConnecting(string status)
		{
			var room = FindRoom(GetRoom("getStatusUpdatedRoom"));
			room.InvokeOnRoomConnecting();
		}
		
		
		
		
		
		internal static void SendReliableMessage(byte [] message, string roomId, string participantId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to send reliable message, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (!Client.AndroidAdapter.Call<bool>("sendReliableRealTimeMessage", message, roomId, participantId))
			{
				DebugLog("Failed to send reliable message");
			}
		}
		
		internal static void _OnReliableMessageSent(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				string statusCode = "";
				Utils.TryGetStatusArg(status, 0, ref statusCode);
				DebugLog("Failed to send message, statusCode: " + statusCode);
				return;
			}
			
			string roomId = "";
			if (!Utils.TryGetStatusArg(status, 1, ref roomId))
			{
				DebugLog("Failed to get sent message roomId");
				return;
			}
			
			int tokenId = 0;
			if (!Utils.TryGetStatusArg(status, 2, ref tokenId))
			{
				DebugLog("Failed to get sent message tokenId");
				return;
			}
			
			string recipientParticipantId = "";
			if (!Utils.TryGetStatusArg(status, 3, ref recipientParticipantId))
			{
				DebugLog("Failed to get sent message recipient");
				return;
			}
			
			DebugLog("Reliable message (token: " + tokenId + ") sent to " + recipientParticipantId);
			
			var room = FindRoom(roomId);
			if (room != null)
				room.InvokeOnMessageSent(recipientParticipantId, tokenId);
		}
		
		internal static void SendUnreliableMessage(byte [] message, string roomId, string participantId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to send unreliable message, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (!Client.AndroidAdapter.Call<bool>("sendUnreliableRealTimeMessage", message, roomId, participantId))
			{
				DebugLog("Failed to send unreliable message");
			}
		}
		
		internal static void SendUnreliableMessage(byte [] message, string roomId, string[] participantIds)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to send unreliable message, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (!Client.AndroidAdapter.Call<bool>("sendUnreliableRealTimeMessage", message, roomId, participantIds))
			{
				DebugLog("Failed to send unreliable message");
			}
		}
		
		internal static void SendUnreliableMessageToAll(byte [] message, string roomId)
		{
			if (!Client.GamesCallsAllowed)
			{
				DebugLog("Failed to send unreliable message, Gpg.Config.GamesEnabled must be true");
				return;
			}
			
			if (!Client.AndroidAdapter.Call<bool>("sendUnreliableRealTimeMessageToAll", message, roomId))
			{
				DebugLog("Failed to send unreliable message");
			}
		}
		
		
		internal static void _OnMessageReceived(string status)
		{
			if (!Utils.IsStatusOk(status))
			{
				string statusCode = "";
				Utils.TryGetStatusArg(status, 0, ref statusCode);
				DebugLog("Failed to receive message, statusCode: " + statusCode);
				return;
			}
			
			string roomId = "";
			if (!Utils.TryGetStatusArg(status, 1, ref roomId))
			{
				DebugLog("Failed to get received message roomId");
				return;
			}
			
			var messageObj = Client.AndroidAdapter.Call<AndroidJavaObject>("getReceivedMessage");
			if (messageObj == null)
			{
				DebugLog("Failed to get received message");
				return;
			}
			
			var message = new RealTimeMessage(messageObj);
			var type = message.IsReliable ? "Reliable" : "Unreliable";
			DebugLog(type + " message received from " + message.SenderParticipantId);
			
			var room = FindRoom(roomId);
			if (room != null)
				room.InvokeOnMessageReceived(message);
		}
		
		
#endif
		
#if !UNITY_ANDROID
		public static void ShowInvitations()
		{
			
		}
		
		public static void PickInvitation()
		{
			
		}
				
		public static void LoadInvitations()
		{
			
		}
		
		public static void DeclineInvitation(string invitationId)
		{
			
		}
		
		public static void DismissInvitation(string invitationId)
		{
			
		}
		
		
		public static void PickPlayers(int minPlayers, int maxPlayers, long exclusiveMask = 0)
		{
			
		}
		
		public static void ShowPlayersSelect(int minPlayers, int maxPlayers)
		{
			
		}
		
		public static void LoadPlayers(int pageSize, bool forceReload)
		{
			
		}
				
		public static void LoadMorePlayers(int pageSize)
		{
			
		}
		
		public static void QuickGame(int minAutoMatchPlayers, int maxAutoMatchPlayers, long exclusiveBitMask = 0)
		{
			
		}
		
		public static void CreateRoom(IEnumerable<string> players, int minAutoMatchPlayers, int maxAutoMatchPlayers, long exclusiveBitMask = 0)
		{
			
		}
		
		public static void JoinRoom(Invitation invitation)
		{
			
		}
		
		public static void JoinRoom(string invitationId)
		{
			
		}
		
		public static void ShowWaitingRoom(int minParticipantsToStart)
		{
			
		}		
		
		public static void LeaveRoom()
		{
			
		}
		
		public static void LeaveRoom(Room room)
		{
			
		}
		
		public static void LeaveRoom(string roomId)
		{
			
		}
		
		
		
		
		
		
		public static void SendReliableMessage(byte [] message, string roomId, string participantId)
		{
			
		}
				
		public static void SendUnreliableMessage(byte [] message, string roomId, string participantId)
		{
			
		}
		
		public static void SendUnreliableMessage(byte [] message, string roomId, string[] participantIds)
		{
			
		}
		
		public static void SendUnreliableMessageToAll(byte [] message, string roomId)
		{
			
		}
#endif
	}
}

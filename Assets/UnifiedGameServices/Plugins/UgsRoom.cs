using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public enum RoomStatus
	{
		Inviting = 0,
		AutoMatching = 1,
		Connecting = 2,
		Active = 3,
	}
	
	public class Room
	{
		public Action OnConnectedToRoom;
		public Action OnDisconnectedFromRoom;
		public Action<Participant> OnPeerDeclined;
		public Action<Participant> OnPeerInvitedToRoom;
		public Action<Participant> OnPeerJoined;
		public Action<Participant> OnPeerLeft;
		public Action<Participant> OnPeerConnected;
		public Action<Participant> OnPeerDisconnected;
		public Action OnRoomAutoMatching;
		public Action OnRoomConnecting;
				
		public Action<Participant, int> OnReliableMessageSent;
		public Action<Participant, RealTimeMessage> OnReliableMessageReceived;
		public Action<Participant, RealTimeMessage> OnUnreliableMessageReceived;
		
		public string Id { get; private set; }
		public string CreatorId { get; private set; }
		public string Description { get; private set; }
		public RoomStatus Status { get; private set; }
		public int Variant { get; private set; }
		public List<Participant> Participants { get; private set; }
		
		public override string ToString()
		{
			var result = "[Room " + Id;
			result += ", Status = " + Status;
			var i = 0;
			var players = "";
			foreach(var p in Participants)
			{
				if (i > 0)
					players += ", ";
				players += p.DisplayName;
				++i;
			}
			if (i > 0)
				result += ", Players = (" + players +")";
			result += "]";
			return result;
		}
		
		public void SendReliableMessage(string participantId, string message)
		{
			SendReliableMessage(participantId, System.Text.Encoding.UTF8.GetBytes(message));
		}
		public void SendReliableMessage(string participantId, byte [] message)
		{
			Ugs.Multiplayer.SendReliableMessage(message, Id, participantId);
		}
		
		public void SendUnreliableMessage(string participantId, string message)
		{
			SendUnreliableMessage(participantId, System.Text.Encoding.UTF8.GetBytes(message));
		}
		public void SendUnreliableMessage(string participantId, byte [] message)
		{
			Ugs.Multiplayer.SendUnreliableMessage(message, Id, participantId);
		}
		
		public void SendUnreliableMessage(IEnumerable<string> participantIds, string message)
		{
			SendUnreliableMessage(participantIds, System.Text.Encoding.UTF8.GetBytes(message));
		}
		public void SendUnreliableMessage(IEnumerable<string> participantIds, byte [] message)
		{	if (participantIds == null)
				return;
			
			var count = 0;
			
			foreach(var p in participantIds)
			{
				if (p != null)
					count++;
				else
					count++;
			}
			var participantsArray = new string[count];
			var i = 0;
			foreach(var p in participantIds)
			{
				if (i < count)
					participantsArray[i] = p ?? "";
				++i;
			}
			Ugs.Multiplayer.SendUnreliableMessage(message, Id, participantsArray);
		}
		
		public void SendUnreliableMessageToAll(string message)
		{
			SendUnreliableMessageToAll(System.Text.Encoding.UTF8.GetBytes(message));
		}
		public void SendUnreliableMessageToAll(byte [] message)
		{
			Ugs.Multiplayer.SendUnreliableMessageToAll(message, Id);
		}
		
		internal Participant GetParticipantById(string participantId)
		{
			foreach(var p in Participants)
				if (p.Id == participantId)
					return p;
			return null;
		}
		
		internal void InvokeOnConnectedToRoom()
		{
			var c = OnConnectedToRoom;
			if (c != null)
				c();
		}
		
		internal void InvokeOnDisconnectedFromRoom()
		{
			var c = OnDisconnectedFromRoom;
			if (c != null)
				c();
		}
		
		internal void InvokeOnPeerDeclined(List<string> participantIds)
		{
			var c = OnPeerDeclined;
			if (c != null)
				foreach(var p in participantIds)
					c(GetParticipantById(p));
		}
		
		internal void InvokeOnPeerInvitedToRoom(List<string> participantIds)
		{
			var c = OnPeerInvitedToRoom;
			if (c != null)
				foreach(var p in participantIds)
					c(GetParticipantById(p));
		}
		
		internal void InvokeOnPeerJoined(List<string> participantIds)
		{
			var c = OnPeerJoined;
			if (c != null)
				foreach(var p in participantIds)
					c(GetParticipantById(p));
		}
		
		internal void InvokeOnPeerLeft(List<string> participantIds)
		{
			var c = OnPeerLeft;
			if (c != null)
				foreach(var p in participantIds)
					c(GetParticipantById(p));
		}
		
		internal void InvokeOnPeersConnected(List<string> participantIds)
		{
			var c = OnPeerConnected;
			if (c != null)
				foreach(var p in participantIds)
					c(GetParticipantById(p));
		}
		
		internal void InvokeOnPeersDisconnected(List<string> participantIds)
		{
			var c = OnPeerDisconnected;
			if (c != null)
				foreach(var p in participantIds)
					c(GetParticipantById(p));
		}
		
		internal void InvokeOnRoomAutoMatching()
		{
			var c = OnRoomAutoMatching;
			if (c != null)
				c();
		}
		
		internal void InvokeOnRoomConnecting()
		{
			var c = OnRoomConnecting;
			if (c != null)
				c();
		}
		
		internal void InvokeOnMessageSent(string recipientParticipantId, int tokenId)
		{
			var c = OnReliableMessageSent;
			if (c != null)
				c(GetParticipantById(recipientParticipantId), tokenId);
		}
		
		internal void InvokeOnMessageReceived(RealTimeMessage message)
		{
			if (message == null)
				return;
			
			var c = message.IsReliable ? OnReliableMessageReceived : OnUnreliableMessageReceived;
			if (c != null)
				c(GetParticipantById(message.SenderParticipantId), message);
		}
		
#if UNITY_ANDROID
		internal void InitFrom(Room other)
		{
			CreatorId = other.CreatorId;
			Description = other.CreatorId;
			Status = other.Status;
			Variant = other.Variant;
			
			var newParticipants = new List<Participant>();
			foreach(var op in other.Participants)
			{
				Participant np = null;
				foreach(var p in Participants)
				{
					if (p.Id == op.Id)
					{
						np = p;
						np.InitFrom(op);
						break;
					}
				}
				if (np == null)
					np = op;
				newParticipants.Add(np);
			}
			
			Participants.Clear();
			foreach(var p in newParticipants)
			{
				Participants.Add(p);
			}
		}
		
		internal Room(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getRoomId");
			CreatorId = obj.Call<string>("getCreatorId");
			Description = obj.Call<string>("getDescription");
			Status = (RoomStatus)obj.Call<int>("getStatus");
			Variant = obj.Call<int>("getVariant");
			
			Participants = new List<Participant>();
			
			var participantsObj = obj.Call<AndroidJavaObject>("getParticipants");
			if (participantsObj != null)
			{
				var count = participantsObj.Call<int>("size");
				for (var i = 0; i < count; ++i)
				{
					var participantObj = participantsObj.Call<AndroidJavaObject>("get", i);
					Participants.Add(new Participant(participantObj));
				}
			}
		}
#endif
	}
}

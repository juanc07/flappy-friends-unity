using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public enum ParticipantStatus
	{
		Invited = 1,
		Joined = 2,
		Left = 3,
		Declined = 4,
	}

	public class Participant
	{
		public string Id { get; private set; }
		public string DisplayName { get; private set; }
		public string HiResImageUri { get; private set; }
		public string IconImageUri { get; private set; }
		public ParticipantStatus Status { get; private set; }
		public bool IsConnectedToRoom { get; private set; }

		public Player Player { get; private set; }
		
		public override string ToString()
		{
			var result = "[Participant " + Id;
			result += ", Name = " + DisplayName;
			result += "]";
			return result;
		}
		
#if UNITY_ANDROID
		internal void InitFrom(Participant other)
		{
			Id = other.Id;
			DisplayName = other.DisplayName;
			HiResImageUri = other.HiResImageUri;
			IconImageUri = other.IconImageUri;
			Status = other.Status;
			IsConnectedToRoom = other.IsConnectedToRoom;
	
			if (Player == null)
				Player = other.Player;
			else
				Player.InitFrom(other.Player);
		}
		
		internal Participant(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getParticipantId");
			DisplayName = obj.Call<string>("getDisplayName");
			Status = (ParticipantStatus)obj.Call<int>("getStatus");
			IsConnectedToRoom = obj.Call<bool>("isConnectedToRoom");

			var playerObj = obj.Call<AndroidJavaObject>("getPlayer");
			Player = playerObj != null ? new Player(playerObj) : null;
			
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
	}
}

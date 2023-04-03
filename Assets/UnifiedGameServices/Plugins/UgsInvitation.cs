using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public class Invitation
	{
		public string Id { get; private set; }
		public Game Game { get; private set; }
		public Participant Inviter { get; private set; }
		
		public override string ToString()
		{
			var result = "[Invitation " + Id;
			result += "]";
			return result;
		}
		
#if UNITY_ANDROID
		internal Invitation(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getInvitationId");
			
			var gameObj = obj.Call<AndroidJavaObject>("getGame");
			Game = gameObj != null ? new Game(gameObj) : null;

			var inviterObj = obj.Call<AndroidJavaObject>("getInviter");
			Inviter = inviterObj != null ? new Participant(inviterObj) : null;
		}
#endif
	}
}

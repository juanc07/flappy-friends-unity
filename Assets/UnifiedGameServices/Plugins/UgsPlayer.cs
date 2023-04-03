using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public class Player
	{
		public string Id { get; private set; }
		public string DisplayName { get; private set; }
		public string HiResImageUri { get; private set; }
		public string IconImageUri { get; private set; }
		
		public override string ToString()
		{
			var result = "[Player " + Id;
			result += ", Name = " + DisplayName;
			result += "]";
			return result;
		}

		public Player(string id, string displayName)
		{
			Id = id;
			DisplayName = displayName;
		}
		
#if UNITY_ANDROID
		internal void InitFrom(Player other)
		{
			Id = other.Id;
			DisplayName = other.DisplayName;
			HiResImageUri = other.HiResImageUri;
			IconImageUri = other.IconImageUri;
		}
		
		internal Player(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getPlayerId");
			DisplayName = obj.Call<string>("getDisplayName");
			
			HiResImageUri = string.Empty;
			if (obj.Call<bool>("hasHiResImage"))
			{
				var uri = obj.Call<AndroidJavaObject>("getHiResImageUri");
				if (uri != null)
					HiResImageUri = uri.Call<string>("toString");
			}
			
			IconImageUri = string.Empty;
			if (obj.Call<bool>("hasIconImageUri"))
			{
				var uri = obj.Call<AndroidJavaObject>("getIconImageUri");
				if (uri != null)
					IconImageUri = uri.Call<string>("toString");
			}
		}
#endif
	}
}

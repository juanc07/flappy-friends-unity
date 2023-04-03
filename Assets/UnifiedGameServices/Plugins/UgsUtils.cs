using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	internal static class Utils
	{
		internal static bool IsStatusOk(string status)
		{
			var statusParts = status.Split(' ');
			return statusParts.Length > 0 && statusParts[0] == "0";
		}

		internal static DateTime TimeStampToDateTime(long timeStamp)
		{
			var dtDateTime = new DateTime(1970,1,1,0,0,0,0);
			dtDateTime = dtDateTime.AddSeconds(timeStamp / 1000).ToLocalTime();
			return dtDateTime;
		}
		
#if UNITY_ANDROID
		internal static List<string> AsStringsList(this AndroidJavaObject obj)
		{
			if (obj == null)
				return null;
			var result = new List<string>();
			var count = obj.Call<int>("size");
			for (var i = 0; i < count; ++i)
			{
				var item = obj.Call<string>("get", i);
				result.Add(item);
			}
			return result;
		}
		
		internal static void AddArray(this List<Player> container, AndroidJavaObject players)
		{
			if (players == null)
				return;
			
			var playersCount = players.Call<int>("getCount");
			for (var i = 0; i < playersCount; ++i)
			{
				var player = players.Call<AndroidJavaObject>("get", i);
				if (player == null)
					break;
				container.Add(new Player(player));
			}
		}
#endif
		
		internal static bool TryGetStatusArg(string status, int argNum, ref int arg)
		{
			var statusParts = status.Split(' ');
			if (argNum < 0 || argNum >= statusParts.Length)
				return false;
			int val;
			if (int.TryParse(statusParts[argNum], out val))
			{
				arg = val;
				return true;
			}
			return false;
		}
		
		internal static bool TryGetStatusArg(string status, int argNum, ref string arg)
		{
			var statusParts = status.Split(' ');
			if (argNum < 0 || argNum >= statusParts.Length)
				return false;
			arg = statusParts[argNum];
			return true;
		}
	}
}

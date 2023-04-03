using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public class RealTimeMessage
	{
		public string SenderParticipantId { get; private set; }
		public byte [] Data { get; private set; }
		public bool IsReliable { get; private set; }
		public int TokenId { get; private set; }
		
		public string StringData
		{
			get
			{
				if (Data == null)
					return null;
				return System.Text.Encoding.UTF8.GetString(Data);
			}
		}
		
		public override string ToString()
		{
			var result = "[RealTimeMessage]";
			return result;
		}
		
#if UNITY_ANDROID
		internal RealTimeMessage(AndroidJavaObject obj)
		{
			TokenId = obj.Call<int>("describeContents");
			SenderParticipantId = obj.Call<string>("getSenderParticipantId");
			Data = obj.Call<byte[]>("getMessageData");
			IsReliable = obj.Call<bool>("isReliable");
		}
#endif
	}
}

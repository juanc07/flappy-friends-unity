using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;

namespace Ugs
{
	public class State
	{
		internal static byte[] StringToByteArray(string str)
		{
			if (str == null)
				return null;
		    UTF8Encoding encoding = new UTF8Encoding();
		    return encoding.GetBytes(str);
		}
		
		internal static string ByteArrayToString(byte[] input)
		{
			if (input == null)
				return null;
		    UTF8Encoding enc = new UTF8Encoding();
		    string str = enc.GetString(input);
		    return str;
		}
		
		public int Key { get; private set; }
		public byte[] Data { get; private set; }
		public string StringData
		{
			get
			{
				return ByteArrayToString(Data);
			}
		}
		internal string Version { get; private set; }
		internal bool HasConflict { get; private set; }
		internal byte[] ConflictData { get; private set; }
		internal string ConflictVersion { get; private set; }
		internal string ResolvedVersion { get; private set; }
		
		public override string ToString()
		{
			var result = "[State " + Key + "]";
			return result;
		}
		
		internal void SetData(byte [] data)
		{
			Data = data;
			Version = string.Empty;
			HasConflict = false;
			ConflictData = null;
			ConflictVersion = string.Empty;
			ResolvedVersion = string.Empty;
		}
		
		internal void SetConflict(byte [] localData, byte [] conflictData, string resolvedVersion)
		{
			Data = localData;
			Version = string.Empty;
			HasConflict = true;
			ConflictData = conflictData;
			ConflictVersion = string.Empty;
			ResolvedVersion = resolvedVersion;
		}
		
		internal State(int stateKey, byte [] localData)
		{
			Key = stateKey;
			SetData(localData);
		}
		
		internal State(int stateKey, byte [] localData, byte [] conflictData, string resolvedVersion)
		{
			Key = stateKey;
			SetConflict(localData, conflictData, resolvedVersion);
		}
		
#if UNITY_ANDROID
		internal State(AndroidJavaObject obj)
		{
			Key = obj.Call<int>("getKey");
			Data = obj.Call<byte[]>("getLocalData");
			Version = obj.Call<string>("getLocalVersion");
			HasConflict = obj.Call<bool>("hasConflict");
			ConflictData = HasConflict ? obj.Call<byte[]>("conflictData") : null;
			ConflictVersion = HasConflict ? obj.Call<string>("conflictVersion") : null;
			ResolvedVersion = string.Empty;
		}
#endif
	}
}

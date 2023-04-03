using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public class Achievement
	{
		private enum State
		{
			STATE_HIDDEN = 2,
			STATE_REVEALED = 1,
			STATE_UNLOCKED = 0,
		}
		
		private enum Type
		{
			TYPE_INCREMENTAL = 1,
			TYPE_STANDARD = 0,
		}
		
		public string Id { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsRevealed { get; private set; }
		public bool IsUnlocked { get; private set; }
		public bool IsIncremental { get; private set; }
		public int CurrentSteps { get; private set; }
		public int TotalSteps { get; private set; }
		public string FormattedCurrentSteps { get; private set; }
		public string FormattedTotalSteps { get; private set; }
		public string RevealedIconUri { get; private set; }
		public string UnlockedIconUri { get; private set; }
		public DateTime LastUpdatedTime { get; private set; }

		public override string ToString()
		{
			var result = "[Achievement " + Name;
			result += ", " + ((IsRevealed ? (IsUnlocked ? "Unlocked" : "Revealed") : "Hidden"));
			if (IsIncremental)
				result += ", " + CurrentSteps + " / " + TotalSteps;
			result += "]";
			return result;
		}
		
		internal void Increment(int steps)
		{
			IsRevealed = true;
			CurrentSteps += steps;
			if (CurrentSteps >= TotalSteps)
			{
				CurrentSteps = TotalSteps;
				IsUnlocked = true;
			}
		}
		
		internal void Reveal()
		{
			IsRevealed = true;
		}
		
		internal void Unlock()
		{
			IsRevealed = true;
			CurrentSteps = TotalSteps;
			IsUnlocked = true;
		}
		
#if UNITY_ANDROID
		internal Achievement(AndroidJavaObject obj)
		{
			Id = obj.Call<string>("getAchievementId");
			Name = obj.Call<string>("getName");
			Description = obj.Call<string>("getDescription");
			
			var type = (Type)obj.Call<int>("getType");
			var state = (State)obj.Call<int>("getState");
			
			IsIncremental = type == Type.TYPE_INCREMENTAL;
			switch(state)
			{
			case State.STATE_HIDDEN:
				IsRevealed = false;
				IsUnlocked = false;
				break;
			case State.STATE_REVEALED:
				IsRevealed = true;
				IsUnlocked = false;
				break;
			case State.STATE_UNLOCKED:
				IsRevealed = true;
				IsUnlocked = true;
				break;
			}

			if (IsIncremental)
			{
				CurrentSteps = obj.Call<int>("getCurrentSteps");
				TotalSteps = obj.Call<int>("getTotalSteps");
				FormattedCurrentSteps = obj.Call<string>("getFormattedCurrentSteps");
				FormattedTotalSteps = obj.Call<string>("getFormattedTotalSteps");
			}

			try
			{
				RevealedIconUri = string.Empty;
				var uri = obj.Call<AndroidJavaObject>("getRevealedImageUri");
				if (uri != null)
					RevealedIconUri = uri.Call<string>("toString");
			}
			catch
			{
				RevealedIconUri = string.Empty;
			}

			try
			{
				UnlockedIconUri = string.Empty;
				var uri = obj.Call<AndroidJavaObject>("getUnlockedImageUri");
				if (uri != null)
					UnlockedIconUri = uri.Call<string>("toString");
			}
			catch
			{
				UnlockedIconUri = string.Empty;
			}

			LastUpdatedTime = Utils.TimeStampToDateTime(obj.Call<long>("getLastUpdatedTimestamp"));
		}
#endif

#if UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementId(int index);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetAchievementState(int index);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetAchievementType(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementName(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementDescription(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementRevealedIconUrl(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementUnlockedIconUrl(int index);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetAchievementCompletedSteps(int index);
		
		[DllImport ("__Internal")]
		private static extern int GpgGetAchievementNumberOfSteps(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementFormattedCompletedSteps(int index);
		
		[DllImport ("__Internal")]
		private static extern string GpgGetAchievementFormattedNumberOfSteps(int index);
		
		[DllImport ("__Internal")]
		private static extern long GpgGetAchievementLastUpdatedTimestamp(int index);
		
		[DllImport ("__Internal")]
		private static extern float GpgGetAchievementProgress(int index);

		internal Achievement(int index)
		{
			Id = GpgGetAchievementId(index);
			Name = GpgGetAchievementName(index);
			Description = GpgGetAchievementDescription(index);

			var state = GpgGetAchievementState(index);
			var type = GpgGetAchievementType(index);

			IsRevealed = (state != 0);
			IsUnlocked = (state == 2);
			IsIncremental = (type == 1);

			if (IsIncremental)
			{
				CurrentSteps = GpgGetAchievementCompletedSteps(index);
				TotalSteps = GpgGetAchievementNumberOfSteps(index);
				FormattedCurrentSteps = GpgGetAchievementFormattedCompletedSteps(index);
				FormattedTotalSteps = GpgGetAchievementFormattedNumberOfSteps(index);
			}

			RevealedIconUri = GpgGetAchievementRevealedIconUrl(index);
			UnlockedIconUri = GpgGetAchievementUnlockedIconUrl(index);

			LastUpdatedTime = Utils.TimeStampToDateTime(GpgGetAchievementLastUpdatedTimestamp(index));
		}
#endif
	}
}

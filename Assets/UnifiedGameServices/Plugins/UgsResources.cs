using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public static class Resources
	{
		#region Logging
		private static void DebugLog(string message)
		{
			if (!Config.Verbose)
				return;
			
			Debug.Log("[Gpg.Resources] " + message);
		}
		#endregion
		
		private class ImageLoadHandler
		{
			public event Action<Texture2D> OnImageLoaded;
			
			public ImageLoadHandler(Action<Texture2D> callback)
			{
				OnImageLoaded += callback;
			}
			
			public void Invoke(Texture2D texture)
			{
				if (OnImageLoaded != null)
					OnImageLoaded(texture);
			}
		}
		
		private static Dictionary<string, ImageLoadHandler> _imageLoadHandlers =
			new Dictionary<string, ImageLoadHandler>();
		
		internal static void Clear()
		{
			_imageLoadHandlers.Clear();
		}
		
		public static void LoadImage(string uri, Action<Texture2D> callback)
		{
			ImageLoadHandler handler = null;
			if (_imageLoadHandlers.TryGetValue(uri, out handler))
			{
				handler.OnImageLoaded += callback;
				return;
			}
			handler = new ImageLoadHandler(callback);
			_imageLoadHandlers.Add(uri, handler);
			StartImageLoad(uri);
		}
		
		private static void OnImageLoaded(string uri, Texture2D texture)
		{
			ImageLoadHandler handler = null;
			if (!_imageLoadHandlers.TryGetValue(uri, out handler))
				return;
			handler.Invoke(texture);
			_imageLoadHandlers.Remove(uri);
		}
		
#if UNITY_ANDROID
		private static AndroidJavaObject AndroidAdapter { get { return Client.AndroidAdapter; } }

		private static void StartImageLoad(string uri)
		{
			if (!Client.GamesCallsAllowed && !Client.AppStateCallsAllowed)
			{
				DebugLog("Failed to load image, either Gpg.Config.GamesEnabled or Gpg.Config.AppStateEnabled must be true");
				return;
			}
			
			if (Client.Schedule(true, false, () => StartImageLoad(uri)))
				return;
			
			if (!Client.AndroidAdapter.Call<bool>("loadImage", uri))
			{
				DebugLog("Failed to load image");
				OnImageLoaded(uri, null);
			}
		}
		
		internal static void _OnImageLoaded(string status)
		{
			var images = Client.AndroidAdapter.Call<AndroidJavaObject>("getLoadedImages");
			if (images != null)
			{
				var imagesCount = images.Call<int>("size");
				for (var i = 0; i < imagesCount; ++i)
				{
					var image = images.Call<AndroidJavaObject>("get", i);
					if (image == null)
						break;
					
					var uri = image.Call<string>("getUri");
					var imageBase64 = image.Call<string>("getImageData");
					
					Texture2D texture = null;
					if (imageBase64.Length > 0)
					{
						var imageBytes = System.Convert.FromBase64String(imageBase64);
						texture = new Texture2D(4, 4);
	        			texture.LoadImage(imageBytes);
					}
					OnImageLoaded(uri, texture);
				}
			}
			images.Call("clear");
		}
#elif UNITY_IPHONE
		private static void StartImageLoad(string uri)
		{
			OnImageLoaded(uri, null);
		}
#else
		private static void StartImageLoad(string uri)
		{
			OnImageLoaded(uri, null);
		}
#endif
	}
}
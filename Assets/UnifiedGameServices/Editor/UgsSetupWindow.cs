using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

public class UgsSetupWindow : EditorWindow
{
	private const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
	private const float CheckInterval = 1.0f;
	private const string DefaultAndroidManifest = "UnifiedGameServices/Editor/DefaultAndroidManifest.xml";
	
	private const string AndroidManifest = "Plugins/Android/AndroidManifest.xml";
	private const string GooglePlayServicesJar = "Plugins/Android/google-play-services.jar";
	private const string AndroidSupportV4Jar = "Plugins/Android/android-support-v4.jar";
	
	private const string IosConfig = "Plugins/iOS/GpgIosConfig.h";
	
	private const string GoogleOpenSourceFramework = "Plugins/iOS/GoogleOpenSource.framework";
	private const string GooglePlusFramework = "Plugins/iOS/GooglePlus.framework";
	private const string GooglePlusBundle = "Plugins/iOS/GooglePlus.bundle";
	private const string PlayGameServicesFramework = "Plugins/iOS/PlayGameServices.framework";
	private const string PlayGameServicesBundle = "Plugins/iOS/PlayGameServices.bundle";

	private const string GooglePlayServicesSourceJar = "extras/google/google_play_services/libproject/google-play-services_lib/libs/google-play-services.jar";
	private const string AndroidSupportV4SourceJar = "extras/android/support/v4/android-support-v4.jar";
	
	private const string IosAppIdKey = "GPG_APPLICATION_ID";
	private const string IosClientIdKey = "GPG_CLIENT_ID";
	
	private static string [] AndroidSdkRoots = new []
	{
		"%ANDROID_HOME%",
		"%ANDROID_SDK%",
		"%ANDROID_SDK_PATH%",
		"%ANDROID_PATH%",
		"%ANDROID%",
		"%PROGRAMFILES(x86)%\\Android\\android-sdk\\",
		"%PROGRAMFILES(x86)%\\AndroidSDK\\android-sdk\\",
		"%PROGRAMFILES(x86)%\\Android SDK\\android-sdk\\",
		"%PROGRAMFILES%\\Android\\android-sdk\\",
		"%PROGRAMFILES%\\AndroidSDK\\android-sdk\\",
		"%PROGRAMFILES%\\Android SDK\\android-sdk\\",
		"C:\\Program Files\\Android\\android-sdk\\",
		"C:\\Program Files\\AndroidSDK\\android-sdk\\",
		"C:\\Program Files\\Android SDK\\android-sdk\\",
		"%HOME%/android-sdk/",
		"%HOME%/android-sdk-mac/",
		"%HOME%/android-sdk-macosx/",
		"%HOME%/android-sdk-mac_86/",
		"%HOME%/android-sdk-mac_x86/",
	};
	
	[MenuItem("Art of Bytes/Unified Game Services")]
    static void ShowWindow()
	{
		((UgsSetupWindow)EditorWindow.GetWindow(typeof(UgsSetupWindow))).CheckSetup(true);
	}
	
	public UgsSetupWindow()
	{
		title = "Google Play Games";
	}

	private static string FullPathTo(string asset)
	{
		return Path.GetFullPath(Path.Combine(Application.dataPath, asset));
	}
	
	private float _lastSetupCheckTime = 0.0f;
	private bool _androidManifestPresent = false;
	private bool _androidManifestModified = false;
	private bool _googlePlayServicesJarPresent = false;
	private bool _androidSupportV4JarPresent = false;
	
	private string _googlePlayServicesJar = string.Empty;
	private string _androidSupportV4Jar = string.Empty;
	
	private bool _iosConfigPresent = false;
	private bool _iosConfigModified = false;
	
	private bool _googleOpenSourceFrameworkPresent = false;
	private bool _googlePlusFrameworkPresent = false;
	private bool _googlePlusBundlePresent = false;
	private bool _playGameServicesFrameworkPresent = false;
	private bool _playGameServicesBundlePresent = false;
	
	private string _appIdFromManifest = string.Empty;
	private string _appIdFromIosConfig = string.Empty;
	private string _userDefinedAppId = string.Empty;
	
	private string _clientIdFromIosConfig = string.Empty;
	private string _userDefinedClientId = string.Empty;
	
	private void TraverseAndroidManifest(bool readState, bool writeState, string newAppId)
	{
		var gamesAppIdPresent = false;
		var appstateAppIdPresent = false;
		var appId = string.Empty;
		var connectionResolverPresent = false;
		
		var doc = new XmlDocument();
		doc.Load(FullPathTo(AndroidManifest));
		
		var app = doc.SelectSingleNode("/manifest/application");
		var metaDatas = doc.SelectNodes("/manifest/application/meta-data");
		for (var i = 0; i < metaDatas.Count; ++i)
		{
			var metaData = metaDatas.Item(i);
			var name = metaData.Attributes.GetNamedItem("android:name");
			var value = metaData.Attributes.GetNamedItem("android:value");
			if (name == null || value == null)
				continue;
			if (name.Value == "com.google.android.gms.games.APP_ID" ||
				name.Value == "com.google.android.gms.appstate.APP_ID")
			{
				if (writeState)
				{
					value.Value = "\\ " + newAppId;
				}
				
				gamesAppIdPresent |= name.Value == "com.google.android.gms.games.APP_ID";
				appstateAppIdPresent |= name.Value == "com.google.android.gms.appstate.APP_ID";
				appId = value.Value;
				if (appId.StartsWith("\\ "))
					appId = appId.Substring(2);
			}
		}
		
		if (!gamesAppIdPresent && writeState)
		{
			var metaData = doc.CreateNode(XmlNodeType.Element, "meta-data", "");
			
			var name = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, "android", "name", AndroidXmlNamespace);
			name.Value = "com.google.android.gms.games.APP_ID";
			metaData.Attributes.Append(name);
			
			var value = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, "android", "value", AndroidXmlNamespace);
			value.Value = "\\ " + newAppId;
			metaData.Attributes.Append(value);
			
			app.AppendChild(metaData);
			
			gamesAppIdPresent = true;
			appId = newAppId;
		}
		
		if (!appstateAppIdPresent && writeState)
		{
			var metaData = doc.CreateNode(XmlNodeType.Element, "meta-data", "");
			
			var name = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, "android", "name", AndroidXmlNamespace);
			name.Value = "com.google.android.gms.appstate.APP_ID";
			metaData.Attributes.Append(name);
			
			var value = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, "android", "value", AndroidXmlNamespace);
			value.Value = "\\ " + newAppId;
			metaData.Attributes.Append(value);
			
			app.AppendChild(metaData);
			
			appstateAppIdPresent = true;
			appId = newAppId;
		}
	
		var activities = doc.SelectNodes("/manifest/application/activity");
		for (var i = 0; i < activities.Count; ++i)
		{
			var activity = activities.Item(i);
			var name = activity.Attributes.GetNamedItem("android:name");
			var label = activity.Attributes.GetNamedItem("android:label");
			if (name == null || label == null)
				continue;
			if (name.Value == "com.artofbytes.gpg.android.ConnectionResolver")
			{
				connectionResolverPresent = true;
			}
		}
		
		if (!connectionResolverPresent && writeState)
		{
			var activity = doc.CreateNode(XmlNodeType.Element, "activity", "");
			
			var name = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, "android", "name", AndroidXmlNamespace);
			name.Value = "com.artofbytes.gpg.android.ConnectionResolver";
			activity.Attributes.Append(name);
			
			var label = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, "android", "label", AndroidXmlNamespace);
			label.Value = "@string/app_name";
			activity.Attributes.Append(label);
			
			app.AppendChild(activity);
			
			connectionResolverPresent = true;
		}
		
		if (readState)
		{
			var keepUserDefinedAppIdSynced = (_userDefinedAppId == _appIdFromManifest);
			
			_androidManifestModified = gamesAppIdPresent && appstateAppIdPresent && connectionResolverPresent;
			if (gamesAppIdPresent || appstateAppIdPresent)
				_appIdFromManifest = appId;
			
			if (keepUserDefinedAppIdSynced ||
				(!string.IsNullOrEmpty(_appIdFromManifest) && 
				string.IsNullOrEmpty(_userDefinedAppId)))
				_userDefinedAppId = _appIdFromManifest;
		}
		
		if (writeState)
		{
			doc.Save(FullPathTo(AndroidManifest));
		}
	}
	
	private static string FindAndroidSDKJar(string sourceJar)
	{
		var jarPaths = new List<string>();
		
		var prefsRoot = EditorPrefs.GetString("AndroidSdkRoot", string.Empty);
		if (!string.IsNullOrEmpty(prefsRoot))
		{
			var root = System.Environment.ExpandEnvironmentVariables(prefsRoot);
			var path = Path.GetFullPath(Path.Combine(root, sourceJar));
			jarPaths.Add(path);
		}

		foreach(var root in AndroidSdkRoots)
		{
			var resolvedRoot = System.Environment.ExpandEnvironmentVariables(root);
			var path = Path.GetFullPath(Path.Combine(resolvedRoot, sourceJar));
			jarPaths.Add(path);
		}

		foreach(var path in jarPaths)
		{
			if (File.Exists(path))
				return path;
		}
		
		return string.Empty;
	}
	
	public static string GetIosApplicationId()
	{
		var configFileName = FullPathTo(IosConfig);
		if (!File.Exists(configFileName))
			return string.Empty;
		var configLines = File.ReadAllText(configFileName).Split('\n');
		foreach(var configLine in configLines)
		{
			var lineParts = configLine.Split(' ');
			if (lineParts.Length > 2 &&
				lineParts[0] == "#define" &&
				lineParts[1] == IosAppIdKey)
				return lineParts[2].Trim('\"');
		}
		return string.Empty;
	}
	
	private void CheckSetup(bool force)
	{
		if (EditorApplication.timeSinceStartup < _lastSetupCheckTime + CheckInterval && !force)
			return;
		_lastSetupCheckTime = (float)EditorApplication.timeSinceStartup;
		
		_googlePlayServicesJarPresent = File.Exists(FullPathTo(GooglePlayServicesJar));
		if (!_googlePlayServicesJarPresent)
		{
			_googlePlayServicesJar = FindAndroidSDKJar(GooglePlayServicesSourceJar);
		}
		_androidSupportV4JarPresent = File.Exists(FullPathTo(AndroidSupportV4Jar));
		if (!_androidSupportV4JarPresent)
		{
			_androidSupportV4Jar = FindAndroidSDKJar(AndroidSupportV4SourceJar);
		}
		_androidManifestPresent = File.Exists(FullPathTo(AndroidManifest));
		if (!_androidManifestPresent)
		{
			_androidManifestModified = false;
			_appIdFromManifest = string.Empty;
		}
		else
		{
			TraverseAndroidManifest(true, false, "");
		}
		
		
		_googleOpenSourceFrameworkPresent = Directory.Exists(FullPathTo(GoogleOpenSourceFramework));
		_googlePlusFrameworkPresent = Directory.Exists(FullPathTo(GooglePlusFramework));
		_googlePlusBundlePresent = Directory.Exists(FullPathTo(GooglePlusBundle));
		_playGameServicesFrameworkPresent = Directory.Exists(FullPathTo(PlayGameServicesFramework));
		_playGameServicesBundlePresent = Directory.Exists(FullPathTo(PlayGameServicesBundle));
		
		
		_iosConfigPresent = File.Exists(FullPathTo(IosConfig));
		if (!_iosConfigPresent)
		{
			_iosConfigModified = false;
			_appIdFromIosConfig = string.Empty;
			_clientIdFromIosConfig = string.Empty;
		}
		else
		{
			_appIdFromIosConfig = string.Empty;
			_clientIdFromIosConfig = string.Empty;
			
			var appIdPresent= false;
			var clientIdPresent= false;
			
			var configContent = File.ReadAllText(FullPathTo(IosConfig));
			var configLines = configContent.Split('\n');
			foreach(var configLine in configLines)
			{
				var lineParts = configLine.Split(' ');
				if (lineParts.Length < 3)
					continue;
				if (lineParts[0] != "#define")
					continue;
				
				if (lineParts[1] == IosAppIdKey)
				{
					appIdPresent = true;
					_appIdFromIosConfig = lineParts[2].Trim('\"');
				}
				if (lineParts[1] == IosClientIdKey)
				{
					clientIdPresent = true;
					_clientIdFromIosConfig = lineParts[2].Trim('\"');
				}
			}
			
			if (string.IsNullOrEmpty(_userDefinedClientId))
				_userDefinedClientId = _clientIdFromIosConfig;
			
			_iosConfigModified = appIdPresent && clientIdPresent;
		}
	}
	
	private bool AndroidSetupIsOk
	{
		get
		{
			return
				_androidManifestPresent && 
				_googlePlayServicesJarPresent &&
				_androidSupportV4JarPresent &&
				_androidManifestModified && 
				!string.IsNullOrEmpty(_appIdFromManifest);
		}
	}
	
	private void FixGooglePlayServicesJar()
	{
		File.Copy(_googlePlayServicesJar, FullPathTo(GooglePlayServicesJar), true);
	}
	
	private void FixAndroidSupportV4Jar()
	{
		File.Copy(_androidSupportV4Jar, FullPathTo(AndroidSupportV4Jar), true);
	}
	
	private void FixAndroidManifestPresense()
	{
		File.Copy(FullPathTo(DefaultAndroidManifest), FullPathTo(AndroidManifest), true);
	}
	
	private void FixAndroidManifestModification()
	{
		TraverseAndroidManifest(false, true, _userDefinedAppId);
	}
	
	private void ApplyAppIdToAndroidManifest(string appId)
	{
		TraverseAndroidManifest(false, true, appId);
	}
	
	
	private bool IosSetupIsOk
	{
		get
		{
			return _iosConfigPresent && _iosConfigModified &&
				_playGameServicesFrameworkPresent && _playGameServicesFrameworkPresent &&
				_googleOpenSourceFrameworkPresent && _googlePlusFrameworkPresent && _googlePlusBundlePresent;
		}
	}
		
	private void FixIosConfigPresense()
	{
		var defaultFile = "";
		defaultFile += "#define GPG_APPLICATION_ID \"" + _userDefinedAppId + "\"\n";
		defaultFile += "#define GPG_CLIENT_ID \"" + _userDefinedClientId + "\"\n";
		File.WriteAllText(FullPathTo(IosConfig), defaultFile);
	}
	
	private void ApplyIdsToIosConfig(string appId, string clientId)
	{
		var defaultFile = "";
		defaultFile += "#define GPG_APPLICATION_ID \"" + appId + "\"\n";
		defaultFile += "#define GPG_CLIENT_ID \"" + clientId + "\"\n";
		File.WriteAllText(FullPathTo(IosConfig), defaultFile);
	}
	
	void OnGUI()
	{
		CheckSetup(false);
		
		_userDefinedAppId = EditorGUILayout.TextField ("Application ID:", _userDefinedAppId);
		EditorGUILayout.LabelField("iOS Client ID:");
		_userDefinedClientId = EditorGUILayout.TextArea (_userDefinedClientId);
		if (_userDefinedAppId != _appIdFromManifest ||
			_userDefinedAppId != _appIdFromIosConfig ||
			_userDefinedClientId != _clientIdFromIosConfig)
		{
			EditorGUILayout.BeginHorizontal();
			GUI.skin.button.fixedWidth = 60;
			GUI.skin.button.fixedHeight = 0;
			if (GUILayout.Button("Apply"))
			{
				GUIUtility.keyboardControl = 0;
			
				if (!_androidManifestPresent)
					FixAndroidManifestPresense();
				if (!_androidManifestModified)
					FixAndroidManifestModification();
				ApplyAppIdToAndroidManifest(_userDefinedAppId);
				
				if (!_iosConfigPresent)
					FixIosConfigPresense();
				ApplyIdsToIosConfig(_userDefinedAppId, _userDefinedClientId);
				CheckSetup(true);
			}
			if (GUILayout.Button("Revert"))
			{
				GUIUtility.keyboardControl = 0;
				if (_appIdFromManifest == _appIdFromIosConfig)
					_userDefinedAppId = _appIdFromManifest;
				_userDefinedClientId = _clientIdFromIosConfig;
			}
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		if (AndroidSetupIsOk)
		{
			EditorGUILayout.LabelField("Android - [OK]", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Google Play Services are set up correctly for Android platform with an application ID " + _appIdFromManifest, MessageType.Info, true);
		}
		else
		{
			EditorGUILayout.LabelField("Android - [Incomplete]", EditorStyles.boldLabel);

			if (!_androidManifestPresent)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox(AndroidManifest + " file is missing. Default manifest file can be generated.", MessageType.Error, true);
				GUI.skin.button.fixedWidth = 39;
				GUI.skin.button.fixedHeight = 39;
				if (GUILayout.Button("Fix"))
				{
					FixAndroidManifestPresense();
					FixAndroidManifestModification();
					CheckSetup(true);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (_androidManifestPresent && !_androidManifestModified)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("AndroidManifest.xml file is present, but missing lines required for Google Play Services to work correctly.", MessageType.Error, true);
				GUI.skin.button.fixedWidth = 39;
				GUI.skin.button.fixedHeight = 39;
				if (GUILayout.Button("Fix"))
				{
					FixAndroidManifestModification();
					CheckSetup(true);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (string.IsNullOrEmpty(_appIdFromManifest) && _androidManifestPresent && _androidManifestModified)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("Application ID is not set up, please enter it in field above and apply the change.", MessageType.Warning, true);
				EditorGUILayout.EndHorizontal();
			}
			if (!_googlePlayServicesJarPresent)
			{
				EditorGUILayout.BeginHorizontal();
				if (!string.IsNullOrEmpty(_googlePlayServicesJar))
				{
					EditorGUILayout.HelpBox(GooglePlayServicesJar + " file is missing. It can be copied from Android SDK.", MessageType.Error, true);
					GUI.skin.button.fixedWidth = 39;
					GUI.skin.button.fixedHeight = 39;
					if (GUILayout.Button("Fix"))
					{
						FixGooglePlayServicesJar();
						CheckSetup(true);
					}
				}
				else
				{
					EditorGUILayout.HelpBox(GooglePlayServicesJar + " file is missing. You need to install Google Play services from your Android SDK Manager and get this jar file from there.", MessageType.Error, true);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (!_androidSupportV4JarPresent)
			{
				EditorGUILayout.BeginHorizontal();
				if (!string.IsNullOrEmpty(_androidSupportV4Jar))
				{
					EditorGUILayout.HelpBox(AndroidSupportV4Jar + " file is missing. It can be copied from Android SDK.", MessageType.Error, true);
					GUI.skin.button.fixedWidth = 39;
					GUI.skin.button.fixedHeight = 39;
					if (GUILayout.Button("Fix"))
					{
						FixAndroidSupportV4Jar();
						CheckSetup(true);
					}
				}
				else
				{
					EditorGUILayout.HelpBox(AndroidSupportV4Jar + " file is missing. You need to install Google Play services from your Android SDK Manager and get this jar file from there.", MessageType.Error, true);
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		if (IosSetupIsOk)
		{
			EditorGUILayout.LabelField("iOS - [OK]", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Google Play Services are set up correctly for iOS platform with an application ID " + _appIdFromIosConfig + " and Client ID " + _clientIdFromIosConfig, MessageType.Info, true);
		}
		else
		{
			EditorGUILayout.LabelField("iOS - [Incomplete]", EditorStyles.boldLabel);

			if (!_iosConfigPresent)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox(IosConfig + " file is missing. Default config can be generated.", MessageType.Error, true);
				GUI.skin.button.fixedWidth = 39;
				GUI.skin.button.fixedHeight = 39;
				if (GUILayout.Button("Fix"))
				{
					FixIosConfigPresense();
					CheckSetup(true);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (_iosConfigPresent && !_iosConfigModified)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("GpgIosConfig.h file is present, but missing lines required for Google Play Services to work correctly.", MessageType.Error, true);
				GUI.skin.button.fixedWidth = 39;
				GUI.skin.button.fixedHeight = 39;
				if (GUILayout.Button("Fix"))
				{
					FixIosConfigPresense();
					CheckSetup(true);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (!_playGameServicesFrameworkPresent || !_playGameServicesBundlePresent)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("Games SDK for iOS is missing. You need to have PlayGameServices.framework and PlayGameServices.bundle in Assets/Plugins/iOS folder. Games SDK can be downloaded here: https://developers.google.com/games/services/downloads/. ", MessageType.Error, true);
				GUI.skin.button.fixedWidth = 69;
				GUI.skin.button.fixedHeight = 39;
				if (GUILayout.Button("Download"))
				{
					Application.OpenURL("https://developers.google.com/games/services/downloads/");
				}
				EditorGUILayout.EndHorizontal();
			}
			if (!_googleOpenSourceFrameworkPresent || !_googlePlusFrameworkPresent || !_googlePlusBundlePresent)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("Google+ SDK for iOS is missing. You need to have GoogleOpenSource.framework, GooglePlus.framework and GooglePlus.bundle in Assets/Plugins/iOS folder. Google+ SDK can be downloaded here: https://developers.google.com/games/services/downloads/. ", MessageType.Error, true);
				GUI.skin.button.fixedWidth = 69;
				GUI.skin.button.fixedHeight = 39;
				if (GUILayout.Button("Download"))
				{
					Application.OpenURL("https://developers.google.com/games/services/downloads/");
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		GUI.skin.button.fixedWidth = 0;
		GUI.skin.button.fixedHeight = 0;
	}
}

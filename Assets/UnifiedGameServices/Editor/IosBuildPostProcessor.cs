using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UgsXCodeEditor;
using UnityEditor.UgsPlistEditor;
using UnityEngine;
using System.IO;
using System;

public class IosBuildPostProcessor
{
	private static string GetRelativePath(string root, string target)
	{
		var rootUri = new Uri(root + "/.");
		var targetUri = new Uri(Path.Combine(Application.dataPath, target));
		return rootUri.MakeRelativeUri(targetUri).ToString();
	}
	
	private static bool InjectCode(string fileName, string after, string before, string injection)
	{
		if (!File.Exists(fileName))
			return false;
		
		var insertAfter = true;
		if (string.IsNullOrEmpty(after))
		{
			after = before;
			insertAfter = false;
		}
		
		string [] afterParts = after.Split(' ');

		var fileContent = File.ReadAllText(fileName);
		
		var startPos = 0;
		var position = 0;
		if (afterParts.Length > 0) while(true)
		{
			startPos = position;
			var start = true;
			var match = true;
			foreach (var part in afterParts)
			{
				var nextPos = fileContent.IndexOf(part, position);

				if (nextPos < 0)
					return false;

				if (!start)
				{
					var whitespace = fileContent.Substring(position, nextPos - position);

					if (whitespace.Trim().Length > 0)
					{
						match = false;
						break;
					}
				}
				start = false;
				position = nextPos + part.Length;
			}
			if (match)
				break;
		}
		
		if (!insertAfter)
		{
			position = Mathf.Max(0, startPos - 1);
		}
		
		fileContent = fileContent.Substring(0, position) + injection + fileContent.Substring(position);
		File.WriteAllText(fileName, fileContent);
		return true;
	}

	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
#if UNITY_IPHONE
		Debug.Log("Post-processing XCode project " + path + " from " + Application.dataPath);

		var currentProject = new XCProject(path);

		var resourcesGroup = currentProject.GetGroup( "Resources" );
		currentProject.AddFile("Plugins/iOS/GooglePlus.bundle", resourcesGroup, "SOURCE_ROOT", true, false);
		currentProject.AddFile("Plugins/iOS/PlayGameServices.bundle", resourcesGroup, "SOURCE_ROOT", true, false);

		var frameworkGroup = currentProject.GetGroup( "Frameworks" );

		currentProject.AddFile("System/Library/Frameworks/AssetsLibrary.framework", frameworkGroup, "SDKROOT", true, false);
		currentProject.AddFile("System/Library/Frameworks/CoreData.framework", frameworkGroup, "SDKROOT", true, false);
		currentProject.AddFile("System/Library/Frameworks/Security.framework", frameworkGroup, "SDKROOT", true, false);
		currentProject.AddFile("System/Library/Frameworks/SystemConfiguration.framework", frameworkGroup, "SDKROOT", true, false);
		currentProject.AddFile("System/Library/Frameworks/QuartzCore.framework", frameworkGroup, "SDKROOT", true, false);
		currentProject.AddFile("System/Library/Frameworks/CoreText.framework", frameworkGroup, "SDKROOT", true, false);

		currentProject.AddFile("Plugins/iOS/GoogleOpenSource.framework", frameworkGroup, "SOURCE_ROOT", true, false);
		currentProject.AddFile("Plugins/iOS/GooglePlus.framework", frameworkGroup, "SOURCE_ROOT", true, false);
		currentProject.AddFile("Plugins/iOS/PlayGameServices.framework", frameworkGroup, "SOURCE_ROOT", true, false);

		currentProject.AddOtherLinkerFlags("-ObjC");

		var frameworkSearchPath = GetRelativePath(path, "Plugins/iOS");
		currentProject.AddFrameworkSearchPaths(frameworkSearchPath);

		currentProject.Save();


		var infoPlistPath = Path.Combine(path, "Info.plist");
		Debug.Log("Post-processing Info plist " + infoPlistPath + " from " + Application.dataPath);

		var infoPlist = new ProjectInfoPlist(infoPlistPath);
		infoPlist.AddStringValue("GPGApplicationID", UgsSetupWindow.GetIosApplicationId());
		infoPlist.AddUrlType(PlayerSettings.bundleIdentifier, "Editor", new [] { PlayerSettings.bundleIdentifier });
		infoPlist.Save();
		
		var appControllerMm = Path.Combine(path, "Classes/AppController.mm");
		if (!File.Exists(appControllerMm))
		{
			appControllerMm = Path.Combine(path, "Classes/UnityAppController.mm");
		}
		if (File.Exists(appControllerMm))
		{
			InjectCode(appControllerMm, "", "", "#import \"../Libraries/GpgIosAdapter.h\"\n");
			
			if (InjectCode(appControllerMm, "- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {", "",
			           "\n\tif ([GpgUnityAdapter application:application openURL:url sourceApplication:sourceApplication annotation:annotation]) \n\t\treturn YES;\n\n"));
			else
				InjectCode(appControllerMm, "", 
					"- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions {",
			        "\n- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation \n{\n\tif ([GpgUnityAdapter application:application openURL:url sourceApplication:sourceApplication annotation:annotation]) \n\t\treturn YES;\n\n\treturn NO;\n}\n\n");
		}
#endif
	}
}

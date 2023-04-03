using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class SoundManagerEditor:EditorWindow{
	[MenuItem("Custom/Sound Manager /Setup...", false, 1)]
	public static void MenuItemSetup() {
		EditorWindow.GetWindow(typeof(SoundManagerEditor));
	}

	private void CreateFolder(string path, string folderName){
		string resourcesPath= "Assets/Resources";
		if (!System.IO.Directory.Exists(resourcesPath)){
			AssetDatabase.CreateFolder("Assets", "Resources");
		}

		if (!System.IO.Directory.Exists(path + "/" + folderName)){
			AssetDatabase.CreateFolder(path, folderName);
		}else{
			EditorUtility.DisplayDialog("Failed: ", folderName + " folder already exist!","ok");
		}
	}

	private void OnGUI(){
		// Title
		GUILayout.BeginArea(new Rect(20, 20, position.width - 40, position.height));
		GUILayout.Label("Sound Manager Setup", EditorStyles.boldLabel);
		GUILayout.Label("NOTES:");
		GUILayout.Label("1. Please rename your audio files \navoid numbers at the begining and avoid spaces \nto prevent parsing error!");
		GUILayout.Label("2. Disable 3D Sound settings on your audio files");
		GUILayout.Space(10);

		// Setup button
		if (GUILayout.Button("Create Sfx and BGM Folder")) {
			CreateFolder("Assets/Resources","SFX");
			CreateFolder("Assets/Resources","BGM");
		}else if (GUILayout.Button("Generate Sfx List")) {
			CreateAudioList("Assets/Resources/SFX","SFX","SFX","Assets/Managers/SoundManager/");
		}else if (GUILayout.Button("Generate BGM List")) {
			CreateAudioList("Assets/Resources/BGM","BGM","BGM","Assets/Managers/SoundManager/");
		}
		GUILayout.EndArea();
	}

	private void CreateAudioList( string audioFolderPath, string audioListname, string audioFolderName,string audiolistFinalPath ){
		string path =audiolistFinalPath + audioListname + ".cs";
		object[] loadedAudio = Resources.LoadAll(audioFolderName);
		int len = loadedAudio.Length;
		int count =0;
		
		if (!System.IO.Directory.Exists(audioFolderPath)){
			EditorUtility.DisplayDialog("Failed: ", "can't create " + audioListname  + " List , Please generate " + audioFolderName + " folder","ok");
			return;
		}
		
		if(len == 0){
			EditorUtility.DisplayDialog("Failed: ", "can't create " + audioListname  + " , Audio files is missing on " + audioFolderName + " folder","ok");
			return;
		}

		if(File.Exists(path)){
			AssetDatabase.DeleteAsset(path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		StringBuilder sb = new StringBuilder();
		sb.Append("//" + audioListname +" LIST \n");
		sb.Append("public enum " + audioListname +"{\n");
		
		foreach( object audio in loadedAudio ){
			AudioClip clip = (AudioClip)audio;
			if(len == 1){
				sb.Append("\t\t"+clip.name+"\n");
			}else{
				count++;
				if(count<len){
					sb.Append("\t\t"+clip.name+","+"\n");
				}else{
					sb.Append("\t\t"+clip.name+"\n");
				}
			}			
		}
		sb.Append("\t}");

		File.WriteAllText(path,sb.ToString());
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Object mono = AssetDatabase.LoadAssetAtPath(path,typeof(MonoScript));
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = mono;
		EditorUtility.DisplayDialog("Success: ", audioListname + " List Generated Successfully!","ok");
	}
}

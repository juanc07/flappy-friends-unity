using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class LevelObjectGenerator:EditorWindow{

	public Object groundBlock;
	private string groundLength;
	public Transform parent;

	[HideInInspector]
	private GameObject newGroundBlock;

	[HideInInspector]
	private GroundSizeController groundSizeController;

	[MenuItem("Custom/LevelObjectGenerator /Setup...", false, 1)]
	public static void MenuItemSetup() {
		EditorWindow.GetWindow(typeof(LevelObjectGenerator));
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
		GUILayout.Label("LevelObjectGenerator", EditorStyles.boldLabel);
		GUILayout.Space(10);

		groundBlock =  EditorGUILayout.ObjectField("groundBlock",groundBlock,typeof(object));
		parent = Selection.activeTransform;
		EditorGUILayout.ObjectField("Parent",parent,typeof(Transform));
		groundLength = EditorGUILayout.TextField("ground length: ",groundLength);

		// Setup button
		if (GUILayout.Button("Create Ground Prefabs")) {
			newGroundBlock = Instantiate(groundBlock) as GameObject;
			newGroundBlock.transform.parent = parent.gameObject.transform;
			groundSizeController = newGroundBlock.GetComponent<GroundSizeController>();
			groundSizeController.length = System.Int32.Parse(groundLength); 
			groundSizeController.isClear =true;
			groundSizeController.generate =true;
			groundSizeController.combineMeshNow =true;
		}else if (GUILayout.Button("Create Level Prefabs")) {
			CreateFolder("Assets/Resources","LevelPrefabs");
		}
		GUILayout.EndArea();
	}

	/*
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
	}*/
}

using System.Collections;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class ParticleManagerEditor : EditorWindow {
	public string particleEffectPath;

	private string particleName="";
	private GameObject particlePrefab;

	private ParticleConfigHolder particleConfig;
	private Array particleEffects;
	private string[] particleNames;
	private int particleCount;

	//new
	private int particleIndex=0;


	[MenuItem("Custom Editor/Particle Manager/Setup...")]
	public static void MenuParticleSetup(){
		EditorWindow.GetWindow(typeof(ParticleManagerEditor));
	}

	private void CreateFolder(string path, string folderName){
		string resourcesPath= "Assets/Resources";
		if (!System.IO.Directory.Exists(resourcesPath)){
			AssetDatabase.CreateFolder("Assets", "Resources");
		}
		
		if (!System.IO.Directory.Exists(path + "/" + folderName)){
			AssetDatabase.CreateFolder(path, folderName);
		}
		/*else{
			EditorUtility.DisplayDialog("Failed: ", folderName + " folder already exist!","ok");
		}*/
	}

	private void OnGUI(){
		GUILayout.BeginArea(new Rect(20,20,position.width-40,position.height));
		GUILayout.Label("ParticleManager",EditorStyles.boldLabel);
		GUILayout.Space(10);

		ParticleConfigHolder particleConfigHolder =(ParticleConfigHolder)ScriptableObject.CreateInstance("ParticleConfigHolder");
		
		particleEffects = Enum.GetValues(typeof(ParticleEffect));
		particleCount = particleEffects.Length;
		particleNames = Enum.GetNames(typeof(ParticleEffect));
		
		if(GUILayout.Button("Create Config")){
			CreateFolder("Assets/Resources","Config");
			for(int index = 0; index<particleCount;index++){
				particleConfigHolder.particles.Set((ParticleEffect)particleEffects.GetValue(index),null);
			}

			AssetDatabase.CreateAsset(particleConfigHolder,"Assets/Resources/Config/ParticleConfig.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = particleConfigHolder;
			EditorUtility.DisplayDialog("Success: ", "Particle Config created successfully","ok");
		}else if(GUILayout.Button("Delete Config")){
			particleEffectPath = "";
			AssetDatabase.DeleteAsset("Assets/Resources/Config/ParticleConfig.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.DisplayDialog("Success: ", "Particle Config deleted successfully","ok");
		}

		GUILayout.Space(10);
		particleName = EditorGUILayout.TextField("Particle Name: ", particleName);
		if(GUI.Button (new Rect(210,105,100,30), "Add Particle")){
			if(!particleName.Equals("",StringComparison.Ordinal)){
				CreateParticleEnum(particleName,"ParticleEffect","Assets/Managers/ParticleManager/");
			}else{
				EditorUtility.DisplayDialog("Failed: ", "particle name is empty!, please enter particle name","ok");
			}

		}

		GUILayout.Space(50);
		particleIndex = EditorGUILayout.Popup("Awesome Drop down:",particleIndex, particleNames, EditorStyles.popup);
		particlePrefab =(GameObject)EditorGUILayout.ObjectField("ParticlePrefab",particlePrefab,typeof(GameObject),false);
		if(GUI.Button (new Rect(260,200,50,30), "Save")){
			Load();
			if(particleConfig==null){
				EditorUtility.DisplayDialog("Failed: ", "Particle Config is missing, please create particle Config 1st","ok");
				return;
			}
			particleConfig.particles.Set((ParticleEffect)particleEffects.GetValue(particleIndex),particlePrefab);
			Save();
			Debug.Log("save");
		}

		if(GUI.Button (new Rect(180,200,60,30), "Refresh")){
			AssetDatabase.Refresh();
		}

		GUILayout.EndArea();
	}

	private void Load(){
		particleConfig = (ParticleConfigHolder)Resources.Load("Config/ParticleConfig");
	}

	private void Save(){
		EditorUtility.SetDirty(particleConfig);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = particleConfig;
	}

	private void CreateParticleEnum( string newParticleName, string fileName,string folderDirectorySavePath ){
		string path =folderDirectorySavePath + fileName + ".cs";
		string[] particleNames = Enum.GetNames(typeof(ParticleEffect));

		foreach(string particleName in particleNames ){
			if(particleName.Equals(newParticleName,StringComparison.Ordinal)){
				EditorUtility.DisplayDialog("failed: ", "Particle name: " + newParticleName + " already exist! ","ok");
				return;
			}
		}

		System.Array.Resize(ref particleNames,particleNames.Length+1);
		particleNames[particleNames.Length-1] = newParticleName;
		ParticleEffect[] particleEffectValue = (ParticleEffect[])Enum.GetValues(typeof(ParticleEffect));
		int len = particleNames.Length;
		int count = 0;
		
		if(File.Exists(path)){
			AssetDatabase.DeleteAsset(path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append("//" + fileName +" LIST \n");
		sb.Append("public enum " + fileName +"{\n");
		
		foreach( string particleName in particleNames ){
			if(len == 1){
				sb.Append("\t\t"+particleName+ "\t\t\t" + "=" + "\t\t\t " + count +"\n" );
			}else{
				if(count<(len - 1)){
					//sb.Append("\t\t"+particleName+","+"\n");
					sb.Append("\t\t"+particleName + "\t\t\t" + "=" + "\t\t\t " + count +" , " + "\n" );
				}else{
					//sb.Append("\t\t"+particleName+"\n");
					sb.Append("\t\t"+particleName + "\t\t\t" + "=" + "\t\t\t " + count +"\n" );
				}
				count++;
			}			
		}
		sb.Append("}");
		
		File.WriteAllText(path,sb.ToString());
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		particleEffects = Enum.GetValues(typeof(ParticleEffect));
		particleCount = particleEffects.Length;
	}
}

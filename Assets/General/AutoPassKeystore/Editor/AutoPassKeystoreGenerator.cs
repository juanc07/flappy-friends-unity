using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;


//[InitializeOnLoad]
public class AutoPassKeystoreGenerator:EditorWindow {
	private string password;
	private string aliasPassword;

	[MenuItem("Custom Editor/Autokeystore/Setup...")]
	public static void MenuItemSetup(){
		EditorWindow.GetWindow(typeof(AutoPassKeystoreGenerator));
	}

	[PostProcessScene]
	static AutoPassKeystoreGenerator(){
		ApplyKeyConfig();
	}

	private void OnGUI(){
		// Title
		GUILayout.BeginArea(new Rect(20, 20, position.width - 40, position.height));
		GUILayout.Label("Autokeystore Setup", EditorStyles.boldLabel);
		GUILayout.Space(10);

		password = EditorGUILayout.TextField("password: ", password);
		aliasPassword = EditorGUILayout.TextField("aliasPassword: ",aliasPassword);
		
		// Setup button
		if (GUILayout.Button("Save keyStoreConfig")) {
			Create();
		}else if (GUILayout.Button("Apply")) {
			ApplyKeyConfig();
			this.Close();
		}
		GUILayout.EndArea();
	}

	private void Create(){
		KeystoreInfo keyinfo = new KeystoreInfo();
		keyinfo.keystorePassword = password;
		keyinfo.keystoreAliasPassword = aliasPassword;
		
		
		KeystoreInfoHolder keystoreInfoHolder = new KeystoreInfoHolder();
		keystoreInfoHolder.keystoreInfo = keyinfo;
		
		
		AssetDatabase.CreateAsset(keystoreInfoHolder,"Assets/Resources/Config/keystoreInfoHolder.asset");
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = keystoreInfoHolder;
	}

	private static void ApplyKeyConfig(){
		Object obj = Resources.Load("Config/keystoreInfoHolder",typeof(KeystoreInfoHolder));
		if(obj!=null){
			KeystoreInfoHolder keystoreInfoHolder =(KeystoreInfoHolder)obj;
			PlayerSettings.keyaliasPass = keystoreInfoHolder.keystoreInfo.keystoreAliasPassword;
			PlayerSettings.keystorePass = keystoreInfoHolder.keystoreInfo.keystorePassword;
			//Debug.Log("apply keystore config now!!");
		}
	}
}

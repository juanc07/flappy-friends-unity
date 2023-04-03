using UnityEngine;
using System.Collections;
using UnityEditor;
using System;


[CustomEditor(typeof(ParticleConfigHolder))]
public class ParticleConfigEditor : Editor {

	private static bool showParticle;

	public override void OnInspectorGUI(){
		showParticle =  EditorGUILayout.Foldout(showParticle,"Show Particle");

		ParticleConfigHolder.ParticleDictionary particleDictionary = ((ParticleConfigHolder)target).particles;
		string[] particleNames = Enum.GetNames(typeof(ParticleEffect));
		ParticleEffect[] particleEffectValue = (ParticleEffect[])Enum.GetValues(typeof(ParticleEffect));
		int particleCount = particleNames.Length;

		if(showParticle){
			EditorGUI.indentLevel++;
			for(int index=0;index<particleCount;index++){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(particleNames[index]);
				ParticleEffect effect = particleEffectValue[index];
				particleDictionary.Set( effect,(GameObject)EditorGUILayout.ObjectField(particleDictionary.Get(effect),typeof(GameObject),false));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.BeginHorizontal();
			//Color color = GUI.color;
			GUI.color = Color.red;
			if(GUILayout.Button("Reset")){
				particleDictionary.Clear();
				AssetDatabase.Refresh();
			}

			GUI.color = Color.green;
			if(GUILayout.Button("Save")){
				EditorUtility.SetDirty(target as ParticleConfigHolder);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}

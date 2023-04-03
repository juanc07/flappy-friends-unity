using UnityEngine;
using System.Collections;

public class FbBridgeManagerCaller : MonoBehaviour {

	private FBBridgeManager fbmanager;

	// Use this for initialization
	void Awake (){
		fbmanager = FBBridgeManager.GetInstance();
		if(!fbmanager.isInit){
			fbmanager.Init();
		}
	}
}

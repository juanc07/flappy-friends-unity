using UnityEngine;
using System.Collections;

public class LoadGameButton : MonoBehaviour {

	private GPSArtOfByte gps;
	private FBBridgeManager fbmanager;
	// Use this for initialization
	void Start () {
		fbmanager = FBBridgeManager.GetInstance();
		gps = GPSArtOfByte.GetInstance();
	}
	
	private void OnClick(){
		if(fbmanager.isInit){
			gps.LoadGame();
		}
	}
}

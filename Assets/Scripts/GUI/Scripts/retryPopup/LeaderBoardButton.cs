using UnityEngine;
using System.Collections;

public class LeaderBoardButton : MonoBehaviour {

	private GPSArtOfByte gps;
	private FBBridgeManager fbmanager;

	// Use this for initialization
	void Start () {
		fbmanager = FBBridgeManager.GetInstance();
		gps = GPSArtOfByte.GetInstance();
		AddEventListner();
	}

	private void OnDestroy(){
		RemoveEventListener();
	}

	private void AddEventListner(){
		fbmanager.OnFaceBookInit+=OnFaceBookInit;
		gps.OnGPSConnectionComplete+=OnGPSConnectionComplete;
		gps.OnGPSConnectionFailed+=OnGPSConnectionFailed;
	}

	private void RemoveEventListener(){
		fbmanager.OnFaceBookInit-=OnFaceBookInit;
		gps.OnGPSConnectionComplete-=OnGPSConnectionComplete;
		gps.OnGPSConnectionFailed-=OnGPSConnectionFailed;
	}

	private void OnGPSConnectionComplete(){

	}

	private void OnGPSConnectionFailed(){
		/*if(fbmanager.isInit){
			gps.SignIn();
		}*/
	}

	private void OnFaceBookInit(){
		if(fbmanager.isInit){
			if(gps.connectionState == GPSArtOfByte.ConnectionState.Disconnected){
				gps.SignIn();
				//Debug.Log("sign in gps");
			}
		}
	}
	
	private void OnClick(){
		if(fbmanager.isInit){
			if(gps.connectionState == GPSArtOfByte.ConnectionState.Connected ){
				gps.ShowLeaderBoard();
				//Debug.Log("Show Leaderboard");
			}else{
				gps.SignIn();
				gps.ShowLeaderBoard();
			}
		}
	}
}

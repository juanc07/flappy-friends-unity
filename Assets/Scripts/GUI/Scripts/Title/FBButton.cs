using UnityEngine;
using System.Collections;

public class FBButton : MonoBehaviour {

	private FBBridgeManager fbmanager;

	// Use this for initialization
	void Start () {
		fbmanager = FBBridgeManager.GetInstance();		
		AddEventListener();
	}
	
	private void OnClick(){
		/*if(!fbmanager.isInit){
			fbmanager.Init();
		}else if(!fbmanager.isLogin){
			fbmanager.Login();
		}else{
			fbmanager.DialogFeed();
		}*/

		if(fbmanager.isLogin){
			fbmanager.DialogFeed();
		}else{
			fbmanager.Login();
		}
	}


	private void OnDestroy(){
		RemoveEventListener();
	}
	
	private void AddEventListener(){
		if(fbmanager!=null){
			//fbmanager.OnFaceBookInit+=OnFaceBookInit;
			fbmanager.OnFaceBookLogin+=OnFaceBookLogin;
		}
	}
	
	private void RemoveEventListener(){
		if(fbmanager!=null){
			//fbmanager.OnFaceBookInit-=OnFaceBookInit;
			fbmanager.OnFaceBookLogin-=OnFaceBookLogin;
		}
	}
	
	/*private void OnFaceBookInit(){
		if(!fbmanager.isLogin){
			fbmanager.Login();
		}
	}*/
	
	private void OnFaceBookLogin(){
		fbmanager.DialogFeed();
	}
}

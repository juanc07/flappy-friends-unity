using UnityEngine;
using System.Collections;

public class TitleOptionButton : MonoBehaviour{

	public GameObject gameUImanagerPrefab;
	private GameUIManager gameUImanager;
	
	// Use this for initialization
	void Start (){
		gameUImanager =  GameObject.FindObjectOfType(typeof(GameUIManager)) as GameUIManager;
		if(gameUImanager==null){
			GameObject gameUImanagerObj =  Instantiate(gameUImanagerPrefab) as GameObject;
			gameUImanagerObj.name = "GameUImanager";
			gameUImanager = gameUImanagerObj.GetComponent<GameUIManager>();
		}
	}
	
	private void OnClick(){
		gameUImanager.ShowHideOption(true);
	}
}

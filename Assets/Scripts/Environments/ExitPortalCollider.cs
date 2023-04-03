using UnityEngine;
using System.Collections;

public class ExitPortalCollider : MonoBehaviour {

	private GameDataManager gameDataManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
	}
	
	private void OnTriggerEnter(Collider other){
		//Debug.Log( " check red portal collided with " + other.tag );
		if(other.tag == "HeroFeet" || other.tag == "Mario" || other.tag == "Hero"){
			if(!gameDataManager.IsLevelComplete && !gameDataManager.player.IsDead){
				gameDataManager.IsLevelComplete = true;
				//Debug.Log("Show level complete now!");
			}
		}
	}
}

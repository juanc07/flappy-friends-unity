using UnityEngine;
using System.Collections;

public class DeathGroundCollider : MonoBehaviour {

	private GameDataManager gameDataManager;
	private LevelManager levelManager;
	private MarioController marioController;
	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		levelManager = GameObject.FindObjectOfType(typeof(LevelManager)) as LevelManager;
		marioController = levelManager.hero.gameObject.GetComponent<MarioController>();
	}
	
	private void OnTriggerEnter( Collider col ){
		//Debug.Log("Ground collider hit " + col.gameObject.tag);
		if(col.gameObject.tag =="Hero" && !marioController.isDead && !gameDataManager.player.IsDead ){
			gameDataManager.player.HP = 0;
			gameDataManager.player.IsDead = true;
			marioController.isDead =true;
			marioController.ApplyGravity =false;
		}else if(col.gameObject.tag =="HeroFeet" && !marioController.isDead && !gameDataManager.player.IsDead ){
			gameDataManager.player.HP = 0;
			gameDataManager.player.IsDead = true;
			marioController.isDead =true;
			marioController.ApplyGravity =false;
		}

		if(col.gameObject.tag =="Mushroom"){
			Destroy(col.gameObject);
		}
	}
}

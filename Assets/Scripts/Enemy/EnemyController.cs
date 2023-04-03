using UnityEngine;
using System.Collections;

public class EnemyController : HeroController{

	public int hp;
	private int originalHp;
	private GameDataManager gameDataManager;

	public override void Start(){
		base.Start();
		gameDataManager = GameDataManager.GetInstance();
		isDestroyBrick =false;
		originalHp = hp;
		AddEventListener();
	}

	private void AddEventListener(){
		gameDataManager.OnGameRestart+= OnGameRestart;
	}
	
	private void RemoveEventListener(){
		if(gameDataManager!=null){
			gameDataManager.OnGameRestart-= OnGameRestart;
		}
	}
	
	private void OnDestroy(){
		RemoveEventListener();
		//Debug.Log("on destory hammer bro controller");
	}
	
	private void OnGameRestart(){
		isDead = false;
		hp = originalHp;
	}


	public override void Hit(){
		base.Hit();
		if(hp> 0){
			hp--;
			if(hp<=0){
				isDead =true;
			}
		}
		//Debug.Log("overried EnemyController hit");
	}

	public override void LimitSpeed ()
	{
		base.LimitSpeed ();
		//Debug.Log(" isRunning "+ isRunning + " isWalking " + isWalking + " isIdle " + isIdle );
	}
}
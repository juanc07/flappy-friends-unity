using UnityEngine;
using System.Collections;

public class MarioController : HeroController{

	private GameDataManager gameDataManager;
	private ParticleManager particleManager;

	private Vector3 initialPosition;
	public int hp;
	//private GameTimer gameTimer;

	private float angle=0;
	public float diveRotSpeed =40f;
	public float takeOffRotSpeed =50f;

	private SoundManager soundManager;

	public override void Start(){
		base.Start();
		gameDataManager = GameDataManager.GetInstance();
		soundManager = SoundManager.GetInstance();
		particleManager = ParticleManager.GetInstance();

		//gameTimer =  GameObject.FindObjectOfType(typeof(GameTimer)) as GameTimer;
		gameDataManager.player.HP = hp;
		initialPosition = this.gameObject.transform.position;
		AddListener();
	}

	private void OnDestroy(){
		RemoveListener();
	}
	
	private void AddListener(){
		gameDataManager.player.OnPlayerRevive+=OnPlayerRevive;
		gameDataManager.player.OnHpUpdate+=OnHpUpdate;
		gameDataManager.OnLevelStart+=OnLevelStart;
		//gameTimer.OnTimeOut+=OnTimeOut;
	}
	
	private void RemoveListener(){
		gameDataManager.player.OnPlayerRevive-=OnPlayerRevive;
		gameDataManager.player.OnHpUpdate-=OnHpUpdate;
		gameDataManager.OnLevelStart-=OnLevelStart;
		//gameTimer.OnTimeOut-=OnTimeOut;
	}

	/*
	private void OnTimeOut(){
		if(!gameDataManager.player.IsDead && !isDead){
			isDead =true;
			gameDataManager.player.IsDead = true;
		}
	}*/

	private void OnLevelStart(){
		ApplyGravity =true;
		alwaysMoveRight =true;
	}

	private void OnHpUpdate(){
		if(gameDataManager.player.HP <= 0){
			HitDead();
		}
	}

	private void OnPlayerRevive(){
		ResetState();
		ApplyGravity =true;

		//soon you will have a checkpoint 
		//find nearest checkpoint and get that check point position and use that instead
		this.gameObject.transform.position = initialPosition;
	}

	public override void Hit(){
		if(gameDataManager.player.HP > 0 && !isHit){
			gameDataManager.player.HP--;
			if(hp<=0){
				hp = 0; 
			}else{
				hp = gameDataManager.player.HP; 
			}

			if(gameDataManager.player.HP<=0 && !gameDataManager.player.IsDead && !isDead){
				isDead =true;
				gameDataManager.player.IsDead = true;
			}
			//Debug.Log("overried hit");
		}
		base.Hit();
	}

	private void HitDead(){
		if(!gameDataManager.player.IsDead && !isDead){
			particleManager.CreateParticle(ParticleEffect.Hit2,this.gameObject.transform.position,new Vector3(5f,5f,5f));
			soundManager.PlaySfx(SFX.hit3);
			isDead =true;
			gameDataManager.player.IsDead = true;
		}
	}

	public override void OnControllerColliderHit (ControllerColliderHit hit)
	{
		base.OnControllerColliderHit (hit);
		HitDead();
		//Debug.Log(" parrot hit " + hit.transform.tag );
	}

	public override void Update ()
	{
		base.Update ();
		//Debug.Log("check if falling " + isFalling);
		if(isFalling){
			angle -= diveRotSpeed * Time.deltaTime;
			transform.rotation  = Quaternion.Euler(angle,270,180);
		}else{
			if(angle<-5){
				//soundManager.PlaySfx(SFX.jump);
				angle += takeOffRotSpeed * Time.deltaTime;
				transform.rotation  = Quaternion.Euler(angle,270,180);
			}
		}

		//this.gameObject.transform.ro
		//Debug.Log( " check y " + this.gameObject.transform.position.y );
	}
}

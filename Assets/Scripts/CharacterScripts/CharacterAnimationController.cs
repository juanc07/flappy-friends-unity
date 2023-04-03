using UnityEngine;
using System.Collections;

public abstract class CharacterAnimationController : MonoBehaviour {

	public GameObject model;
	public HeroController heroController;
	private bool hasPlayedJump =false;
	public Animation modelAnimation{set;get;}
	private bool isHitPlayed =false;
	public bool isDeathPlayed =false;

	public enum Animations{idle,walk,run,jump,hit,falling,falling2,death}

	private GameDataManager gameDataManager;

	// Use this for initialization
	public virtual void Start () {
		gameDataManager = GameDataManager.GetInstance();
		modelAnimation = model.gameObject.GetComponent<Animation>();
		AddEventListener();
	}

	private void AddEventListener(){
		heroController.HitComplete += OnHitComplete;
		gameDataManager.OnGameRestart+= OnGameRestart;
	}
	
	private void RemoveEventListener(){
		heroController.HitComplete -= OnHitComplete;
		if(gameDataManager!=null){
			gameDataManager.OnGameRestart-= OnGameRestart;
		}
	}
	
	public virtual void OnDestroy(){
		RemoveEventListener();
		//Debug.Log("on destory hammer bro controller");
	}
	
	public virtual void OnGameRestart(){
		PlayWalk();
		isDeathPlayed =false;
	}

	private void OnHitComplete(){
		isHitPlayed = false;
	}
	
	// Update is called once per frame
	void Update (){
		if(heroController.isFacingLeft){
			FaceLeft();
		}else if(heroController.isFacingRight){
			FaceRight();
		}

		if(heroController.isHit && !isHitPlayed ){
			PlayHit();
		}else if(heroController.isDead && !isDeathPlayed ){
			PlayDeath();
		}else{
			if(heroController.isHit || heroController.isDead){
				return;
			}

			if(heroController.isInAir){
				if((!hasPlayedJump && !heroController.isFalling) || (heroController.isHitWall && !heroController.isFalling)){
					PlayJump();
				}else{
					PlayFalling2();
				}
			}else{
				if(heroController.isIdle){
					PlayIdle();
				}else if(heroController.isWalking){
					if(heroController.alwaysRun){
						PlayRun();
					}else{
						PlayWalk();
					}

				}else if(heroController.isRunning){
					PlayRun();
				}
			}
		}
	}


	public virtual void PlayHit(){
		isHitPlayed =true;
	}

	public virtual void PlayDeath(){
		isDeathPlayed = true;
	}

	public virtual void PlayIdle(){
		hasPlayedJump =false; 
	}

	public virtual void PlayWalk(){
		hasPlayedJump =false; 
	}

	public virtual void PlayRun(){
		hasPlayedJump =false; 
	}

	public virtual void PlayJump(){
		hasPlayedJump =true;
	}

	public virtual void PlayFalling(){
		
	}

	public virtual void PlayFalling2(){
		
	}


	public void FaceLeft(){
		Quaternion tempRotation;
		tempRotation =  Quaternion.Euler(0, 270f, 0);
		model.gameObject.transform.rotation = tempRotation;
	}

	public void FaceRight(){
		Quaternion tempRotation;
		tempRotation =  Quaternion.Euler(0, 90f, 0);
		model.gameObject.transform.rotation = tempRotation;
	}
}

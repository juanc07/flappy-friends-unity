using UnityEngine;
using System.Collections;

public abstract class AIController : MonoBehaviour {

	private HeroController heroController;
	private bool isTurning=false;
	public HitController hitController;
	public float moveDelay = 1f;
	public float turnDelay = 1f;
	//private bool isActivated =false;
	private bool isStop = false;

	//private LevelManager levelManager;

	// Use this for initialization
	public virtual void Start (){
		heroController = this.gameObject.GetComponent<HeroController>();
		//levelManager = GameObject.FindObjectOfType( typeof(LevelManager) ) as LevelManager;
		MoveRight();
	}

	public  HeroController AIHeroController{
		get{return heroController;}
	}
	
	// Update is called once per frame
	public virtual void Update (){
		if(hitController.currentHitObject!=null){
			if(hitController.currentHitObject.tag == "Enemy"){
				CheckWhereToGo();
			}else if(hitController.currentHitObject.tag == "Hero" ){
				CheckWhereToGo();
			}else if(hitController.currentHitObject.tag == "Mushroom" ){
				CheckWhereToGo();
			}else{
				if(heroController.isHitLeftSide || heroController.isHitRightSide){				
					StopMoving();
				}
			}
			//Debug.Log("ai there's a currentHitObject but its not belong to the check collision" + hitController.currentHitObject);
		}else{				
			if(heroController.isHitLeftSide || heroController.isHitRightSide){
				//CheckWhereToGo();
				StopMoving();
			}
		}
	}
	
	public void CheckWhereToGo(){
		//Debug.Log("ai Check where to go");
		if(heroController.isFacingRight && !isTurning){
			isTurning =true;
			isStop = false;
			heroController.isWalking =true;
			//Debug.Log("ai trying to move left");
			MoveLeft();

		}else if(heroController.isFacingLeft && !isTurning){
			isTurning =true;
			isStop = false;
			heroController.isWalking =true;
			MoveRight();
		}
	}

	private void RefreshDelay(){
		isTurning =false;
	}

	private void MoveLeft(){
		heroController.speed = 0;
		heroController.isRightBtnPress =false;
		heroController.isLeftBtnPress =true;	
		heroController.isHitLeftSide = false;
		heroController.isHitRightSide = false;
		Invoke("RefreshDelay", turnDelay);
	}

	private void MoveRight(){
		heroController.speed = 0;
		heroController.isRightBtnPress =true;
		heroController.isLeftBtnPress =false;
		heroController.isHitLeftSide =false;
		heroController.isHitRightSide =false;
		Invoke("RefreshDelay", turnDelay);
	}

	private void StopMoving(){
		if(!isStop){
			//Debug.Log("ai stop moving!!");
			isStop =true;
			heroController.StopMoving();
			Invoke("CheckWhereToGo", moveDelay);
		}
	}
}

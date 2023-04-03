using UnityEngine;
using System.Collections;
using System;

public abstract class HitController : MonoBehaviour {
	
	public GameObject obj;	
	private HeroController marioController;
	public GameObject currentHitObject;
	public HeroController modelController;
	private LevelManager levelManager;	
	
	// Use this for initialization
	void Start (){
		levelManager = GameObject.FindObjectOfType( typeof(LevelManager) )as LevelManager;
		marioController = levelManager.hero.gameObject.GetComponent<HeroController>();
	}

	public HeroController GetMarioController{
		get{return marioController;}
	}
	
	public virtual void OnTriggerEnter(Collider col){
		if(modelController.isDead){
			return;
		}
		
		if(col.gameObject.GetComponent<HitController>()!=null){
			HeroController collidedHeroController = col.gameObject.GetComponent<HitController>().obj.gameObject.GetComponent<HeroController>() as HeroController;
			
			if(collidedHeroController!=null){
				if(modelController.id.Equals(collidedHeroController.id,StringComparison.Ordinal)){
					//Debug.Log("this object hit himself ");
				}else{

					if(col.gameObject.tag=="Hero"){
						currentHitObject = col.gameObject;
					}else if(col.gameObject.tag=="Mario"){
						currentHitObject = col.gameObject;
					}else if(col.gameObject.tag=="Enemy"){
						currentHitObject = col.gameObject;
						//Debug.Log("Enemy hit other " + col.gameObject.tag);
					}else if(col.gameObject.tag=="Mushroom"){
						currentHitObject = col.gameObject;
						//Debug.Log("Enemy hit other " + col.gameObject.tag);
					}else if(col.gameObject.tag=="Crate"){
						currentHitObject = col.gameObject;
						//Debug.Log("Enemy hit other " + col.gameObject.tag);
					}else if(col.gameObject.tag=="HeroFeet" && marioController.isFalling ){
						currentHitObject = col.gameObject;
						//Debug.Log("Enemy hit other " + col.gameObject.tag);
					}
				}
			}
		}else{
			currentHitObject = col.gameObject;
		}
	}
	
	public virtual void OnTriggerExit(Collider col){
		currentHitObject =null;
		if(modelController.isDead){
			return;
		}
	}
	
	public virtual void OnTriggerStay(Collider col){
		if(modelController.isDead){
			return;
		}		
		//Debug.Log("OnTriggerStay " + col.gameObject.tag);
		if(col.gameObject.tag=="Hero"){
			currentHitObject = col.gameObject;
		}
	}
}

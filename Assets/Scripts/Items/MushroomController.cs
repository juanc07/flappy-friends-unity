using UnityEngine;
using System.Collections;

public class MushroomController : HeroController{
	
	public override void Start(){
		base.Start();
		isDestroyBrick =false;
	}
	
	
	public override void Hit(){
		base.Hit();
	}
	
	public override void LimitSpeed ()
	{
		base.LimitSpeed ();
	}

	public override void OnControllerColliderHit (ControllerColliderHit hit)
	{
		base.OnControllerColliderHit (hit);
		if(isHitLeftSide || isHitRightSide){
			//Debug.Log( " hit controller  " + hit.controller.gameObject.tag );
		}

	}
}
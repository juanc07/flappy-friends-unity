using UnityEngine;
using System.Collections;
using System;

public class GoombaHitController : HitController {
	public override void OnTriggerEnter (Collider col)
	{
		base.OnTriggerEnter (col);

		if(currentHitObject!=null){
			if(currentHitObject.tag=="HeroFeet" && GetMarioController.isFalling){
				//Debug.Log("goomba detect hero feet");
				modelController.Hit();
				//GetMarioController.Bounce();
			}else if(currentHitObject.tag=="Hero" && !GetMarioController.isFalling){
				GetMarioController.Hit();
			}
		}
	}

	public override void OnTriggerStay (Collider col)
	{
		base.OnTriggerStay (col);

		if(currentHitObject!=null){
			if(currentHitObject.tag=="Hero"){
				GetMarioController.Hit();
			}
		}
	}
}

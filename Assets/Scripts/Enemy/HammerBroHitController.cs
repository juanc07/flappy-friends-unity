using UnityEngine;
using System.Collections;
using System;

public class HammerBroHitController : HitController {
	public override void OnTriggerEnter (Collider col)
	{
		base.OnTriggerEnter (col);
		if(currentHitObject!=null){
			if(currentHitObject.tag=="HeroFeet" && GetMarioController.isFalling){
				modelController.Hit();
				modelController.StopMoving();
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

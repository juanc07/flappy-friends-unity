using UnityEngine;
using System.Collections;
using System;

public class MushroomHitController : HitController{

	public override void OnTriggerEnter (Collider col)
	{
		base.OnTriggerEnter (col);

		//Debug.Log( "MushroomHitController OnTriggerEnter " + col.gameObject.tag );
		if(currentHitObject!=null){
			if(currentHitObject.tag == "Hero"){
				Destroy(obj);
				//Debug.Log("destroy mushroom");
			}else if(currentHitObject.tag == "HeroFeet"){
				Destroy(obj);
				//Debug.Log("destroy mushroom");
			}else if(currentHitObject.tag == "Ground"){
				Destroy(obj);
				//Debug.Log("destroy mushroom");
			}
		}
	}

	public override void OnTriggerExit (Collider col)
	{
		base.OnTriggerExit (col);

	}

	public override void OnTriggerStay (Collider col)
	{
		base.OnTriggerStay (col);
	}
}

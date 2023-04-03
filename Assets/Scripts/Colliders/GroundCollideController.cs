using UnityEngine;
using System.Collections;

public class GroundCollideController : MonoBehaviour {

	public HeroController heroController;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider other) {

	}
	
	private void OnTriggerExit(Collider col) {
		if(col.gameObject.tag=="Brick"){
			//heroController.isInAir =true;
			//print("GroundCollideController No longer in contact with " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Wall"){
			//heroController.isInAir =true;
			//print("GroundCollideController No longer in contact with " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Ground"){
			//heroController.isInAir =true;
			//print("GroundCollideController No longer in contact with " + col.gameObject.tag);
		}
	}


	private void OnTriggerStay(Collider col){

		/*
		if(col.gameObject.tag=="PushCollider"){

		}else if(col.gameObject.tag=="Hero"){
			
		}else{
			Debug.Log("isGrounded grounded!");
		}*/

		if(col.gameObject.tag=="Brick"){
			//print("GroundCollideController contact with " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Wall"){
			//print("GroundCollideController contact with " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Ground"){
			//print("GroundCollideController contact with " + col.gameObject.tag);
		}else{
			//heroController.isInAir =false;
		}
	}
}

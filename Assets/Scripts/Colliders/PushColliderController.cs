using UnityEngine;
using System.Collections;

public class PushColliderController : MonoBehaviour {

	public HeroController heroController;
	public float pushPower = 2f;
	private BoxCollider boxCollider;

	// Use this for initialization
	void Start () {
		boxCollider = this.gameObject.GetComponent<BoxCollider>();
	}

	private void Update(){
		if(heroController.isFacingRight){
			SwitchGrabCollider(0);
		}else if(heroController.isFacingLeft){
			SwitchGrabCollider(1);
		}
	}

	public void SwitchGrabCollider(int dir){
		Vector3 tempBoxColliderCenter = boxCollider.center;
		if(dir==0){
			tempBoxColliderCenter.x=0.6f;
		}else if(dir==1){
			tempBoxColliderCenter.x=-0.6f;
		}		
		boxCollider.center = tempBoxColliderCenter;
	}

	private void OnTriggerEnter(Collider col){
		if(col.gameObject.tag=="Brick"){
			heroController.speed=0;
			//Debug.Log("push collider hit: " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Wall"){
			heroController.isHitWall =true;
			heroController.speed=0;
			//Debug.Log("push collider hit: " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Crate"){
			//heroController.speed=0;
		}
	}

	private void OnTriggerExit(Collider col){
		if(col.gameObject.tag=="Wall"){
			heroController.isHitWall =false;
			//Debug.Log("push collider hit: " + col.gameObject.tag);
		}
	}

	void OnTriggerStay(Collider col) {
		if(col.gameObject.tag=="Brick"){
			heroController.speed=0;
			//Debug.Log("push collider hit stay: " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Wall"){
			heroController.isHitWall =true;
			heroController.speed=0;
			//Debug.Log("push collider hit stay: " + col.gameObject.tag);
		}else if(col.gameObject.tag=="Crate"){
			//if(heroController.isHoldingAction){
				Push(col);
			//}
		}
	}

	private void Push(Collider hit){
		Rigidbody body = hit.collider.attachedRigidbody;
		
		if(body == null || body.isKinematic){
			return;
		}

		Vector3 direction;
		if(heroController.isMovingRight){
			direction = new Vector3(1f,0,0);
			body.velocity = direction * pushPower;
		}else if(heroController.isMovingLeft){
			direction = new Vector3(-1,0,0);
			body.velocity = direction * pushPower;
		}


		//Debug.Log("push");
	}
}

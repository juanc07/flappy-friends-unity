using UnityEngine;
using System.Collections;

public class GrabColliderController : MonoBehaviour {

	public GameObject hero;
	private HeroController heroController;
	public float offsetX=0.75f;
	public GameObject itemHolder;
	public float throwingPower = 30f;
	private SphereCollider sphereCollider;

	// Use this for initialization
	void Start (){
		heroController = hero.gameObject.GetComponent<HeroController>();
		sphereCollider = this.gameObject.GetComponent<SphereCollider>();
	}
	
	private void OnTriggerEnter(Collider col) {
		//Debug.Log( "grabCollider detect: " + col.gameObject.tag );
	}

	private void OnTriggerStay(Collider col) {
		//Debug.Log( "grabCollider detect: " + col.gameObject.tag );
		if(col.gameObject.tag=="Crate" && heroController.isHoldingAction 
		   && !heroController.isHoldingSomething && heroController.isIdle){
			//Debug.Log("grab now!");
			Grab(col);
		}
	}

	private void Grab(Collider col){
		Rigidbody body = col.collider.attachedRigidbody;
		if(body==null && body.isKinematic){
			return;
		}

		col.gameObject.GetComponent<BoxCollider>().isTrigger =true;
		col.gameObject.isStatic =true;
		body.isKinematic =true;
		heroController.isHoldingSomething =true;
		body.useGravity = false;
		col.gameObject.transform.parent = itemHolder.gameObject.transform;
		
		Vector3 tempPosition = col.gameObject.transform.position;
		tempPosition.x = hero.gameObject.transform.position.x;
		tempPosition.y = hero.gameObject.transform.position.y + 2f;
		col.gameObject.transform.position = tempPosition;
	}

	private void ThrowObject(){
		if(heroController.isHoldingSomething){
			heroController.isThrow =false;
			heroController.isHoldingSomething =false;

			Transform child = itemHolder.gameObject.transform.GetChild(0);
			child.gameObject.isStatic =false;

			Rigidbody body = child.collider.attachedRigidbody;
			if(body==null && body.isKinematic){
				return;
			}


			child.gameObject.GetComponent<BoxCollider>().isTrigger = false;
			child.gameObject.transform.parent =null;
			body.isKinematic =false;
			body.useGravity = true;

			Vector3 throwDirection = new Vector3(0,0,0);
			if(heroController.isLookingUp){
				throwDirection = new Vector3(0,1f,0);
			}else if(heroController.isLookingDown){
				child.gameObject.transform.position = new Vector3(child.gameObject.transform.position.x,child.gameObject.transform.position.y - 4f,child.gameObject.transform.position.z);
				throwDirection = new Vector3(0,-1f,0);
			}else{
				if(heroController.isFacingRight){
					throwDirection = new Vector3(1f,0,0);
				}else if(heroController.isFacingLeft){
					throwDirection = new Vector3(-1f,0,0);
				}
			}

			body.velocity = throwDirection * throwingPower;
		}
	}

	private void Update(){
		if(heroController.isFacingRight){
			SwitchGrabCollider(0);
		}else if(heroController.isFacingLeft){
			SwitchGrabCollider(1);
		}

		if(heroController.isThrow && !heroController.isDead){
			ThrowObject();
		}
	}

	private void SwitchGrabCollider(int dir){
		Vector3 tempBoxColliderCenter = sphereCollider.center;
		if(dir==0){
			tempBoxColliderCenter.x=offsetX;
		}else if(dir==1){
			tempBoxColliderCenter.x=-offsetX;
		}		
		sphereCollider.center = tempBoxColliderCenter;
	}
}

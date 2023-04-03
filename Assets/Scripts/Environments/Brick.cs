using UnityEngine;
using System.Collections;

public class Brick:MonoBehaviour,IDestroyable{
	public bool isDestroyable;
	private Vector3 originalPosition;
	private Vector3 targetPosition;

	public float targetValue =1f;
	public float smooth =15f;
	public bool reset;
	private bool reachTarget;
	private bool endAnimationDown =false;
	public bool allowToMove =true;

	public GameObject originalBrick;
	public GameObject targetBrick;
	private bool isHitOnce = false;
	public bool hasTargetBrick =false;
	public bool hasItem = false;
	public int itemCount =1;

	public GameObject[] items;
	public float hitDelay= 0.3f;
	public bool isReady= true;

	private void Start(){
		originalPosition = this.gameObject.transform.position;
		if(hasTargetBrick){
			ShowHideOriginalBrick(true);
			ShowHideTargetBrick(false);
		}
	}

	private void ShowHideTargetBrick(bool val){
		targetBrick.SetActive(val);
	}

	private void ShowHideOriginalBrick(bool val){
		originalBrick.SetActive(val);
	}



	public void DestroyBlock(){
		if(isDestroyable){
			Destroy(this.gameObject);
		}
	}

	private void Update(){
		if(reset){
			this.gameObject.transform.position = originalPosition;
			reachTarget =false;
		}else{
			if(isHitOnce && isReady ){
				if(!reachTarget){
					MoveUp();
				}else{
					MoveDown();
				}
			}
		}
	}

	public void AnimateBlock(){
		this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, targetPosition,smooth * Time.deltaTime );
	}

	private void Bounce(float speed){
		//targetPosition = new Vector3(originalPosition.x, Mathf.PingPong(Time.time * speed, 14) - 4, originalPosition.z);
		//targetPosition = new Vector3(originalPosition.x, Mathf.Sin(Time.time * speed) * 7 + 3, originalPosition.z);
		//targetPosition = new Vector3(originalPosition.x,originalPosition.y + Mathf.Sin(Time.time * speed) * targetValue, originalPosition.z);
	}

	public void MoveUp(){
		if(allowToMove){
			isHitOnce = true;
			reachTarget = false;
			endAnimationDown =false;
			targetPosition = new Vector3(originalPosition.x,originalPosition.y + targetValue,originalPosition.z);
			this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, targetPosition,smooth * Time.deltaTime );
			if(this.gameObject.transform.position.y >=  (originalPosition.y + targetValue) - 0.5f){
				reachTarget =true;
				//Debug.Log("go down now");
			}
		}
	}

	public void MoveDown(){
		if(allowToMove && reachTarget && !endAnimationDown){
			targetPosition = new Vector3(originalPosition.x,originalPosition.y,originalPosition.z);
			this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, targetPosition,smooth * Time.deltaTime );
			//float targetDown = (int)(targetPosition.y + 0.5f);
			float targetDown = targetPosition.y + 0.00950f;
			//float targetDown = targetPosition.y;
			//Debug.Log("brick down position " + this.gameObject.transform.position.y + "target check down position " + targetDown);
			if(this.gameObject.transform.position.y<= targetDown){
				endAnimationDown = true;
				if(hasItem){
					itemCount--;
					if(itemCount<=0){
						hasItem =false;
					}
					//Debug.Log("show item");
					SummonItem();
				}else{
					//Debug.Log("no more item");
				}

				if(hasTargetBrick && !hasItem){
					ShowHideOriginalBrick(false);
					ShowHideTargetBrick(true);
					//Debug.Log("show target brick");
				}
				isReady = false;
				Invoke("RefreshDelay",hitDelay);
			}
		}
	}

	private void RefreshDelay(){
		isReady =true;
	}

	private void SummonItem(){
		Vector3 newPosition = this.gameObject.transform.position;
		newPosition.y += 1f; 
		Quaternion newRotation = this.gameObject.transform.rotation;
		/*Vector3 newScale = this.gameObject.transform.localScale;
		newScale.x *= 0.5f; 
		newScale.y *= 0.5f; 
		newScale.z *= 0.5f;*/

		if(items.Length > 0){
			GameObject item = Instantiate( items.GetValue(0) as Object,newPosition,newRotation) as GameObject;
			item.SetActive( true );
			item.transform.parent = this.gameObject.transform;
			//item.transform.localScale =  newScale;
		}
	}
}

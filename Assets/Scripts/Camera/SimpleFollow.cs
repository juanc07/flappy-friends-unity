using UnityEngine;
using System.Collections;

public class SimpleFollow : MonoBehaviour {

	public Transform target;
	public float distance;

	public bool hasSmoothing=false;
	public bool smoothX=false;
	public bool smoothY=false;
	public bool smoothZ=false;

	public float smoothing=0.5f;

	public float restrictUpY = 22f;
	public float restrictDownY = 11f;

	public bool followX = true;
	public bool followY =true;
	public bool followZ =true;

	public float offsetX=20f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update (){
		Vector3 tempPosition = transform.position;

		if(hasSmoothing){
			float currSmoothing = smoothing * Time.deltaTime;

			if(followX){
				if(smoothX){
					tempPosition.x = Mathf.Lerp(tempPosition.x, target.position.x + offsetX, currSmoothing);
				}else{
					tempPosition.x = target.position.x + offsetX;
				}
			}

			if(followY){
				if(smoothY){
					if(target.position.y >  restrictDownY ){
						tempPosition.y = Mathf.Lerp(tempPosition.y, target.position.y, currSmoothing);
					}
				}else{
					if(target.position.y >  restrictUpY){
						tempPosition.y = target.position.y;
					}
				}
			}

			if(followZ){
				if(smoothZ){
					tempPosition.z = Mathf.Lerp(tempPosition.z, target.position.z - distance, currSmoothing);
				}else{
					tempPosition.z = target.position.z -distance;
				}
			}
		}else{
			if(followZ){
				tempPosition.z = target.position.z -distance;
			}

			if(followY){
				tempPosition.y = target.position.y;
			}

			if(followX){
				tempPosition.x = target.position.x;
			}
		}
		transform.position = tempPosition;
	}
}

using UnityEngine;
using System.Collections;

public class BackgroundFollow : MonoBehaviour {
	
	public Transform target;
	public float distance;
	
	public bool hasSmoothing=false;
	public bool smoothX=false;	
	public float smoothing=0.5f;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update (){
		Vector3 tempPosition = transform.position;
		
		if(hasSmoothing){
			float currSmoothing = smoothing * Time.deltaTime;
			if(smoothX){
				tempPosition.x = Mathf.Lerp(tempPosition.x, target.position.x, currSmoothing);
			}else{
				tempPosition.x = target.position.x;
			}
		}else{
			tempPosition.x = target.position.x;
		}
		transform.position = tempPosition;
	}
}

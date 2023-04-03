using UnityEngine;
using System.Collections;

public class FixedPosition : MonoBehaviour {

	public bool isFixedX=false;
	public bool isFixedY=false;
	public bool isFixedZ=true;

	public bool cacheIntialX=false;
	public bool cacheIntialY=false;
	public bool cacheIntialZ=false;

	public float originX=0;
	public float originY=0;
	public float originZ=0;

	// Use this for initialization
	void Start (){
		if(cacheIntialX){
			originX = this.gameObject.transform.position.x;
		}

		if(cacheIntialY){
			originY = this.gameObject.transform.position.y;
		}

		if(cacheIntialZ){
			originZ = this.gameObject.transform.position.z;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 tempPosition = this.gameObject.transform.position;

		if(isFixedX){
			tempPosition.x =originX;
		}

		if(isFixedY){
			tempPosition.y =originY;
		}

		if(isFixedZ){
			tempPosition.z =originZ;
		}

		this.gameObject.transform.position = tempPosition;
	}
}

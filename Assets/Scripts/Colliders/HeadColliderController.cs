using UnityEngine;
using System.Collections;

public class HeadColliderController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag=="Brick"){
			Debug.Log("head collider hit brick");
			Destroy(col.gameObject);
		}
	}
}

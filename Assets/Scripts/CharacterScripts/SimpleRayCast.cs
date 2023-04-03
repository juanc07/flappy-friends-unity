using UnityEngine;
using System.Collections;

public class SimpleRayCast : MonoBehaviour {

	public float distance = 100f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 dir = transform.TransformDirection(Vector3.forward);
		/*Vector3 dir = transform.TransformDirection(Vector3.down);
		Debug.DrawLine(transform.position, dir, Color.red);
		if (Physics.Raycast(transform.position, dir, distance))
			print("There is something in front of the object!");
			*/

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit)){
			Vector3 distanceToGround = hit.point;
			Debug.Log(" distanceToGround " + distanceToGround);
		}			
	}
}

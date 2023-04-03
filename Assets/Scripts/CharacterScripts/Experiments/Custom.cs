using UnityEngine;
using System.Collections;

public class Custom:MonoBehaviour {

	public GameObject hero;
	public Custom(){
		//Debug.Log("yeah");
	}

	private void Start(){
		CharacterAnimationController c = hero.gameObject.GetComponent<CharacterAnimationController>();
		Debug.Log( "c " + c );
	}

}

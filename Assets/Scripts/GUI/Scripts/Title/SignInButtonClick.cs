using UnityEngine;
using System.Collections;

public class SignInButtonClick : MonoBehaviour {

	private GPSArtOfByte gps;
	// Use this for initialization
	void Start () {
		gps = GPSArtOfByte.GetInstance();
	}
	
	private void OnClick(){
		gps.GPSSignIn();
	}
}

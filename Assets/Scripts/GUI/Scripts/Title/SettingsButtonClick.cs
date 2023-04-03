using UnityEngine;
using System.Collections;

public class SettingsButtonClick : MonoBehaviour {

	public GameObject settingsPopup;

	// Use this for initialization
	void Start () {
	
	}
	
	private void OnClick(){
		settingsPopup.gameObject.SetActive(true);
	}
}

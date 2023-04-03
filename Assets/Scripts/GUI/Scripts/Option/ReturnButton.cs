using UnityEngine;
using System.Collections;

public class ReturnButton : MonoBehaviour {

	public GameObject optionPopupPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	private void OnClick(){
		optionPopupPanel.SetActive(false);
	}
}

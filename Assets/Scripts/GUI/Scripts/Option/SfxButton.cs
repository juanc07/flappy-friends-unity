using UnityEngine;
using System.Collections;

public class SfxButton : MonoBehaviour {
	
	private bool state=false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	private void Awake(){
		UIToggle toggle = GetComponent<UIToggle>();
		EventDelegate.Add(toggle.onChange, Toggle);
	}
	
	public void Toggle ()
	{
		if (enabled){
			if(!state){
				state =true;
				Debug.Log("enable sfx");
			}else{
				state =false;
				Debug.Log("disable sfx");
			}
		}
	}
}

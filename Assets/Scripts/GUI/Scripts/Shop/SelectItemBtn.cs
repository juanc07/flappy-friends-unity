using UnityEngine;
using System.Collections;

public class SelectItemBtn : MonoBehaviour {

	public ShopManagerController shopManagerController;

	// Use this for initialization
	void Start () {

	}
	
	private void OnClick(){
		shopManagerController.SelectItem();
	}
}

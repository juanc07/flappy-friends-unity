using UnityEngine;
using System.Collections;

public class BuyItemBtn : MonoBehaviour {

	public ShopManagerController shopManagerController;
	// Use this for initialization
	void Start () {
	
	}
	
	private void OnClick(){
		shopManagerController.BuyItem();
	}
}

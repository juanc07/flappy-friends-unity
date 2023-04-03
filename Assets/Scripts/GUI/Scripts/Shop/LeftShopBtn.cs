using UnityEngine;
using System.Collections;

public class LeftShopBtn : MonoBehaviour {

	public ShopManagerController shopManagerController;

	// Use this for initialization
	void Start () {
	
	}

	private void OnClick(){
		if(shopManagerController!=null){
			shopManagerController.Prev();
		}
	}
}

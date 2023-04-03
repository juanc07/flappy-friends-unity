using UnityEngine;
using System.Collections;

public class RightShopBtn : MonoBehaviour {

	public ShopManagerController shopManagerController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	private void OnClick(){
		if(shopManagerController!=null){
			shopManagerController.Next();
		}
	}
}

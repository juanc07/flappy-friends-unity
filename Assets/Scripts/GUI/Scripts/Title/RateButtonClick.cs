using UnityEngine;
using System.Collections;

public class RateButtonClick : MonoBehaviour {
	
	private void OnClick(){
		string urlString = "market://details?id=" + "com.gigadrillgames.slappybird3d";
		//string urlString = "https://play.google.com/store/apps/details?id=com.gigadrillgames.slappybird3d";
		//com.monsterpatties.fluffyfriends
		//string urlString = "market://details?id=" + "com.gigadrillgames.floppycow3d";
		Application.OpenURL(urlString);
	}
}

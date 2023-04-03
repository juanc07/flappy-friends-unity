using UnityEngine;
using System.Collections;

public class HammerBroAIController : AIController {

	private int countDownIdle=0;

	public override void Update ()
	{
		base.Update ();

		if( AIHeroController.isIdle ){
			countDownIdle++;
			if(countDownIdle > 30){
				CheckWhereToGo();
				countDownIdle = 0;
			}
		}
	}



}

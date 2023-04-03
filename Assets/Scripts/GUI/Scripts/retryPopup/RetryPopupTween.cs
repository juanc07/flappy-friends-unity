using UnityEngine;
using System.Collections;

public class RetryPopupTween : MonoBehaviour {

	protected AbstractGoTween _tween;
	protected float _tweenTimeScale = 1f;
	public GameObject retryBtn;
	public GameObject leaderboardBtn;
	public GameObject scoreBoard;


	// Use this for initialization
	void Start (){
		Vector3 initialPosition = scoreBoard.gameObject.transform.position;
		Vector3 newPosition = scoreBoard.gameObject.transform.position;
		newPosition.y = -3f;
		scoreBoard.gameObject.transform.position = newPosition;

		GoTweenChain chain = new GoTweenChain(new GoTweenCollectionConfig());

		GoTweenConfig config1 = new GoTweenConfig()
			.setEaseType(GoEaseType.SineIn)
				.position( new Vector3( initialPosition.x,initialPosition.y,initialPosition.z))
				;

		/*
		Vector3 retryInitialPosition = retryBtn.gameObject.transform.position;
		Vector3 retryNewPosition = retryBtn.gameObject.transform.position;
		retryNewPosition.x = -50f;
		retryBtn.gameObject.transform.position= retryNewPosition;

		Vector3 leaderboardInitialPosition = leaderboardBtn.gameObject.transform.position;
		Vector3 leaderboardBtnNewPosition = leaderboardBtn.gameObject.transform.position;
		leaderboardBtnNewPosition.x = 5f;
		leaderboardBtn.gameObject.transform.position= leaderboardBtnNewPosition;



		GoTweenConfig config2 = new GoTweenConfig()
			.setEaseType(GoEaseType.SineIn)
				.position( new Vector3( retryInitialPosition.x,retryInitialPosition.y,retryInitialPosition.z))
				;

		GoTweenConfig config3 = new GoTweenConfig()
				.setEaseType(GoEaseType.SineIn)
				.position( new Vector3( leaderboardInitialPosition.x,leaderboardInitialPosition.y,leaderboardInitialPosition.z))
				;



		/*_tween = Go.to( this.transform, 0.65f, newg GoTweenConfig()
		               .setEaseType(GoEaseType.SineIn)
		               .position( new Vector3( initialPosition.x,initialPosition.y,initialPosition.z))
		               );*/


		GoTween tween = new GoTween( scoreBoard.transform, 0.60f, config1 );
		chain.append( tween );

		/*
		GoTween tween2 = new GoTween( retryBtn.transform, 0.70f, config2 );
		chain.append( tween2 );
		GoTween tween3 = new GoTween( leaderboardBtn.transform, 0.70f, config3 );
		chain.append( tween3 );
		*/



		_tween = chain;
		_tween.timeScale = _tweenTimeScale;
		_tween.play();
	}	
}

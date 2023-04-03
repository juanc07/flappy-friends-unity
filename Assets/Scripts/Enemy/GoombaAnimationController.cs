using UnityEngine;
using System.Collections;

public class GoombaAnimationController : CharacterAnimationController {

	//ESPECIAL ANIMATION
	private const string ANIMATION_SQUEEZED = "squeezed";

	public override void PlayHit(){
		base.PlayHit();
		if(!modelAnimation.IsPlaying(ANIMATION_SQUEEZED)){
			modelAnimation.Play(ANIMATION_SQUEEZED);
		}
	}
	
	public override void PlayDeath(){
		base.PlayDeath();
		if(!modelAnimation.IsPlaying(Animations.death.ToString())){
			modelAnimation.Play(Animations.death.ToString());
		}
	}
	
	public override void PlayIdle(){
		base.PlayIdle();
		modelAnimation.Play(Animations.idle.ToString());
	}
	
	public override void PlayWalk(){
		base.PlayWalk();
		modelAnimation.Play(Animations.walk.ToString());
	}
	
	public override void PlayRun(){
		base.PlayRun();
		if(modelAnimation.GetClip(Animations.run.ToString()) != null){
			modelAnimation.Play(Animations.run.ToString());
		}
	}
	
	public override void PlayJump(){
		base.PlayJump();
		if(modelAnimation.GetClip(Animations.jump.ToString()) != null){
			modelAnimation.Play(Animations.jump.ToString());
		}
	}
	
	public override void PlayFalling(){
		base.PlayFalling();
		modelAnimation.Play(Animations.falling.ToString());
	}
	
	public override void PlayFalling2(){
		base.PlayFalling2();
		if(!modelAnimation.IsPlaying(Animations.jump.ToString())){
			if(modelAnimation.GetClip(Animations.falling2.ToString()) != null){
				modelAnimation.Play(Animations.falling2.ToString());
			}
		}
	}
}

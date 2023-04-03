using UnityEngine;
using System.Collections;

public class HitColliderController : MonoBehaviour {

	private GameDataManager gameDataManager;
	private SoundManager soundManager;
	private ParticleManager particleManager;
	//private ParticleManager particleManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		particleManager = ParticleManager.GetInstance();
		soundManager = SoundManager.GetInstance();
		//particleManager = ParticleManager.GetInstance();
	}
	
	private void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Coin"){
			soundManager.PlaySfx3(SFX.coin3);
			gameDataManager.player.Coin++;
			//gameDataManager.UpdateScore(5);
			Destroy(col.gameObject);
		}else if(col.gameObject.tag == "Well"){
			if(!gameDataManager.player.IsDead){
				particleManager.CreateParticle(ParticleEffect.Hit1,this.gameObject.transform.position,new Vector3(2f,2f,2f));
				soundManager.PlaySfx(SFX.hit3);
				gameDataManager.SetPlayerHP(0);
			}
		}
	}
	
	private void OnTriggerExit(Collider col){
		if(col.gameObject.tag == "Scorer" && !gameDataManager.player.IsDead){
			if(!col.gameObject.GetComponent<ScoreTagger>().isTag){
				particleManager.CreateParticle(ParticleEffect.Hit1,this.gameObject.transform.position,new Vector3(2f,2f,2f));
				col.gameObject.GetComponent<ScoreTagger>().isTag=true;
				soundManager.PlaySfx(SFX.coinEffectAmplify,0.75f);
				gameDataManager.UpdateScore(1);
			}
		}
	}
}

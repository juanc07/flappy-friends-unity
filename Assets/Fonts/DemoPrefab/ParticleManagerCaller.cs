using UnityEngine;
using System.Collections;

public class ParticleManagerCaller : MonoBehaviour {

	private ParticleManager particleManager;

	// Use this for initialization
	void Start () {
		particleManager = ParticleManager.GetInstance();
		InvokeRepeating("ShowParticle",0.3f,0.3f);
	}
	
	private void ShowParticle(){
		particleManager.CreateParticle(ParticleEffect.Hit1,new Vector3(0,0,0),new Vector3(1f,1f,1f));
	}
}

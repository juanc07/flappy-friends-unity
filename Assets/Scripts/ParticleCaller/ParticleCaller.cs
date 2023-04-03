using UnityEngine;
using System.Collections;

public class ParticleCaller : MonoBehaviour {

	private ParticleManager particleManager;

	// Use this for initialization
	void Start () {
		particleManager = ParticleManager.GetInstance();
		particleManager.CreateParticle(ParticleEffect.Hit1,new Vector3(0,0,0),new Vector3(0,0,0),true);
		particleManager.CreateParticle(ParticleEffect.Hit1,new Vector3(0,0,0),new Vector3(0,0,0),true);
		particleManager.CreateParticle(ParticleEffect.Hit1,new Vector3(0,0,0),new Vector3(0,0,0),true);
		
		particleManager.CreateParticle(ParticleEffect.Hit2,new Vector3(0,0,0),new Vector3(0,0,0),true);
		particleManager.CreateParticle(ParticleEffect.Hit2,new Vector3(0,0,0),new Vector3(0,0,0),true);
		particleManager.CreateParticle(ParticleEffect.Hit2,new Vector3(0,0,0),new Vector3(0,0,0),true);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleConfigHolder : ScriptableObject{
	[System.Serializable]
	public class ParticleDictionary:InspectorDictionary<ParticleEffect,GameObject>{}
	public ParticleDictionary particles = new ParticleDictionary();

	public GameObject GetParticle(ParticleEffect effect){
		return particles.Get(effect);
	}
}

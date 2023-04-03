using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {
	
	private static ParticleManager instance;
	private static GameObject container;
	private ParticleConfigHolder particleConfig;
	
	private List<ParticleObject> particlePool = new List<ParticleObject>();
	
	
	public static ParticleManager GetInstance(){
		if(instance == null){
			container = new GameObject();
			container.name = "ParticleManager";
			instance = container.AddComponent(typeof(ParticleManager)) as ParticleManager;
			DontDestroyOnLoad(instance.gameObject);
		}
		return instance;
	}
	
	private void Awake(){
		particleConfig = (ParticleConfigHolder)Resources.Load("Config/ParticleConfig");
		//Debug.Log(" check particleConfig: " + particleConfig);
	}
	
	// Use this for initialization
	void Start (){
		//Debug.Log("particle pre warm!");
	}
	
	private void Update(){
		CheckForNotPlayingParticle();
	}
	
	public void CreateParticle( ParticleEffect particleEffect, Vector3 position, Vector3 scale,bool preParticle = false){
		ParticleObject particleObject =  SearchParicleById(particleEffect);
		if(particleObject==null){
			particleObject = new ParticleObject();
			particleObject.key = particleEffect;
			if(particleConfig.particles.Get(particleEffect)!=null){
				particleObject.particle= Instantiate( particleConfig.particles.Get(particleEffect),position,Quaternion.Euler(0,0,0)) as GameObject;
				particleObject.particle.gameObject.transform.parent = this.gameObject.transform;
				particleObject.particle.gameObject.transform.localScale = scale;
				particlePool.Add(particleObject);
				particleObject.particle.gameObject.GetComponent<ParticleSystem>().Play();
				if(preParticle){
					particleObject.particle.SetActive(false);
				}
				//Debug.Log("create new particle");
			}else{
				Debug.Log("create particle failed: can't find " + particleEffect.ToString() + " particle effect");
			}
		}else{
			particleObject.particle.gameObject.transform.position = position;
			particleObject.particle.gameObject.GetComponent<ParticleSystem>().Play();
			if(preParticle){
				particleObject.particle.SetActive(false);
			}
			//Debug.Log("resuse particle");
		}
	}
	
	public void ClearParticlePool(){
		particlePool.Clear();
	}
	
	private ParticleObject SearchParicleById(ParticleEffect particleEffect){
		int count  = particlePool.Count;
		ParticleObject particleObject = null;
		
		for(int index=0;index<count;index++){
			if(particlePool[index]!=null && particlePool[index].particle != null){
				if(particlePool[index].key == particleEffect){
					if(!particlePool[index].particle.GetComponent<ParticleSystem>().isPlaying){
						particlePool[index].particle.gameObject.SetActive(true);
						particleObject = particlePool[index];
						return particleObject;
					}
				}
			}
		}
		
		return particleObject;
	}
	
	private void CheckForNotPlayingParticle(){
		int count  = particlePool.Count;	
		for(int index=0;index<count;index++){
			if(particlePool[index]!=null && particlePool[index].particle != null){
				if(!particlePool[index].particle.GetComponent<ParticleSystem>().isPlaying){
					particlePool[index].particle.gameObject.SetActive(false);
					//Debug.Log("set active false to not playing particles");
				}
			}
		}
	}
	
}

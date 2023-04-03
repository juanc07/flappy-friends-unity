using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour {

	private static SoundManager instance;
	private static GameObject container;

	public Hashtable soundEffects = new Hashtable();
	private string sfxBasePath="SFX/";

	public bool isSFXLoaded =false;
	private Action SFXLoaded;

	private List<AudioData> sfxCollection = new List<AudioData>();
	private List<AudioData> bgmCollection = new List<AudioData>();

	public Hashtable backGroundMusics = new Hashtable();
	//private string bgmBasePath="BGM/";


	public bool isBGMLoaded =false;
	public bool isBGMPlaying = false;
	private Action BGMLoaded;

	public AudioSource sfxAudioSource;
	public AudioSource sfxAudioSource2;
	public AudioSource sfxAudioSource3;
	public AudioSource bgmAudioSource;

	public bool isSfxOn = true;
	public bool isBgmOn =true;
	private float lastBgmVolume =1f;

	public event Action OnSFXLoaded{
		add{SFXLoaded+=value;}
		remove{SFXLoaded-=value;}
	}

	public event Action OnBGMLoaded{
		add{BGMLoaded+=value;}
		remove{BGMLoaded-=value;}
	}

	public static SoundManager GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name ="SoundManager";
			instance = container.AddComponent(typeof(SoundManager)) as SoundManager;
			DontDestroyOnLoad(instance);
		}

		return instance;
	}

	private void OnDestroy(){
		int sfxCount = sfxCollection.Count;
		for(int index=0;index < sfxCount;index++){
			sfxCollection[index] = null;
		}
		sfxCollection.Clear();

		int bgmCount = bgmCollection.Count;
		for(int index=0;index < bgmCount;index++){
			bgmCollection[index] = null;
		}
		bgmCollection.Clear();

		sfxCollection = null;
		bgmCollection = null;

		//Debug.Log("SoundManager pooled Audio Data cleared");
	}


	//load all sfx from resources/SFX
	private void LoadAllSfx(){
		object[] loadedSfx = Resources.LoadAll("SFX");
		foreach( object sfx in loadedSfx ){
			AudioClip clip = (AudioClip)sfx;
			soundEffects[clip.name] = clip;
		}
		if(null!=SFXLoaded){
			isSFXLoaded =true;
			SFXLoaded();
		}
	}

	//load all bgm from resources/BGM
	private void LoadAllBGM(){
		object[] loadedBgm = Resources.LoadAll("BGM");
		foreach( object bgm in loadedBgm ){
			AudioClip clip = (AudioClip)bgm;
			backGroundMusics[clip.name] = clip;
		}
		if(null!=BGMLoaded){
			isBGMLoaded =true;
			BGMLoaded();
		}
	}

	//load single sfx based on sfx name
	private void LoadSfx(string sfxName){
		AudioClip loadedClip =(AudioClip)Resources.Load(sfxBasePath+sfxName);
		AudioClip clip = (AudioClip)loadedClip;
		soundEffects[clip.name] = clip;
	}

	private AudioClip CheckCachedSFX( string sfxName ){
		int count = sfxCollection.Count;
		AudioClip clip = null;
		for(int index=0;index < count;index++){
			if(sfxCollection[index].name.Equals(sfxName,StringComparison.Ordinal)){
				clip = sfxCollection[index].clip;
				break;
			}
		}

		if(clip==null){
			clip = (AudioClip)soundEffects[sfxName];
			AudioData audioData = new AudioData();
			audioData.id = sfxCollection.Count+1;
			audioData.name = sfxName;
			audioData.clip = clip;
			audioData.type = AudioData.AudioDataType.SFX;
			sfxCollection.Add(audioData);
			//Debug.Log("sfx not in cache, cache: " + sfxName + " now!");
		}else{
			//Debug.Log("sfx used cached: " + sfxName);
		}

		return clip;
	}

	public void PlaySfx(SFX sfxName, float volume =1f){
		AudioClip clip = CheckCachedSFX(sfxName.ToString());
		if(clip!=null){
			if(!isSfxOn){
				volume = 0;
			}

			sfxAudioSource.loop =false;
			sfxAudioSource.clip = clip;
			sfxAudioSource.volume = volume;
			sfxAudioSource.Play();
		}else{
			Debug.Log("Sfx not yet loaded!");
		}
	}

	public void PlaySfx2(SFX sfxName, float volume =1f){
		AudioClip clip = CheckCachedSFX(sfxName.ToString());
		if(clip!=null){
			if(!isSfxOn){
				volume = 0;
			}

			sfxAudioSource2.loop =false;
			sfxAudioSource2.clip = clip;
			sfxAudioSource2.volume = volume;
			sfxAudioSource2.Play();
		}else{
			Debug.Log("Sfx2 not yet loaded!");
		}
	}

	public void PlaySfx3(SFX sfxName, float volume =1f){
		AudioClip clip = CheckCachedSFX(sfxName.ToString());
		if(clip!=null){
			if(!isSfxOn){
				volume = 0;
			}

			sfxAudioSource3.loop =false;
			sfxAudioSource3.clip = clip;
			sfxAudioSource3.volume = volume;
			sfxAudioSource3.Play();
		}else{
			Debug.Log("Sfx3 not yet loaded!");
		}
	}

	public void PlayBGM(BGM bgmName, float volume =1f){
		AudioClip clip = CheckCachedBGM(bgmName.ToString());
		if(clip!=null){
			if(!isBgmOn){
				volume = 0;
			}else{
				lastBgmVolume = volume;
			}

			bgmAudioSource.loop =true;
			bgmAudioSource.clip = clip;
			bgmAudioSource.volume = volume;
			bgmAudioSource.Play();
			isBGMPlaying = true;
		}else{
			Debug.Log("bgm not yet loaded!");
		}
	}


	private AudioClip CheckCachedBGM( string bgmName ){
		int count = bgmCollection.Count;
		AudioClip clip = null;
		for(int index=0;index < count;index++){
			if(bgmCollection[index].name.Equals(bgmName,StringComparison.Ordinal)){
				clip = bgmCollection[index].clip;
				break;
			}
		}
		
		if(clip==null){
			clip = (AudioClip)backGroundMusics[bgmName];
			AudioData audioData = new AudioData();
			audioData.id = bgmCollection.Count+1;
			audioData.name = bgmName;
			audioData.clip = clip;
			audioData.type = AudioData.AudioDataType.BGM;
			bgmCollection.Add(audioData);
			//Debug.Log("bgm not in cache, cache: " + bgmName + " now!");
		}else{
			//Debug.Log("bgm used cached: " + bgmName);
		}
		
		return clip;
	}

	private void EnableOrCreateAudioListener(){
		AudioListener[] audioListeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
		foreach(AudioListener audioListener in  audioListeners ){
			audioListener.enabled = false;
		}

		AudioListener ownAudioListener = this.gameObject.GetComponent<AudioListener>();
		if(!ownAudioListener){
			this.gameObject.AddComponent<AudioListener>();
		}else{
			ownAudioListener.enabled = true;
		}
	}

	private AudioSource CreateAudioSource(string audioSourceName){
		AudioSource audioSource;

		GameObject audioSourceHolder = new GameObject();
		audioSourceHolder.name = audioSourceName;
		audioSourceHolder.transform.parent = this.gameObject.transform;
		audioSourceHolder.AddComponent<AudioSource>();
		audioSource = audioSourceHolder.GetComponent<AudioSource>();

		return audioSource;
	}
    
	private void CreateSFXAndBGMHolder(){
		sfxAudioSource = CreateAudioSource("SFX");
		sfxAudioSource2 = CreateAudioSource("SFX2");
		sfxAudioSource3 = CreateAudioSource("SFX3");
		bgmAudioSource = CreateAudioSource("BGM");
	}

	public void SetSFXVolume(float volume = 1f){
		sfxAudioSource.volume = volume;
	}

	public void SetBGMVolume(float volume = 1f){
		bgmAudioSource.volume = volume;
	}

	public void MuteBGM(){
		isBgmOn =false;
		SetBGMVolume(0);
	}

	public void UnMuteBGM(){
		isBgmOn =true;
		SetBGMVolume(lastBgmVolume);
	}

	public void MuteSfx(){
		isSfxOn =false;
		SetSFXVolume(0);
	}

	public void UnMuteSfx(){
		isSfxOn =true;
		SetSFXVolume(1f);
	}

    // Use this for initialization
	void Start () {
		CreateSFXAndBGMHolder();
		EnableOrCreateAudioListener();
		LoadAllSfx();
		LoadAllBGM();
	}

	private void OnLevelWasLoaded(){
		EnableOrCreateAudioListener();
	}
}

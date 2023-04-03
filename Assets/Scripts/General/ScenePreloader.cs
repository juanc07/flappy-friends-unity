using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScenePreloader : MonoBehaviour {
	public enum Scenes{Preloader,Title,Game,Shop}
	public UILabel preloaderLabel;
	public UISprite preloaderBG;
	public float delay = 0.1f;
	public bool hidePreloader =true;

	private List<string> preloaderMessages = new List<string>( new string[]{"Please wait...","Loading..", "Now Loading..."  } );

	public enum PreloaderState{Started,Done}
	public PreloaderState preloaderState=PreloaderState.Started;

	//private string prevSceneName;
	private string currentSceneName{set;get;}
	private string nextSceneName;

	// Use this for initialization
	void Start (){
		DontDestroyOnLoad(this.gameObject);
		//prevSceneName = Application.loadedLevelName;
		currentSceneName = Application.loadedLevelName;
		if(hidePreloader && !currentSceneName.Equals( Scenes.Preloader.ToString(),StringComparison.Ordinal )){
			ShowHideContent(false);
		}else{
			LoadScene(Scenes.Title);
		}
		//Debug.Log("scene preloader started..");
	}

	private void ShowHideContent(bool val){
		preloaderLabel.gameObject.SetActive(val);
		preloaderBG.gameObject.SetActive(val);
	}
	
	// Update is called once per frame
	void Update (){
	}

	private void OnLevelWasLoaded(){
		//prevSceneName = currentSceneName;
		currentSceneName = Application.loadedLevelName;

		if( currentSceneName.Equals( Scenes.Preloader.ToString(),StringComparison.Ordinal )){
			//System.GC.Collect();
			//Debug.Log("manual collect garbage");
			//Invoke("LoadNextScene",delay);
			LoadNextScene();
		}

		if( currentSceneName.Equals( nextSceneName,StringComparison.Ordinal ) ){
			preloaderState = PreloaderState.Done;
			ShowHideContent(false);
			//Debug.Log("loading scene done, currentSceneName: " + currentSceneName);
		}
	}

	private void ShowRandomMessage(){
		int rnd = UnityEngine.Random.Range(0,preloaderMessages.Count);
		preloaderLabel.text = preloaderMessages[rnd];
	}

	public void LoadScene(Scenes scene){
		ShowHideContent(true);
		ShowRandomMessage();
		preloaderState = PreloaderState.Started;
		nextSceneName = scene.ToString();

		if( !currentSceneName.Equals( Scenes.Preloader.ToString(),StringComparison.Ordinal )){
			Application.LoadLevel(Scenes.Preloader.ToString());
		}else{
			Invoke("LoadNextScene",delay);
		}
	}

	private void LoadNextScene(){
		Application.LoadLevel(nextSceneName);
	}
}

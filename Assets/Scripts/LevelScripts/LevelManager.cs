using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public GameObject[] levelChunks;
	public GameObject levelHolder;
	public GameObject hero;
	public int chunkLength;
	public int offsetX;
	private Vector3 heroPosition;
	private List<GameObject> chunks = new List<GameObject>();
	//private int preloadedChunk=0;
	public bool dontLoad =false;


	// Use this for initialization
	void Start () {
		if(dontLoad)return;
		GenerateRandomChunk(0,false,0);
		GenerateRandomChunk(126);
		/*
		foreach( Transform child in this.gameObject.transform ){
			//Debug.Log("getting level child!");
			//preloadedChunk++;
			chunks.Add(child.gameObject);
		}*/
	}

	private void GenerateRandomChunk(int xPosition, bool isRandom =true,int index =0){
		int rnd = Random.Range(0,levelChunks.Length);
		GameObject levelChunk;
		if(isRandom){
			levelChunk = Instantiate(levelChunks[rnd]) as GameObject;
		}else{
			levelChunk = Instantiate(levelChunks[index]) as GameObject;
		}

		levelChunk.gameObject.transform.position = new Vector3(xPosition,0,0);
		levelChunk.transform.parent = levelHolder.transform;
		chunks.Add(levelChunk);
	}
	
	// Update is called once per frame
	void Update (){
		if(dontLoad)return;

		int chunkCount =(int)hero.gameObject.transform.position.x / (chunkLength - offsetX);
		//Debug.Log("check chunkcount " + chunkCount);

		heroPosition = hero.gameObject.transform.position;
		//if(chunkCount > 0 && chunks.Count < chunkCount){
		if(chunkCount > 0 && chunks.Count < (chunkCount + 2)){
			//Debug.Log("check chunkCount " + chunkCount);
			int rnd = Random.Range(0,levelChunks.Length);
			GameObject levelChunk = Instantiate(levelChunks[rnd]) as GameObject;
			//levelChunk.name = "LevelChunk" + (chunks.Count + 1);
			Vector3 levelChunkPosition =  levelChunk.gameObject.transform.position;
			//levelChunkPosition.x = chunkLength * (chunks.Count + 1) + chunkLength;
			levelChunkPosition.x = chunkLength * chunks.Count;
			levelChunk.gameObject.transform.position = levelChunkPosition;
			levelChunk.transform.parent = levelHolder.transform;
			chunks.Add(levelChunk);
			//Debug.Log("create chunk!");

		}
		//CheckFarAwayChunk();
	}

	private void CheckFarAwayChunk(){
		int cnt = chunks.Count;
		for(int index =0;index<cnt;index++){
			if(heroPosition.x > chunks[index].gameObject.transform.position.x + chunkLength){
				chunks[index].gameObject.SetActive(false);
			}/*else if(chunks[index].gameObject.transform.position.x - chunkLength > heroPosition.x ){
				chunks[index].gameObject.SetActive(false);
			}*/else{
				chunks[index].gameObject.SetActive(true);
			}
		}
	}
}

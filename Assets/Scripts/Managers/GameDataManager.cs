using UnityEngine;
using System.Collections;
using System;

public class GameDataManager : MonoBehaviour {

	private static GameDataManager instance;
	private static GameObject container;

	public Player player = new Player();

	private Action GameRestart;
	public event Action OnGameRestart{
		add{ GameRestart+=value;}
		remove{GameRestart-=value;}
	}

	private bool isLevelComplete=false;
	private Action LevelComplete;
	public event Action OnLevelComplete{
		add{LevelComplete+=value;}
		remove{LevelComplete-=value;}
	}

	private bool isLevelStart=false;
	private Action LevelStart;
	public event Action OnLevelStart{
		add{LevelStart+=value;}
		remove{LevelStart-=value;}
	}


	public static GameDataManager GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name ="GameDataManager";
			instance = container.AddComponent(typeof(GameDataManager)) as GameDataManager;			
			DontDestroyOnLoad(instance);
		}
		return instance;
	}

	// Use this for initialization
	void Start () {
		AddListener();
		SetScore(0);
		SetLevel(1);
		//Debug.Log("start data manager!");
	}

	private void AddListener(){
		player.OnPlayerRevive+=PlayerRevive;
	}

	private void RemoveListener(){
		player.OnPlayerRevive-=PlayerRevive;
	}

	private void OnDestroy(){
		RemoveListener();
	}

	private void PlayerRevive(){
		if(null!= GameRestart){
			GameRestart();
		}
	}

	public void UpdateScore(int score){
		player.Score+=score;
	}

	public void SetScore(int score){
		player.Score=score;
	}

	public void SetHp(int hp){
		player.HP=hp;
	}

	public void SetCoin(int coin){
		player.Coin=coin;
	}

	public void UpdateLevel(){
		player.Level++;
	}

	public void SetLevel(int level){
		player.Level=level;
	}

	public int GetLevel(){
		return player.Level;
	}

	public bool IsLevelComplete{
		set{ isLevelComplete=value;
			if(null!=LevelComplete){
				LevelComplete();
			}
		}
		get{return isLevelComplete;}
	}

	public bool IsLevelStart{
		set{ isLevelStart=value;
			if(null!=LevelStart){
				LevelStart();
				//Debug.Log("dispatch level start event");
			}
		}		
		get{return isLevelStart;}
	}

	public void ResetLevel(){
		isLevelStart =false;
		isLevelComplete = false;
	}

	public void SetPlayerHP( int hp){
		player.HP = hp;
	}

	public void UpdatePlayerHP( int hp){
		player.HP += hp;
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}

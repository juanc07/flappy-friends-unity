using UnityEngine;
using System.Collections;
using System;
using System.Text;


public class GameTimer : MonoBehaviour {

	public int min;
	public int sec;
	private float totalSeconds;
	public string timerDisplay;

	private GameDataManager gameDataManager;
	//private bool isLevelComplete =false;

	private bool isTimeOut=false;
	private Action TimeOut;
	public event Action OnTimeOut{
		add{TimeOut+=value;}
		remove{TimeOut-=value;}
	}

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		totalSeconds = ConvertMinToSec(min) + sec;
		Invoke("LevelStart",0.1f);

		//Debug.Log("time started!");
	}

	private void LevelStart(){
		if(!gameDataManager.IsLevelStart){
			gameDataManager.IsLevelStart = true;
		}
	}
	
	// Update is called once per frame
	void Update (){
		if(gameDataManager!=null){
			if(gameDataManager.IsLevelComplete){
				return;
			}
		}

		if(totalSeconds>0){
			totalSeconds-=Time.deltaTime;
			timerDisplay = ConvertSecToMin(totalSeconds);
			//Debug.Log( "time ==>" + timerDisplay);
		}else{
			if(!isTimeOut){
				isTimeOut =true;
				if(null != TimeOut){
					TimeOut();
				}
			}
		}
	}

	private int ConvertMinToSec(int minute){
		return minute * 60;
	}

	private string ConvertSecToMin(float second){
		float remainder = (second % 60);
		float total = second / 60;
		return "Time " +((int)total).ToString("00") + ":" + ((int)remainder).ToString("00");
	}
}

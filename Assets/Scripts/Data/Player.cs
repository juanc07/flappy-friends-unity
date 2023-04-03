using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player{
	private List<int> boughtItems= new List<int>();
	private int selectedItem=0;

	private int coin;
	private Action CoinUpdate;
	public event Action OnCoinUpdate{
		add{CoinUpdate+=value;}
		remove{CoinUpdate-=value;}
	}

	private int totalCoin;
	private Action TotalCoinUpdate;
	public event Action OnTotalCoinUpdate{
		add{TotalCoinUpdate+=value;}
		remove{TotalCoinUpdate-=value;}
	}

	private int life;
	private Action LifeUpdate;
	public event Action OnLifeUpdate{
		add{ LifeUpdate+=value;}
		remove{LifeUpdate-=value;}
	}

	private int hp;
	private Action HpUpdate;
	public event Action OnHpUpdate{
		add{ HpUpdate+=value;}
		remove{HpUpdate-=value;}
	}

	private bool isDead;
	private Action PlayerDead;
	public event Action OnPlayerDead{
		add{PlayerDead+=value;}
		remove{PlayerDead-=value;}
	}

	private bool hasSetHighScore;
	private Action SetHighScore;
	public event Action OnSetHighScore{
		add{SetHighScore+=value;}
		remove{SetHighScore-=value;}
	}

	private Action PlayerRevive;
	public event Action OnPlayerRevive{
		add{PlayerRevive+=value;}
		remove{PlayerRevive-=value;}
	}

	private int score;
	private Action ScoreUpdate;
	public event Action OnScoreUpdate{
		add{ScoreUpdate+=value;}
		remove{ScoreUpdate-=value;}
	}

	private int totalScore;
	private Action TotalScoreUpdate;
	public event Action OnTotalScoreUpdate{
		add{TotalScoreUpdate+=value;}
		remove{TotalScoreUpdate-=value;}
	}

	private int hiScore;
	private Action HiScoreUpdate;
	public event Action OnHiScoreUpdate{
		add{HiScoreUpdate+=value;}
		remove{HiScoreUpdate-=value;}
	}

	private int level;
	private Action LevelUpdate;
	public event Action OnLevelUpdate{
		add{LevelUpdate+=value;}
		remove{LevelUpdate-=value;}
	}

	public int Score{
		set{ score = value;
			/*if(score > hiScore){
				hiScore = score;
			}*/
			if(null!=ScoreUpdate ){
				ScoreUpdate();
			}
		}
		get{ return score;}
	}

	public void AddItem(ItemAnimal itemAnimal){
		int len = boughtItems.Count;
		bool found =false;
		for(int index=0;index<len;index++){
			if(boughtItems[index]==itemAnimal.id){
				found =true;
				break;
			}
		}

		if(!found){
			boughtItems.Add(itemAnimal.id);
			selectedItem = itemAnimal.id;
			Debug.Log(" selected item id " + selectedItem);
		}
	}

	public void AddItem(int itemId){
		int len = boughtItems.Count;
		bool found =false;
		for(int index=0;index<len;index++){
			if(boughtItems[index]==itemId){
				found =true;
				break;
			}
		}
		
		if(!found){
			boughtItems.Add(itemId);
			selectedItem = itemId;
			Debug.Log(" selected item id " + selectedItem);
		}
	}

	public bool CheckItemIfBought(ItemAnimal itemAnimal){
		int len = boughtItems.Count;
		bool isBought =false;

		for(int index=0;index<len;index++){
			if(boughtItems[index]==itemAnimal.id){
				isBought =true;
				break;
			}
		}		
		return isBought;
	}

	public List<int> GetBoughtAnimals(){
		return boughtItems;
	}

	public int TotalScore{
		set{ totalScore = value;
			if(totalScore > hiScore){
				hiScore = totalScore;
				hasSetHighScore =true;
			}
			if(null!=TotalScoreUpdate ){
				TotalScoreUpdate();
			}
		}
		get{ return totalScore;}
	}

	public int HiScore{
		set{ hiScore = value;
			if(null!=HiScoreUpdate ){
				HiScoreUpdate();
			}
		}
		get{ return hiScore;}
	}

	public int Level{
		set{ level = value;
			if(null!=LevelUpdate ){
				LevelUpdate();
			}
		}
		get{ return level;}
	}


	public int Coin{
		set{coin =value;
			if(null!= CoinUpdate){
				CoinUpdate();
			}
		}
		get{return coin;}
	}


	public int TotalCoin{
		set{totalCoin =value;
			if(null!= TotalCoinUpdate){
				TotalCoinUpdate();
			}
		}
		get{return totalCoin;}
	}

	public int HP{
		set{hp =value;
			if(null!= HpUpdate){
				HpUpdate();
			}
		}
		get{return hp;}
	}


	public int Life{
		set{
			life =value;
			if(null!= LifeUpdate){
				LifeUpdate();
			}
		}
		get{return life;}
	}


	public bool HasSetHighScore{
		set{
			hasSetHighScore =value;
		}

		get{
			return hasSetHighScore;
		}
	}

	public bool IsDead{
		set{
			isDead =value;
			if(null!=PlayerDead && isDead){
				PlayerDead();
			}else if( null!= PlayerRevive && !isDead){
				hp =3;
				PlayerRevive();
			}
		}

		get{return isDead;}
	}

	public int SelectedItem{
		get{return selectedItem;}
		set{selectedItem=value;}
	}

	private bool isConnectedToGooglePlay;
	private Action PlayerConnectedToGooglePlay;
	public event Action OnPlayerConnectedToGooglePlay{
		add{PlayerConnectedToGooglePlay+=value;}
		remove{PlayerConnectedToGooglePlay-=value;}
	}
}

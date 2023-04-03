using UnityEngine;
using System.Collections;
using System;

public class CheckAnimal : MonoBehaviour {

	public GameObject[] animals;
	private GameDataManager gameDataManager;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		CheckAnimalToShow();
	}
	
	private void CheckAnimalToShow(){
		int count = animals.Length;
		for(int index=0;index<count;index++){
			GameObject animal = (GameObject)animals.GetValue(index);
			if(animal!=null){
				if(index == gameDataManager.player.SelectedItem){
					animal.gameObject.SetActive(true);
				}else{
					animal.gameObject.SetActive(false);
				}
			}
		}
	}

}

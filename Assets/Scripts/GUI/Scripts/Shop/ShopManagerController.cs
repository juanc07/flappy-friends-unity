using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShopManagerController : MonoBehaviour {

	public GameObject[] animals;
	private int currentIndex =0;
	public UILabel priceLabel;
	public UILabel animalNameLabel;
	public UILabel playerGoldLabel;

	public GameObject buyButton;
	public GameObject selectButton;

	private GameDataManager gameDatamanager;
	private List<ItemAnimal> itemAnimals = new List<ItemAnimal>();

	private ItemAnimal currentSelected;

	// Use this for initialization
	void Start (){
		gameDatamanager = GameDataManager.GetInstance();
		/*if(gameDatamanager.player.TotalCoin < 9999){
			gameDatamanager.player.TotalCoin = 9999;
		}*/

		UpdatePlayerGold();
		InitItems();
		InitItemToDisplay();
		ShowHide();


		AddEventListener();
	}

	private void InitItemToDisplay(){
		//Debug.Log("check " + gameDatamanager.player.SelectedItem + " items length " + gameDatamanager.player.GetBoughtAnimals().Count);
		if(gameDatamanager.player.SelectedItem==0)return;
		currentIndex = gameDatamanager.player.SelectedItem;
	}

	private void AddEventListener(){
		gameDatamanager.player.OnTotalCoinUpdate+=OnTotalCoinUpdate;
	}

	private void OnDestroy(){
		gameDatamanager.player.OnTotalCoinUpdate-=OnTotalCoinUpdate;
	}

	private void OnTotalCoinUpdate(){
		UpdatePlayerGold();
	}

	private void UpdatePlayerGold(){
		playerGoldLabel.text = gameDatamanager.player.TotalCoin.ToString("00000");
	}

	private void InitItems(){
		int count = animals.Length;
		for(int index=0;index<count;index++){
			ItemAnimal itemAnimal = new ItemAnimal();
			itemAnimal.id = index;
			itemAnimal.animal = (GameObject)animals.GetValue(index);
			itemAnimal.animalName = itemAnimal.animal.name;
			itemAnimal.price = 500 * index;

			if(itemAnimal.price <= 0){
				itemAnimal.isBought = true;
			}else{
				itemAnimal.isBought = gameDatamanager.player.CheckItemIfBought(itemAnimal);
			}
			itemAnimals.Add(itemAnimal);
		}
	}

	private ItemAnimal GetItemById(int idToSearch){
		int cnt = itemAnimals.Count;
		ItemAnimal found = null;

		for(int index=0;index<cnt;index++){
			if(itemAnimals[index]!=null){
				if(itemAnimals[index].id ==idToSearch ){
					found = itemAnimals[index];
					break;
				}
			}
		}

		return found;
	}

	private GameObject GetItemAnimal(string name){
		int len = animals.Length;
		GameObject animal = null;
		for(int index =0;index<len;index++){
			animal = (GameObject)animals.GetValue(index);
			if(animal.gameObject.name.Equals(name,StringComparison.Ordinal)){
				break;
			}
		}

		return animal;
	}

	private void FadeSelectedButton(){
		UISprite selectedSprite = selectButton.gameObject.transform.GetComponentInChildren<UISprite>();
		if(selectedSprite!=null){
			Color32 color = new Color32(64,64,64,255);
			selectedSprite.color = color;
		}				
	}

	private void LightSelectedButton(){
		UISprite selectedSprite = selectButton.gameObject.transform.GetComponentInChildren<UISprite>();
		if(selectedSprite!=null){
			Color32 color = new Color32(255,255,255,255);
			selectedSprite.color = color;
		}
	}

	public void Next(){
		if(currentIndex<animals.Length-1){
			currentIndex++;
			ShowHide();
		}
	}

	public void Prev(){
		if(currentIndex>0){
			currentIndex--;
			ShowHide();
		}
	}

	public void BuyItem(){
		if(gameDatamanager.player.TotalCoin >= currentSelected.price ){
			gameDatamanager.player.TotalCoin-= currentSelected.price;
			gameDatamanager.player.AddItem(currentSelected);
			currentSelected.isBought =true;
			ShowHide();
			Debug.Log(" items length " + gameDatamanager.player.GetBoughtAnimals().Count);
		}
	}

	public void SelectItem(){
		gameDatamanager.player.SelectedItem = currentSelected.id;
		if(currentSelected.id == gameDatamanager.player.SelectedItem){
			LightSelectedButton();
		}else{
			FadeSelectedButton();
		}
	}

	private void ShowHide(){
		int len = itemAnimals.Count;
		for(int index =0;index<len;index++){
			GameObject animal;
			if(index == currentIndex){
				currentSelected = itemAnimals[index];
				animal = itemAnimals[index].animal;
				animal.gameObject.SetActive(true);
				animalNameLabel.text = itemAnimals[index].animalName;

				if(itemAnimals[index].isBought){
					priceLabel.text = "own";
					selectButton.SetActive(true);
					buyButton.SetActive(false);
				}else{
					priceLabel.text = itemAnimals[index].price.ToString("0000");
					selectButton.SetActive(false);
					buyButton.SetActive(true);
				}

				if(index == gameDatamanager.player.SelectedItem){
					LightSelectedButton();
				}else{
					FadeSelectedButton();
				}

				//Debug.Log( "show check animal " + animal.gameObject.name );
			}else{
				animal = itemAnimals[index].animal;
				animal.gameObject.SetActive(false);
				//Debug.Log( "hide check animal " + animal.gameObject.name );
			}
		}
	}
}

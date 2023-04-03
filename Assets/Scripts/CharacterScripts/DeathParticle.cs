using UnityEngine;
using System.Collections;

public class DeathParticle : MonoBehaviour {

	public GameObject character;
	private HeroController heroController;
	public float delay = 1f;
	public float duration = 1f;
	private bool isActivated =false;
	private GameDataManager gameDataManager;
	private Vector3 originalPosition;
	private Vector3 safePosition;

	// Use this for initialization
	void Start () {
		gameDataManager = GameDataManager.GetInstance();
		heroController = character.GetComponent<HeroController>();
		AddEventListener();

		originalPosition = this.gameObject.transform.position;
		safePosition = originalPosition;
		safePosition.y -= 100f;
	}

	private void AddEventListener(){
		gameDataManager.OnGameRestart+= OnGameRestart;
	}

	private void RemoveEventListener(){
		if(gameDataManager!=null){
			gameDataManager.OnGameRestart-= OnGameRestart;
		}
	}

	private void OnDestroy(){
		RemoveEventListener();
	}

	private void OnGameRestart(){
		isActivated = false;
		MoveToOriginalPosition();
		Deactivate(true);
	}
	
	// Update is called once per frame
	void Update () {
		if(heroController.isDead && !isActivated){
			isActivated = true;
			Invoke("ShowDeathParticle",delay);
		}
	}

	private void Deactivate(bool val){
		this.gameObject.SetActive(val);
	}

	private void ShowDeathParticle(){
		//Debug.Log("play death particle");
		//instatiate death particle here
		Deactivate(false);
		Invoke("RemoveCharacter",duration);
	}

	private void RemoveCharacter(){
		//Debug.Log("RemoveCharacter!");
		//Destroy(character);
		MoveToSafePosition();
	}

	private void MoveToSafePosition(){
		this.gameObject.transform.position = safePosition;
	}

	private void MoveToOriginalPosition(){
		this.gameObject.transform.position = originalPosition;
	}
}

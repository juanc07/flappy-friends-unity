using UnityEngine;
using System.Collections;

public abstract class EventListener:MonoBehaviour{

	public virtual void Start(){
		AddEventListener();
	}

	public virtual void OnDestroy(){
		RemoveEventListener();
	}

	public virtual void  AddEventListener(){

	}

	public virtual void RemoveEventListener(){
		
	}
}

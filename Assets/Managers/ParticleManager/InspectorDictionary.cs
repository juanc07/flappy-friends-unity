using System.Collections.Generic;
[System.Serializable]
public class InspectorDictionary<K,V>{
	public List<K> keys = new List<K>();
	public List<V> values = new List<V>();


	public void Set( K key, V value ){
		int index = keys.IndexOf(key);

		if(index>=0){
			values[index] = value;
		}else{
			keys.Add(key);
			values.Add(value);
		}
	}

	public V Get(K key){
		int index = keys.IndexOf(key);

		if(index>=0){
			return values[index];
		}else{
			return default(V);
		}
	}

	public void Clear(){
		keys.Clear();
		values.Clear();
	}

}

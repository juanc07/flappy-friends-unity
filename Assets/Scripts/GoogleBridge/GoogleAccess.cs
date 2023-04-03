using UnityEngine;
using System.Collections;

public class GoogleAccess : MonoBehaviour {

	private GPSArtOfByte gps;
	public string leaderboardId;

	// Use this for initialization
	void Start () {
		gps = GPSArtOfByte.GetInstance();
		gps.ScoreLeaderboardId = leaderboardId;
		//gps.SignIn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

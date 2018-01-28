using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameManager : NetworkBehaviour {

	int teamOneScore = 0;
	int teamTwoScore = 0;

	public static GameManager Singleton;

	void OnEnable () {
		if (Singleton == null) {
			Singleton = this;
		} else {
			Destroy (this);
		}
	}

	void OnDisable () {
		if (Singleton == this) {
			Singleton = null;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Command]
	public void GiveBall (Player _player) {
		

	}

	public void Score (int _teamId) {
		if (_teamId < 0 || _teamId > 1) {
			return;
		}

		Debug.Log ("Team : " + _teamId);
	}
}

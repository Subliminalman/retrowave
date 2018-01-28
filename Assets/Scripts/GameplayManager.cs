using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameplayManager : NetworkBehaviour {

	const float gameTime = 180f;
	float currentGameTime;
	float waitGameTime = 0f;

	int teamOneScore = 0;
	int teamTwoScore = 0;

	int ballPossessedByPlayer = -1; //-1 is no one otherwise it is playerID

	enum GameState { PreGame, Playing, EndGame };
	GameState currentGameState = GameState.PreGame;

	[SerializeField]
	Ball ballPrefab;


	Ball ball;

	public static GameplayManager Singleton;



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
		CmdResetGame ();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!isServer) {
			return;
		}

		if (currentGameState == GameState.PreGame) {

			waitGameTime -= Time.deltaTime;
			if (waitGameTime <= 0f) {
				currentGameState = GameState.Playing;
			}

		} else if (currentGameState == GameState.Playing) {

			currentGameTime -= Time.fixedDeltaTime;

			if (currentGameTime <= 0f) {
				currentGameState = GameState.EndGame;
			}

		} else if (currentGameState == GameState.EndGame) {
			
			//TODO: Show some type of end game thing
		}
	}

	[Command]
	void CmdResetGame () {	
	
		if (ball == null) {
			ball = Instantiate<Ball> (ballPrefab, Vector3.zero, Quaternion.identity);
			NetworkServer.Spawn (ball.gameObject);
		}
		teamOneScore = 0;
		teamTwoScore = 0;

		ballPossessedByPlayer = -1;

		currentGameState = GameState.PreGame;

		waitGameTime = 3f;
		currentGameTime = gameTime;

		Player[] players = FindObjectsOfType<Player> ();
		for (int i = 0; i < players.Length; i++) {
			players [i].RpcRespawn ();
		}			
	}

	//Give ball only if the ball isn't possessed by another player
	[Command]
	public void CmdGiveBall (int _playerId) {	
		if (ballPossessedByPlayer == -1) {	
		 	ballPossessedByPlayer = _playerId;
			Player[] players = FindObjectsOfType<Player> ();
			for (int i = 0; i < players.Length; i++) {
				if (players [i].playerId == _playerId) {
					players [i].hasBall = true;
				}
			}
		}
	}

	[Command]
	public void CmdDropBall (Vector3 _position) {
		ballPossessedByPlayer = -1;
		ball.DropBall (_position);
	}

	[Command]
	public void CmdScore (int _teamId) {
		if (_teamId < 0 || _teamId > 1) {
			return;
		}

		Debug.Log ("Team : " + _teamId);
	}
}

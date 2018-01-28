using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameplayManager : NetworkBehaviour {

	const float gameTime = 180f;
	float currentGameTime;
	float waitGameTime = 0f;
	float waitEndTime = 5f;

	int teamOneScore = 0;
	int teamTwoScore = 0;

	int ballPossessedByPlayer = -1; //-1 is no one otherwise it is playerID

	enum GameState { PreGame, Playing, Results, EndGame };
	GameState currentGameState = GameState.PreGame;

	[SerializeField]
	Transform ballSpawn;

	[SerializeField]
	Ball ballPrefab;

	[SerializeField]
	public HUD hud;

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

			hud.SetGameInfo (teamOneScore, teamTwoScore, currentGameTime);

			if (currentGameTime <= 0f) {
				currentGameState = GameState.Results;
			}

		} else if (currentGameState == GameState.Results) {
			waitEndTime = 5f;

			DetermineWinner ();

			currentGameState = GameState.EndGame;

		} else if (currentGameState == GameState.EndGame) {
			
			//TODO: Show some type of end game thing

			waitEndTime -= Time.deltaTime;
			if (waitEndTime <= 0f) {
				CmdResetGame ();
			}
		}
	}

	[Server]
	void DetermineWinner () {
		if (teamOneScore > teamTwoScore) {
			hud.PinkWins ();
		} else if (teamOneScore < teamTwoScore) {
			hud.CyanWins ();
		} else {
			hud.TieWins ();
		}
	}

	[Command]
	void CmdResetGame () {	
	
		if (ball == null) {
			ball = Instantiate<Ball> (ballPrefab, ballSpawn.position, Quaternion.identity);
			NetworkServer.Spawn (ball.gameObject);
		}

		ResetBall ();

		teamOneScore = 0;
		teamTwoScore = 0;

		ballPossessedByPlayer = -1;

		currentGameState = GameState.PreGame;

		waitGameTime = 3f;
		currentGameTime = gameTime;

		hud.SetGameInfo (teamOneScore, teamTwoScore, currentGameTime);

		hud.ResetMessage ();

		Player[] players = FindObjectsOfType<Player> ();
		for (int i = 0; i < players.Length; i++) {
			players [i].RpcRespawn ();
		}			
	}

	[Server]
	public void ShootBall (Vector3 _position, Vector3 _rotation) {

		if (ball == null) {
			Debug.LogError ("BALL IS NULL");
			ball = Instantiate<Ball> (ballPrefab, Vector3.zero, Quaternion.identity);
			NetworkServer.Spawn (ball.gameObject);
		}
		ball.Shoot (_position, _rotation);
	}

	//Give ball only if the ball isn't possessed by another player
	[Server]
	public void GiveBall (int _playerId) {	
		if (ballPossessedByPlayer == -1) {	
		 	ballPossessedByPlayer = _playerId;
		}
	}

	[Server]
	public void DropBall (Vector3 _position) {
		ballPossessedByPlayer = -1;
		ball.DropBall (_position);
	}

	[Server]
	public void ResetBall () {		
		ball.DropBall (ballSpawn.position);
	}

	[Server]
	public void Score (int _teamId) {
		if (_teamId < 0 || _teamId > 1) {
			return;
		}

		if (_teamId == 0) {
			teamOneScore++;
		} else if (_teamId == 1) {
			teamTwoScore++;
		}

		ball.DisableBall ();

		StartCoroutine (HideAndWait ());


		Debug.Log ("Team : " + _teamId);
	}

	IEnumerator HideAndWait () {
		
		yield return new WaitForSeconds (3f);
		ResetBall ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : TeamSetup {

	public const int maxHealth = 100;

	[SyncVar(hook = "OnChangeHealth")]
	int currentHealth = maxHealth;

	[SyncVar(hook = "OnChangeAmmo")]
	int currentAmmo = 0;

	public int playerId = -1;

	private NetworkStartPosition[] spawnPoints;

	[SerializeField]
	HUD hud;

	[SerializeField] 
	Gun gun;

	[SerializeField]
	Transform shootTransform;

	[SyncVar]
	public bool hasBall = false;
	bool canFire = true;


	protected override void Start () {
		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
			hud = FindObjectOfType<HUD> ();
			//TEMP
			playerId = Random.Range(0, 100000);
		}

		if (NetworkServer.active) {
			RpcRespawn ();
			Spawn (currentTeam);
		}

		base.Start ();
	}

	void Update () {
		if (!isLocalPlayer) {
			return;
		}

		if (!canFire) {
			return;
		}

		if (Input.GetMouseButton(0)) {
			if (hasBall) {
				//TODO: Set animation parameters
				//TODO: If we have ball and die.... drop ball


				Debug.Log ("SHOOT BALL");
				ShootBall();

			} else {				
				gun.CmdFire ();
			}
		}			
	}

	public void TakeDamage (int _amount) {
		if (!isServer) {
			return;
		}

		currentHealth -= _amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
			if (NetworkServer.active) {
				RpcRespawn ();
				Spawn (currentTeam);
			} 
		}
	}
		
	void OnChangeHealth (int _currentHealth) {
		if (isLocalPlayer) {
			hud.SetCurrentHealth (_currentHealth);
			hud.SetMaxAmmo (gun.MaxAmmo);
		}
	}

	void OnChangeAmmo (int _currentAmmo) {
		if (isLocalPlayer) {
			hud.SetCurrentAmmo (_currentAmmo);
			hud.SetMaxHealth (maxHealth);
		}
	}

	void ShootBall () {
		hasBall = false;
		canFire = false;
		GameplayManager.Singleton.CmdShootBall (shootTransform.position + shootTransform.forward * 1.5f, shootTransform.forward);

		StartCoroutine (WaitAndCanFire ());
	}

	IEnumerator WaitAndCanFire () {
		yield return new WaitForSeconds (1f);
		canFire = true;
	}
		
	public void GiveBall () {
		//Put player into has ball state
		GameplayManager.Singleton.CmdGiveBall (playerId);
	}

	[ClientRpc]
	public void RpcDropBall () {
		//call ball to drop at player position
		if (hasBall) {
			GameplayManager.Singleton.CmdDropBall (transform.position);
		}
	}

	[ClientRpc]
	public void RpcRespawn () {
        if (!NetworkServer.active) {
		    Spawn (currentTeam);
        }
	}

	void Spawn (int _teamIndex) {
		gun.Reset ();

		if (isServer) {
			currentAmmo = gun.MaxAmmo;
			currentHealth = maxHealth;		
		}

		if (isLocalPlayer) {
			Vector3 spawnPoint = new Vector3 (0f, 0f, -5f);

			if (spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints [_teamIndex].transform.position;
			}

			transform.position = spawnPoint;
		}
	}		
}

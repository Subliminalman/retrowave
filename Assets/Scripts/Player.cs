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

	public bool hasBall = false;

	protected override void Start () {
		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
			hud = FindObjectOfType<HUD> ();
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

		if (Input.GetMouseButton(0)) {
			gun.CmdFire ();
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

	[ClientRpc]
	public void RpcGiveBall () {
		//Put player into has ball state
		GameplayManager.Singleton.CmdGiveBall(playerId);
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
			Vector3 spawnPoint = Vector3.zero;

			if (spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints [_teamIndex].transform.position;
			}

			transform.position = spawnPoint;
		}
	}		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {


	public bool destroyOnDeath;
	public int currentTeam = 1;
	public const int maxHealth = 100;

	[SyncVar(hook = "OnChangeHealth")]
	int currentHealth = maxHealth;

	[SyncVar(hook = "OnChangeAmmo")]
	int currentAmmo = 0;

	private NetworkStartPosition[] spawnPoints;

	[SerializeField]
	HUD hud;

	[SerializeField] 
	Gun gun;

	void Start () {
		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
			hud = FindObjectOfType<HUD> ();
		}

		RpcRespawn ();
	}

	void Update () {
		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetMouseButton(0)) {
			gun.Fire ();
		}			
	}

	public void TakeDamage (int _amount) {
		if (!isServer) {
			return;
		}

		currentHealth -= _amount;
		if (currentHealth <= 0) {

			if (destroyOnDeath) {
				Destroy (gameObject);
			} else {
				currentHealth = 0;
				RpcRespawn ();
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
	}

	[ClientRpc]
	public void RpcDropBall () {
		//call ball to drop at player position
	}

	[ClientRpc]
	void RpcRespawn () {
		RpcSpawn (currentTeam);
	}

	[ClientRpc]
	void RpcSpawn (int _teamIndex) {
		gun.Reset ();
		currentAmmo = gun.MaxAmmo;
		currentHealth = maxHealth;

		if (isLocalPlayer) {
			Vector3 spawnPoint = Vector3.zero;

			if (spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints [_teamIndex].transform.position;
			}

			transform.position = spawnPoint;

			currentHealth = maxHealth;
		}
	}		
}
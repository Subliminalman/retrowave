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

    [SyncVar]
	bool canFire = true;


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

		if (!canFire) {
			return;
		}

		if (Input.GetMouseButton(0)) {
			if (hasBall) {
				//TODO: Set animation parameters
				//TODO: If we have ball and die.... drop ball
				ShootBall();

			} else {				
				gun.CmdFire ();
			}
		}			

        if (Input.GetMouseButtonDown(1)) {
            if (hasBall) {
                DropBall ();
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
        CmdShootBall ();
        // Do this local so we don't try to shoot next update loop (server will sync the values back to us soon)
		hasBall = false;
		canFire = false;
	}

    [Command] 
    void CmdShootBall () {
        if (!hasBall || !canFire) {
            Debug.LogError ("Client is cheating!");
            return;
        }

        // Synced version, server will set these on all remotes
        hasBall = false;
        canFire = false;

		GameplayManager.Singleton.ShootBall (shootTransform.position + shootTransform.forward * 1.5f, shootTransform.forward);

		StartCoroutine (WaitAndCanFire ());
    }

    [Server]
	IEnumerator WaitAndCanFire () {
		yield return new WaitForSeconds (1f);
		canFire = true;
	}

    [Server]
	public void GiveBall () {
		//Put player into has ball state
        hasBall = true;
		GameplayManager.Singleton.GiveBall (playerId);
	}

    private void DropBall () {
        CmdDropBall ();
        // Do this local so we don't try to shoot next update loop (server will sync the values back to us soon)
        hasBall = false;
    }

	[Command] 
	public void CmdDropBall () {
        if (!hasBall) {
            Debug.LogError ("Client is cheating!");
            return;
        }
		//call ball to drop at player position
        hasBall = false;
		GameplayManager.Singleton.DropBall (shootTransform.position + shootTransform.forward * 1.5f);
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

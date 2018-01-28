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

	[SerializeField]
	Renderer skin;
	[SerializeField]
	GameObject glasses;

    [SyncVar]
	public bool hasBall = false;

    [SyncVar]
	bool canFire = true;

	[SerializeField]
	GameObject gunObj, ballObj;

	protected override void Start () {
		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
			hud = FindObjectOfType<HUD> ();
			//TEMP AND BADDDDDDD!
			playerId = Random.Range(0, 100000);
		}

		Spawn (currentTeam);
		if (NetworkServer.active) {
			RpcRespawn ();
			Spawn (currentTeam);
		}


		base.Start ();
	}

	void Update () {		
		gunObj.SetActive (hasBall == false);
		ballObj.SetActive (hasBall);

		if (!isLocalPlayer) {
			return;
		}			
			
		if (hud == null) {
			hud = FindObjectOfType<HUD> ();
			if (hud != null) {
				hud.SetColor (currentTeam);
				hud.SetCurrentHealth (maxHealth);
				hud.SetMaxAmmo (gun.MaxAmmo);
			}
		} else {
			hud.SetCurrentAmmo (gun.CurrentAmmo);
			hud.SetMaxAmmo (gun.MaxAmmo);
			hud.SetMaxHealth (100);
		}

		if (!canFire) {
			return;
		}

		if (Input.GetMouseButton (0)) {
			if (hasBall) {
				//TODO: Set animation parameters
				//TODO: If we have ball and die.... drop ball

				ShootBall ();

			} else {				
				gun.CmdFire ();
			}
		}			

		if (Input.GetMouseButtonDown (1)) {
			if (hasBall) {
				DropBall ();
			}
		}

		if (transform.position.y < -10f) {
			Kill (true);
		}
	}

	public void TakeDamage (int _amount) {
		if (!isServer) {
			return;
		}

		currentHealth -= _amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
			Kill ();
		}
	}

	void Kill (bool _resetBall = false) {
		if (NetworkServer.active) {
			RpcRespawn ();
			Spawn (currentTeam);

			if (hasBall) {
				if (_resetBall) {
					GameplayManager.Singleton.ResetBall ();
				} else {
					GameplayManager.Singleton.DropBall (transform.position);
				}
			}
		} 
	}

	void OnChangeHealth (int _currentHealth) {
		if (isLocalPlayer) {
			if (hud == null) {
				hud = FindObjectOfType<HUD> ();
			}

			if (hud != null) {
				hud.SetColor (currentTeam);
				hud.SetCurrentHealth (_currentHealth);
				hud.SetMaxAmmo (gun.MaxAmmo);
			} 

		}
	}

	void OnChangeAmmo (int _currentAmmo) {
		if (isLocalPlayer) {
			if (hud != null) {
				hud.SetCurrentAmmo (_currentAmmo);
				hud.SetMaxHealth (maxHealth);
			}
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


		glasses.SetActive (!isLocalPlayer);

		if (hud == null) {
			hud = FindObjectOfType<HUD> ();
		}

		if (hud != null) {
			hud.SetColor (currentTeam);
			hud.SetCurrentHealth (maxHealth);
			hud.SetMaxAmmo (gun.MaxAmmo);
		}

		skin.material.color = _teamIndex == 0 ? Color.magenta : Color.cyan;

		if (isLocalPlayer) {
			Vector3 spawnPoint = new Vector3 (0f, 0f, -5f);
			Vector3 spawnRotation = Vector3.zero;

			if (spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints [_teamIndex].transform.position;
				spawnRotation = spawnPoints [_teamIndex].transform.rotation.eulerAngles;
			}

			transform.position = spawnPoint + Vector3.up * 1.5f;
			transform.rotation = Quaternion.Euler (spawnRotation);
		}
	}		
}

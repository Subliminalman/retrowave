using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUD : NetworkBehaviour {
	[SerializeField]
	Text ammoText;

	[SerializeField]
	Text healthText;

	[SerializeField]
	Text teamOneText, teamTwoText, timeText;

	[SerializeField]
	Image hudImg;

	[SerializeField]
	Image pinkImg, cyanImg,tieImg;

	int currentAmmo = 0;
	int maxAmmo = 0;
	int currentHealth = 0;
	int maxHealth = 0;

	void Awake () {
		ClientScene.RegisterPrefab (gameObject);
	}

	public void SetColor (int _teamIndex) {
		hudImg.color = _teamIndex == 0 ? Color.magenta : Color.cyan;
	}

	public void ResetMessage () {
		RpcResetMessage ();
		LocalResetMessage ();
	}

	void LocalResetMessage () {
		pinkImg.enabled = false;
		cyanImg.enabled = false;
		tieImg.enabled = false;
	}		

	[ClientRpc]
	void RpcResetMessage () {
		if (!NetworkServer.active) {
			LocalResetMessage ();
		}
	}

	public void TieWins () {
		RpcTieWinsMessage ();
		LocalTieWinsMessage ();
	}

	void LocalTieWinsMessage () {
		pinkImg.enabled = true;
	}

	[ClientRpc]
	void RpcTieWinsMessage () {
		if (!NetworkServer.active) {
			LocalPinkWinsMessage ();
		}
	}

	public void PinkWins () {
		RpcPinkWinsMessage ();
		LocalPinkWinsMessage ();
	}

	void LocalPinkWinsMessage () {
		pinkImg.enabled = true;
	}

	[ClientRpc]
	void RpcPinkWinsMessage () {
		if (!NetworkServer.active) {
			LocalPinkWinsMessage ();
		}
	}

	public void CyanWins () {
		RpcPinkWinsMessage ();
		LocalPinkWinsMessage ();
	}

	void LocalCyanWinsMessage () {
		pinkImg.enabled = true;
	}

	[ClientRpc]
	void RpcCyanWinsMessage () {
		if (!NetworkServer.active) {
			LocalCyanWinsMessage ();
		}
	}

	public void SetGameInfo (int _teamOneScore, int _teamTwoScore, float _time) {		
		RpcSetGameInfo (_teamOneScore, _teamTwoScore, _time);
		LocalSetGameInfo (_teamOneScore, _teamTwoScore, _time);
	}

	[ClientRpc]
	void RpcSetGameInfo (int _teamOneScore, int _teamTwoScore, float _time) {		
		if (!NetworkServer.active) {
			LocalSetGameInfo (_teamOneScore, _teamTwoScore, _time);
		}
	}

	void LocalSetGameInfo(int _teamOneScore, int _teamTwoScore, float _time) {
		teamOneText.text = "" + _teamOneScore;
		teamTwoText.text = "" + _teamTwoScore;

		System.TimeSpan t = System.TimeSpan.FromSeconds( _time );

		string formattedTime = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
		timeText.text = "" + formattedTime;//Format this later
	}

	public void SetCurrentAmmo (int _ammo) {
		currentAmmo = _ammo;
	}

	public void SetMaxAmmo (int _maxAmmo) {
		maxAmmo = _maxAmmo;
	}

	public void SetCurrentHealth (int _health) {
		currentHealth = _health;
	}

	public void SetMaxHealth (int _maxHealth) {
		maxHealth = _maxHealth;
	}

	void Update () {
		if (ammoText) {
			ammoText.text = "" + currentAmmo + " / " + maxAmmo;
		}

		if (healthText) {
			healthText.text = "" + currentHealth + " / 100";
		}
	}
}

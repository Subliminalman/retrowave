using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public RectTransform healthBar;

	public const int maxHealth = 100;

	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;

	public void TakeDamage (int _amount) {
		if (!isServer) {
			return;
		}

		currentHealth -= _amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
			RpcRespawn ();
		}
	}
		
	void OnChangeHealth (int currentHealth) {
		healthBar.sizeDelta = new Vector2 ((((float)currentHealth) / maxHealth), healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn () {
		if (isLocalPlayer) {
			transform.position = Vector3.zero;
			currentHealth = maxHealth;
		}
	}
}

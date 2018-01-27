using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	[SerializeField]
	Text ammoText;

	[SerializeField]
	Text healthText;

	int currentAmmo = 0;
	int maxAmmo = 0;
	int currentHealth = 0;
	int maxHealth = 0;


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
			healthText.text = "" + currentHealth + " / " + maxHealth;
		}
	}
}

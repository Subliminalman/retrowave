using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Gun : NetworkBehaviour {

	public enum GunType { Projectile, HitScan };
	public GunType currentGunType = GunType.HitScan;

	[SerializeField]
	Transform fireOrigin;

	[SerializeField]
	Bullet projectile;

	[SerializeField]
	int damage = 1;

	[SerializeField][Range(0f, 5f)]
	float fireRate;

	[SerializeField][Range(0f, 5f)]
	float reloadTime;

	[SerializeField][Range(1f, 100f)]
	int maxAmmo;

	[SerializeField][Range(1f, 200f)]
	float fireDistance;

	[SerializeField][Range(0f, 5f)]
	float fireSpread;

	[SerializeField]
	AudioClip reloadClip;

	[SerializeField]
	AudioClip[] bulletClips;

	bool isReloading = false;
	int currentAmmo;
	float currentFireRateTime;
	string hitLayer = "Hit";
	AudioPoolManager audioPool;

	public int CurrentAmmo {
		get { return currentAmmo; }
	}

	public int MaxAmmo {
		get { return maxAmmo; }
	}		
		
	void Awake () {
		Reset ();
	}

	void FixedUpdate () {
		currentFireRateTime -= Time.fixedDeltaTime;
		currentFireRateTime = Mathf.Clamp (currentFireRateTime, 0f, fireRate);
	}

	public void Reset () {
		currentAmmo = maxAmmo;
		currentFireRateTime = fireRate;
	}

	public virtual void Fire () {
		if (isReloading) {
			return;
		}

		if (currentFireRateTime > 0f) {
			return;
		}

		currentFireRateTime = fireRate;

		if (currentGunType == GunType.HitScan) {
			CmdHitScan ();
		} else if (currentGunType == GunType.Projectile) {
			CmdProjectileShoot ();
		}

		--currentAmmo;

		if (currentAmmo <= 0) {
			Reload ();
		}
	}

	protected virtual void Reload () {
		//Play animation here
		FinishedReloading ();	
	}

	protected virtual void FinishedReloading () {
		currentAmmo = maxAmmo;
		currentFireRateTime = 0f;
	}

	[Command]
	protected virtual void CmdHitScan () {
		RaycastHit hit;
		Vector3 spread = (Random.insideUnitCircle * fireSpread);
		if (Physics.Raycast (fireOrigin.position, fireOrigin.forward + spread, out hit, fireDistance, 1 << LayerMask.NameToLayer (hitLayer))) {
			Player p = hit.transform.gameObject.GetComponent<Player> ();
			if (p != null) {
				p.TakeDamage (damage);
			}				
		}			
	}

	[Command]
	protected virtual void CmdProjectileShoot () {
		if (projectile == null) {
			return;
		}

		Bullet b = Instantiate<Bullet> (projectile, fireOrigin.position, Quaternion.Euler (fireOrigin.forward));
		b.Shoot ();
		NetworkServer.Spawn (b.gameObject);
	}

	[ClientRpc]
	void RpcBulletPlaySound(int _clipIndex) {
		if (_clipIndex < 0 || _clipIndex >= bulletClips.Length) {
			return;
		}			

		AudioSource audio = audioPool.GetAudioSource ();

		audio.transform.position = transform.position;

		AudioClip clip = bulletClips [_clipIndex];

		audio.PlayOneShot (clip);
		StartCoroutine(audioPool.WaitAndReturnAudioSource(clip.length, audio));
	}

	[ClientRpc]
	void RpcPlayReloadSound() {
		

		AudioSource audio = audioPool.GetAudioSource ();

		audio.transform.position = transform.position;

		audio.PlayOneShot (reloadClip);
		StartCoroutine(audioPool.WaitAndReturnAudioSource(reloadClip.length, audio));
	}
}

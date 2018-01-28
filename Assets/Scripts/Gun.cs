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
	GameObject hitscanHitPrefab;

	[SerializeField]
	AudioClip reloadClip;

	[SerializeField]
	AudioClip[] bulletClips;

	[SerializeField]
	Animator anim;

	[SerializeField]
	Renderer render;

	bool isReloading = false;
	int currentAmmo;
	float currentFireRateTime;
	public LayerMask hitMask;
	AudioPoolManager audioPool;
    private Player attachedPlayer;

	public int CurrentAmmo {
		get { return currentAmmo; }
	}

	public int MaxAmmo {
		get { return maxAmmo; }
	}		
		
	void Awake () {
		Reset ();
		attachedPlayer = GetComponent<Player> ();
	}

	void FixedUpdate () {
		currentFireRateTime -= Time.fixedDeltaTime;
		currentFireRateTime = Mathf.Clamp (currentFireRateTime, 0f, fireRate);
	}

	public void Reset () {
		currentAmmo = maxAmmo;
		currentFireRateTime = fireRate;

		int teamLayer = 1 << gameObject.layer;
		hitMask &= ~teamLayer;			
	}

	public void ResetFireRate () {
		currentFireRateTime = fireRate;
	}

	[Command]
	public void CmdFire () {
		Debug.Log ("Try Fire");

		if (isReloading) {
			return;
		}
			
		if (currentFireRateTime > 0f) {
			return;
		}

		currentFireRateTime = fireRate;

		if (currentGunType == GunType.HitScan) {
			HitScan ();
		} else if (currentGunType == GunType.Projectile) {
			ProjectileShoot ();
		}

		--currentAmmo;

		if (currentAmmo <= 0) {
			Reload ();
		}
	}
		
	protected virtual void Reload () {
		//Play animation here
		StartCoroutine(WaitAndFinishReloading());
	}

	IEnumerator WaitAndFinishReloading () {
		//Temp
		render.enabled = false;

		yield return new WaitForSeconds (reloadTime);
		FinishedReloading ();
	}

	protected virtual void FinishedReloading () {
		currentAmmo = maxAmmo;
		currentFireRateTime = 0f;
		render.enabled = true;
	}
		
	protected virtual void HitScan () {
		RaycastHit hit;
		Vector3 spread = (Random.insideUnitCircle * fireSpread);
		if (Physics.Raycast (fireOrigin.position, fireOrigin.forward + spread, out hit, fireDistance, hitMask)) {
			Player p = hit.transform.gameObject.GetComponent<Player> ();
			//if (p == null || p.currentTeam == attachedPlayer.currentTeam) {
                // No hit
                //return;
			//}
			Debug.Log ("HIT");
			if (p != null) {
				p.TakeDamage (damage);
			}
			GameObject go = Instantiate<GameObject> (hitscanHitPrefab, hit.point + hit.normal, Quaternion.identity);
			NetworkServer.Spawn (go);
		}			
	}

	protected virtual void ProjectileShoot () {
		if (projectile == null) {
			return;
		}

		Bullet b = Instantiate<Bullet> (projectile, fireOrigin.position, fireOrigin.rotation);
		b.currentTeam = attachedPlayer.currentTeam;
		b.Shoot (fireOrigin.forward);
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

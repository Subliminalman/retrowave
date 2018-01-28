using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : TeamSetup {
    	public int damage = 10;
    	public float velocity = 100.0f;
	Rigidbody rigid;


	public virtual void Shoot (Vector3 _forward) {
		rigid = GetComponent<Rigidbody> ();
		if (rigid == null) {
			rigid = gameObject.AddComponent<Rigidbody> ();
		}			

		rigid.AddForce (_forward * velocity, ForceMode.Impulse);
	}

	void OnCollisionEnter (Collision _col) {

		var hit = _col.gameObject;
		var health = hit.GetComponent<Player>();
		if (health != null && health.currentTeam != currentTeam) {
			health.TakeDamage(damage);
		}
		Destroy (gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	Rigidbody rigid;

	public virtual void Shoot () {
		if (rigid == null) {
			rigid = gameObject.AddComponent<Rigidbody> ();
		}
	}

	void OnCollisionEnter (Collision _col) {

		var hit = _col.gameObject;
		var health = hit.GetComponent<Player>();
		if (health != null) {
			health.TakeDamage(10);
		}
		Destroy (gameObject);
	}
}

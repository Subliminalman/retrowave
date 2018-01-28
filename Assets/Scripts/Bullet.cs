using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage = 10;
    public float velocity = 100.0f;
	Rigidbody rigid;

	public virtual void Shoot () {
        rigid = GetComponent<Rigidbody> ();
		if (rigid == null) {
			rigid = gameObject.AddComponent<Rigidbody> ();
		}
        rigid.AddForce (transform.forward * velocity, ForceMode.Impulse);
	}

	void OnCollisionEnter (Collision _col) {

		var hit = _col.gameObject;
		var health = hit.GetComponent<Player>();
		if (health != null) {
			health.TakeDamage(damage);
		}
		Destroy (gameObject);
	}
}

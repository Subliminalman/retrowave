using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ball : NetworkBehaviour {

	[SerializeField]
	float resetForce;

	Rigidbody rigid;
	Renderer renderer;
	Collider col;

	static Ball Singleton;


	void OnEnable () {
		if (Singleton == null) {
			Singleton = this;
		} else {
			Destroy (gameObject);
		}
	}

	void OnApplicationQuit () {
		if (Singleton == this) {
			Singleton = null;
		}
	}
		
	void DropBall (Vector3 _position) {		
		transform.position = _position;
		rigid.velocity = Vector3.zero;
		gameObject.SetActive (true);
		rigid.AddForce(Vector3.up * resetForce);
	}

	void OnCollisionEnter (Collision _col) {
		if (_col.transform.CompareTag ("Player")) {
			Player p = _col.gameObject.GetComponent<Player> ();

			if (p != null) {
				//Hide ball
				//Give player Ball state
				//Tell game manager which player has ball so it doesn't respawn ball
				p.RpcGiveBall ();
				gameObject.SetActive (false);
			}
		}
	}
		
}

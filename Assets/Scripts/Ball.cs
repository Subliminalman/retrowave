using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Ball : NetworkBehaviour {

	[SerializeField]
	float resetForce;
	[SerializeField]
	float shootForce;

	Rigidbody rigid;
	Renderer renderer;
	Collider col;

	void Awake () {
		rigid = GetComponent<Rigidbody> ();
	}
		
	public void Shoot (Vector3 _position, Vector3 _rotation) {
        RpcShoot (_position, _rotation);
        LocalShoot (_position, _rotation);
    }

    [ClientRpc]
    private void RpcShoot (Vector3 _position, Vector3 _rotation) {
        if (!NetworkServer.active) {
            LocalShoot (_position, _rotation);
        }
    }

    private void LocalShoot (Vector3 _position, Vector3 _rotation) {
		gameObject.SetActive (true);
		transform.position = _position;
		transform.rotation = Quaternion.Euler (_rotation);
		rigid.velocity = Vector3.zero;
		rigid.AddForce (_rotation * shootForce);
	}

	public void DropBall (Vector3 _position) {		
        RpcDropBall (_position);
        LocalDropBall (_position);
    }

    [ClientRpc]
    private void RpcDropBall (Vector3 _position) {
        if (!NetworkServer.active) {
            LocalDropBall (_position);
        }
    }

    private void LocalDropBall (Vector3 _position) {
		transform.position = _position;
		rigid.velocity = Vector3.zero;
		gameObject.SetActive (true);
		rigid.AddForce(Vector3.up * resetForce);
	}

    [ClientRpc]
    private void RpcDisableBall () {
        gameObject.SetActive (false);
    }

	void OnCollisionEnter (Collision _col) {
        if (!NetworkServer.active || !gameObject.activeSelf) {
            // Let the server decided which collision worked (first one sets active false)
            return;
        }

		if (_col.transform.CompareTag ("Player")) {
			Player p = _col.gameObject.GetComponent<Player> ();

			if (p != null) {
				//Hide ball
				//Give player Ball state
				//Tell game manager which player has ball so it doesn't respawn ball
				p.GiveBall ();
                RpcDisableBall ();
				gameObject.SetActive (false);
			}
		}
	}
		
}

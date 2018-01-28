using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Goal : NetworkBehaviour {
	[SerializeField]
	int teamId = 0;
	[SerializeField]
	ParticleSystem particleSys;

	void Awake () {
		particleSys.Stop ();
	}

	public void PlaySparks () {
		RpcPlaySparks ();
		LocalPlaySparks ();
	}

	[ClientRpc]
	void RpcPlaySparks () {
		if (!NetworkServer.active) {
			LocalPlaySparks ();
		}
	}

	void LocalPlaySparks () {
		particleSys.Play ();
	}


	void OnTriggerEnter (Collider _col) {
		Ball b = _col.transform.GetComponent<Ball> ();

		if (b != null) {
			PlaySparks ();
			GameplayManager.Singleton.Score (teamId);		
		}
	}
}

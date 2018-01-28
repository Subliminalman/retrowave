using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnAOE : MonoBehaviour {
    public GameObject AOEPrefab;

	void OnCollisionEnter (Collision c) {
		if (NetworkServer.active) {
            GameObject aoe = Instantiate (AOEPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn (aoe);
        }
	}
}

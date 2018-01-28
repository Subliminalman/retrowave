using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Goal : NetworkBehaviour {
	[SerializeField]
	int teamId = 0;

	void OnTriggerEnter (Collider _col) {
		//Ball b = _col.transform.GetComponent<Ball> ();

		//if (b != null) {
			//GameplayManager.Singleton.Score (teamId);		

			Debug.Log ("GOALLLLLLLLL");
		//}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Goal : NetworkBehaviour {
	[SerializeField]
	int teamId = 0;

	void OnTriggerEnter (Collider _col) {
		Ball b = _col.transform.GetComponent<Ball> ();

		if (b != null) {
			GameManager.Singleton.Score (teamId);		
		}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour{

	public GameObject enemyPrefab;
	public int numberOfEnemies;

	public override void OnStartServer () {
		for (int i = 0; i < numberOfEnemies; i++) {
			Vector3 spawnPosition = new Vector3 (Random.Range (-8f, 8f), 0f, Random.Range (-8f, 8f));
			Quaternion spawnRotation = Quaternion.Euler (0f, Random.Range (0f, 180f), 0f);

			GameObject enm = Instantiate<GameObject> (enemyPrefab, spawnPosition, spawnRotation);
			NetworkServer.Spawn (enm);
		}
	}
}

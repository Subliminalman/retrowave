using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamSetup : NetworkBehaviour {

    [SyncVar]
    public int currentTeam;

	protected virtual void Start () {
        // Current team set by the network (or lobby)
        switch (currentTeam) {
            case 0:
                Debug.Log ("Setting layer to Team1");
                gameObject.layer = LayerMask.NameToLayer ("Team1");
                break;
            case 1:
                Debug.Log ("Setting layer to Team2");
                gameObject.layer = LayerMask.NameToLayer ("Team2");
                break;
            default:
                Debug.Log ("Setting layer to default");
                gameObject.layer = LayerMask.NameToLayer ("default");
                break;
        }
	}
	
}

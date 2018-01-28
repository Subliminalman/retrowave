using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : NetworkLobbyManager {

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer)
	{
        // gamePlayer.GetComponent<...>().name = lobbyPlayer.GetComponent<LobbyPlayer> ().nameSelection;
        Debug.Log ("Setting gamePlayer to team " + lobbyPlayer.GetComponent<LobbyPlayer> ().teamSelection);
        gamePlayer.GetComponent<Player>().currentTeam = lobbyPlayer.GetComponent<LobbyPlayer> ().teamSelection;
        return true;
    }

}

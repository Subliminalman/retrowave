using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using TMPro;

public class LobbyPlayer : NetworkLobbyPlayer {
    [SyncVar(hook="OnNameChanged")]
    public string nameSelection = "";

    [SyncVar(hook="OnSelectedTeam")]
    public int teamSelection = 0;

    private NetworkSetup setup;
    private Toggle readyToggle;
    private TMP_InputField playerName;
    private TMP_Dropdown teamSelect;

    private void Awake () {
        setup = (NetworkSetup) FindObjectOfType (typeof(NetworkSetup));
    }

    public override void OnClientEnterLobby () {
        readyToggle = setup.readyToggles[slot];
        readyToggle.gameObject.SetActive (true);
        readyToggle.interactable = false;
        readyToggle.isOn = false;

        playerName = setup.playerNames[slot];
        playerName.gameObject.SetActive (true);
        playerName.interactable = false;
        playerName.text = nameSelection; // syncvar hooks don't run for the first set

        teamSelect = setup.teamSelection[slot];
        teamSelect.gameObject.SetActive (true);
        teamSelect.interactable = false;
        teamSelect.value = teamSelection; // syncvar hooks don't run for the first set
    }


    public override void OnStartLocalPlayer () {
        readyToggle.interactable = true;
        readyToggle.onValueChanged.AddListener (ReadyCallback);

        playerName.interactable = true;
        playerName.onEndEdit.AddListener (NameCallback);

        switch (slot) {
            case 0:
                NameCallback ("Player One");
                break;
            case 1:
                NameCallback ("Player Two");
                break;
            case 2:
                NameCallback ("Player Three");
                break;
            case 3:
                NameCallback ("Player Four");
                break;
            case 4:
                NameCallback ("Player Five");
                break;
            case 5:
                NameCallback ("Player Six");
                break;
            case 6:
                NameCallback ("Player Seven");
                break;
            case 7:
                NameCallback ("Player Eight");
                break;
        }

        teamSelect.interactable = true;
        teamSelect.onValueChanged.AddListener (TeamCallback);
    }

    public override void OnClientExitLobby () {
        if (readyToggle != null) {
            if (isLocalPlayer) {
                readyToggle.onValueChanged.RemoveListener (ReadyCallback);
                teamSelect.onValueChanged.RemoveListener (TeamCallback);
            }
            readyToggle.gameObject.SetActive (false);
            teamSelect.gameObject.SetActive (false);
        }
    }

    // From network (like hook)
    public override void OnClientReady (bool ready) {
        readyToggle.isOn = ready;
    }

    // UI
    private void ReadyCallback (bool ready) {
        if (ready) {
            SendReadyToBeginMessage ();
        } else {
            SendNotReadyToBeginMessage ();
        }
    }

    // Hook
    private void OnNameChanged (string name) {
        playerName.text = name;
    }

    // UI
    private void NameCallback (string name) {
        nameSelection = name;
        if (!NetworkServer.active) {
            CmdSetName (name);
        }
    }

    [Command]
    private void CmdSetName (string name) {
        nameSelection = name;
    }

    // Hook
    private void OnSelectedTeam (int team) { 
        teamSelect.value = team;
    }

    // UI
    private void TeamCallback (int team) {
        teamSelection = team;
        if (!NetworkServer.active) {
            CmdSetTeam (team);
        }
    }

    [Command]
    private void CmdSetTeam (int team) {
        teamSelection = team;
    }
}

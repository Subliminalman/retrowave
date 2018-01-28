using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class NetworkSetup : MonoBehaviour {

    private string ip = "127.0.0.1";
    public string IP { set { ip = value; } }

    public short port = 9919;

    private NetworkClient client = null;

    public GameObject clientPanel;
    public GameObject lobbyPanel;

    public Toggle[] readyToggles;
    public TMP_InputField[] playerNames;
    public TMP_Dropdown[] teamSelection;

    public TextMeshProUGUI ipInput;

    private void Awake () {
        Application.runInBackground = true;
        Random.InitState (Mathf.FloorToInt (Time.realtimeSinceStartup * 100000));

        for (int i = 0; i < readyToggles.Length; i ++) {
            readyToggles[i].gameObject.SetActive (false);
        }

        for (int i = 0; i < teamSelection.Length; i ++) {
            teamSelection[i].gameObject.SetActive (false);
        }
    }

    private void Start () {
        clientPanel.SetActive (true);
        lobbyPanel.SetActive (false);
    }

    public void StartHost () {
		if (client == null) {
			NetworkLobbyManager nm = (NetworkLobbyManager)NetworkManager.singleton;
			if (nm == null) {
				Debug.LogError ("No NetworkManager loaded");
				return;
			}

			nm.showLobbyGUI = false;

			if (NetworkServer.active) {
				// Restart when reloading the scene
				nm.StopServer ();
			}

			nm.networkPort = port;
			client = nm.StartHost ();
		}
		else
		{
			Debug.LogError ("Client already exists, but hosting");
		}

        clientPanel.SetActive (false);
        lobbyPanel.SetActive (true);
    }

    public void StartClient () {
        if (client == null) {
            NetworkManager nm = NetworkManager.singleton;
            nm.networkAddress = ip;
            nm.networkPort = port;
            client = nm.StartClient ();
        } else {
            Debug.LogError ("Second client start");
        }

        clientPanel.SetActive (false);
        lobbyPanel.SetActive (true);
    }
}

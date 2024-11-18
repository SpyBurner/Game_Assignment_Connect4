using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
public class LoginManager : PhotonSingleton<LoginManager>
{
    public Text usernameText;
    private GameObject photonStatus;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.OfflineMode = false;
        if (usernameText == null)
        {
            throw new System.Exception("Username text is not set");
        }
        photonStatus = GameObject.Find("PhotonStatus");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonStatus != null)
        {
            photonStatus.GetComponent<Text>().text = PhotonNetwork.NetworkClientState.ToString();
        }

        if (usernameText != null)
        {
            usernameText.text = PhotonNetwork.NickName;
        }
    }

    public void Login()
    {
        string username = usernameText.text;

        if (string.IsNullOrEmpty(username))
        {
            Debug.Log("Username is empty");
            return;
        }

        PhotonNetwork.NickName = username;

        PhotonNetwork.ConnectUsingSettings();
    }


    public void Logout()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected from server: " + cause.ToString());
    }
}

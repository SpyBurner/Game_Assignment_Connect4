using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviourPunCallbacks
{
    public GameObject boardPrefab;
    public GameObject playerPrefab;
    public GameObject turnManagerPrefab;

    private void Awake()
    {
        //DEBUG
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom(null);
        }
        //
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(boardPrefab.name, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(turnManagerPrefab.name, Vector3.zero, Quaternion.identity);
        }
        Debug.LogError("Spawning player");
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);

        int num = PhotonNetwork.LocalPlayer.ActorNumber;
        if (num == 1)
        {
            newPlayer.GetComponent<PlayerCore>().color = Color.red;
        }
        else
        {
            newPlayer.GetComponent<PlayerCore>().color = Color.blue;
        }

        newPlayer.GetComponent<PhotonView>().RPC("SetTurnID", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    // Start is called before the first frame update
    void Start()
    {
    }
}

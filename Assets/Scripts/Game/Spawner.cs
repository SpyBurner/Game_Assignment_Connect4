using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviourPunCallbacks
{
    public GameObject boardPrefab;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
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
        }       

        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
    }
}

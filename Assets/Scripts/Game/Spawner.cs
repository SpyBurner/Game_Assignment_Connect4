using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviourPunCallbacks
{
    public GameObject boardPrefab;
    public GameObject playerPrefab;
    public GameObject turnManagerPrefab;

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
            PhotonNetwork.Instantiate(turnManagerPrefab.name, Vector3.zero, Quaternion.identity);
        }
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
        {
            GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            newPlayer.GetComponent<PlayerCore>().color = new Color(Random.value, Random.value, Random.value);

            newPlayer.GetComponent<PhotonView>().RPC("SetTurnID", RpcTarget.AllBuffered, PhotonNetwork.PlayerList.Length - 1);

            Vector3 newPos = transform.position + newPlayer.GetComponent<SpriteRenderer>().bounds.size.x * Vector3.right * (PhotonNetwork.PlayerList.Length - 1);
            newPlayer.GetComponent<PhotonView>().RPC("SetPosition", RpcTarget.AllBuffered, newPos.x, newPos.y);
        }
    }
}

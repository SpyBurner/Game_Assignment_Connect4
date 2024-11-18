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
    void Awake()
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
        newPlayer.GetComponent<PlayerCore>().color = new Color(Random.value * num, Random.value * num, Random.value * num);
        
        newPlayer.GetComponent<PhotonView>().RPC("SetTurnID", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        
        //Vector3 newPos = transform.position + newPlayer.GetComponent<SpriteRenderer>().bounds.size.x * Vector3.right * (PhotonNetwork.LocalPlayer.ActorNumber);
        //newPlayer.GetComponent<PhotonView>().RPC("SetPosition", RpcTarget.All, newPos.x, newPos.y);
    }
}

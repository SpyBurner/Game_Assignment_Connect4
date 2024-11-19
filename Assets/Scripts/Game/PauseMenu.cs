using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public void Continue()
    {
        if (photonView.IsMine)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(2, null, raiseEventOptions, SendOptions.SendReliable);
            photonView.RPC("ConitnueRPC", RpcTarget.All);
        }

    }

    public void Quit()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(2, null, raiseEventOptions, SendOptions.SendReliable);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    [PunRPC]
    void ConitnueRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(2, null, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}

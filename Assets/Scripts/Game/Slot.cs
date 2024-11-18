using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviourPunCallbacks, IPunObservable
{
    public int x, y;
    public Color currentColor = Color.white;

    private PhotonView photonView;

    private Player occupyingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        photonView.RPC("SetName", RpcTarget.AllBuffered, "Slot " + x + " " + y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void Occupy(EventData obj)
    {
        if (!photonView.IsMine || occupyingPlayer != null)
        {
            return;
        }

        object[] data = (object[])obj.CustomData;

        occupyingPlayer = (Player)data[0];
        Color color = (Color)data[1];

        SetColor(color);
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    [PunRPC]
    public void SetName(string name)
    {
        gameObject.name = name;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(occupyingPlayer);
            stream.SendNext(GetComponent<SpriteRenderer>().color);
        }
        else
        {
            occupyingPlayer = (Player)stream.ReceiveNext();
            SetColor((Color)stream.ReceiveNext());
        }
    }
}

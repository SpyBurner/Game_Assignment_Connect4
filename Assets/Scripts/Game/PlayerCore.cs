using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCore : MonoBehaviourPunCallbacks, IPunObservable
{
    public int turnID = 0;
    public Color color = Color.red;

    public UnityEvent OnTurnStart;
    public UnityEvent OnTurnEnd;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().color = color;
        GameObject.Find("myTurn").GetComponent<Text>().text = turnID.ToString();
    }

    [PunRPC]
    public void SetTurnID(int id)
    {
        turnID = id;
    }

    [PunRPC]
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y);
    }

    public void Interact(Slot slot)
    {
        if (slot.isOccupied() || !photonView.IsMine)
        {
            return;
        }

        TurnManager turnManager = FindAnyObjectByType<TurnManager>();
        if (turnManager && turnManager.turnID != turnID)
        {
            return;
        }


        object[] data = new object[2];
        data[0] = (object)photonView.Owner;
        data[1] = new Dictionary<string, float> { { "r", color.r }, { "g", color.g }, { "b", color.b }, { "a", color.a } };

        slot.GetComponent<PhotonView>().RPC("Occupy", RpcTarget.AllBuffered, data);

        FindAnyObjectByType<TurnManager>().GetComponent<PhotonView>().RPC("AdvanceTurn", RpcTarget.AllBuffered);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(turnID);
            stream.SendNext(color.r);
            stream.SendNext(color.g);
            stream.SendNext(color.b);
            stream.SendNext(color.a);
        }
        else
        {
            turnID = (int)stream.ReceiveNext();
            color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }
}

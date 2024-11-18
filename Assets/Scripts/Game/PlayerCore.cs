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

    private GameObject pos = null;

    // Start is called before the first frame update
    void Start()
    {
        pos = GameObject.Find("P" + PhotonNetwork.LocalPlayer.ActorNumber + "Pos");
    }

    // Update is called once per frame
    void Update()
    {

        GetComponent<SpriteRenderer>().color = color;
        GameObject.Find("myTurn").GetComponent<Text>().text = turnID.ToString();

        if (!photonView.IsMine)
        {
            return;
        }
        transform.position = pos.transform.position;
    }

    [PunRPC]
    public void SetTurnID(int id)
    {
        if (photonView.IsMine)
        {
            turnID = id;
        }
    }

    [PunRPC]
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y);
    }

    public void Interact(Slot slot)
    {
        if (slot.isOccupied())
        {
            Debug.LogError("Not my view!");
            return;
        }
        else 
            Debug.LogError("My view!");

        TurnManager turnManager = FindAnyObjectByType<TurnManager>();
        if (turnManager && turnManager.turnID != turnID)
        {
            Debug.LogError("Not my turn!");
            return;
        }
        else
            Debug.Log("My turn!");


        object[] data = new object[2];
        data[0] = (object)photonView.Owner;
        data[1] = new Dictionary<string, float> { { "r", color.r }, { "g", color.g }, { "b", color.b }, { "a", color.a } };

        //slot.GetComponent<PhotonView>().RPC("Occupy", RpcTarget.AllBuffered, data);
        slot.GetComponent<PhotonView>().RPC("OnClickedByPlayer", RpcTarget.AllBuffered, data);

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

            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);
            stream.SendNext(transform.position.z);
        }
        else
        {
            turnID = (int)stream.ReceiveNext();
            color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());

            transform.position = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnManager : PhotonSingleton<TurnManager>, IPunObservable
{
    public int turnID = 1;

    public UnityEvent OnAdvanceTurn;
    // Start is called before the first frame update
    void Start()
    {
        OnAdvanceTurn = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject.Find("currentTurn").GetComponent<Text>().text = turnID.ToString();
    }

    [PunRPC]
    public void AdvanceTurn()
    {
        turnID++;
        if (turnID >= PhotonNetwork.PlayerList.Length + 1)
        {
            turnID = 1;
        }
        OnAdvanceTurn.Invoke();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(turnID);
        }
        else
        {
            turnID = (int)stream.ReceiveNext();
        }
    }
}

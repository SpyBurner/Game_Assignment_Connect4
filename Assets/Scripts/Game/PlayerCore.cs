using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCore : MonoBehaviourPunCallbacks
{
    public int turnID { get; private set; } = -1;
    public Color color = Color.red;

    public UnityEvent OnTurnStart;
    public UnityEvent OnTurnEnd;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact(Slot slot)
    {
        object[] data = new object[2];
        data[0] = photonView.Owner;
        data[1] = color;

        slot.GetComponent<PhotonView>().RPC("Occupy", RpcTarget.AllBuffered, data);
    }


}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviourPunCallbacks, IPunObservable
{
    public int rows = 6;
    public int cols = 7;
    public float gap = 2f;
    private Hashtable slots = new Hashtable();
    private bool generated = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && !generated)
        {
            init();
        }
    }

    void init()
    {
        generated = true;
        for (int i = -rows/2; i <= rows/2; i++)
        {
            for (int j = -cols/2; j <= cols/2 - 1; j++)
            {
                Vector2 pos = new Vector2(i, j) * gap;

                GameObject newSlot = PhotonNetwork.Instantiate("Slot", pos, Quaternion.identity);

                newSlot.GetComponent<Slot>().x = i;
                newSlot.GetComponent<Slot>().y = j;

                newSlot.GetComponent<PhotonView>().RPC("SetName", RpcTarget.AllBuffered, "Slot" + i + j);

                newSlot.transform.parent = this.transform;
                //slots.Add((i, j), newSlot);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}

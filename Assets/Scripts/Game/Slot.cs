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

    private Player occupyingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("SetName", RpcTarget.AllBuffered, "Slot " + x + " " + y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void Occupy(object[] obj)
    {
        if (!photonView.IsMine || occupyingPlayer != null)
        {
            return;
        }

        occupyingPlayer = (Player)obj[0];
        
        Dictionary<string, float> colorDict = (Dictionary<string, float>)obj[1];
        Color color = new Color(colorDict["r"], colorDict["g"], colorDict["b"], colorDict["a"]);

        SetColor(color);
    }

    public bool isOccupied()
    {
        return occupyingPlayer != null;
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
            object[] data = new object[3];
            data[0] = (object)photonView.Owner;
            data[1] = new Dictionary<string, float> { { "r", currentColor.r }, { "g", currentColor.g }, { "b", currentColor.b }, { "a", currentColor.a } };
            data[2] = gameObject.name;

            stream.SendNext(data);
        }
        else
        {
            object[] data = (object[])stream.ReceiveNext();

            occupyingPlayer = data[0] as Player;
            Dictionary<string, float> colorDict = data[1] as Dictionary<string, float>;
            Color color = new Color(colorDict["r"], colorDict["g"], colorDict["b"], colorDict["a"]);
            gameObject.name = (string)data[2];

            SetColor(color);
        }
    }
}

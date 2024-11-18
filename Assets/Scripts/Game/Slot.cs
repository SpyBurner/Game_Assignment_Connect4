using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Slot : MonoBehaviourPunCallbacks, IPunObservable
{
    public int x, y;
    public Color currentColor = Color.white;

    public Player occupyingPlayer = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void Occupy(object[] obj)
    {
        if (occupyingPlayer != null)
        {
            return;
        }
        
        occupyingPlayer = (Player)obj[0];
       
        Dictionary<string, float> colorDict = (Dictionary<string, float>)obj[1];
        Color color = new Color(colorDict["r"], colorDict["g"], colorDict["b"], colorDict["a"]);

        SetColor(color);
        BoardManager board = FindAnyObjectByType<BoardManager>();
        board.CheckForFourInARow(this);
    }
    [PunRPC]
    void OnClickedByPlayer(object[] obj)
    {
        Slot emptySlot = FindFarthestUnoccupiedBelow(this);
        
        emptySlot.Occupy(obj);
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
            object[] data = new object[5];
            data[0] = (object)occupyingPlayer;
            data[1] = new Dictionary<string, float> { { "r", currentColor.r }, { "g", currentColor.g }, { "b", currentColor.b }, { "a", currentColor.a } };
            data[2] = gameObject.name;
            data[3] = x;
            data[4] = y;

            stream.SendNext(data);
        }
        else
        {
            object[] data = (object[])stream.ReceiveNext();

            occupyingPlayer = data[0] as Player;
            Dictionary<string, float> colorDict = data[1] as Dictionary<string, float>;
            Color color = new Color(colorDict["r"], colorDict["g"], colorDict["b"], colorDict["a"]);
            gameObject.name = (string)data[2];
            x = (int)data[3];
            y = (int)data[4];

            SetColor(color);
        }
    }

    public Slot FindFarthestUnoccupiedBelow(Slot obj)
    {
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("The object does not have a Collider2D.");
            return null;
        }

        Vector2 startPosition = obj.transform.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPosition, Vector2.down, Mathf.Infinity);

        Slot farthestUnoccupied = obj;
        float farthestDistance = 0f;

        foreach (RaycastHit2D hit in hits)
        {
            Slot slot = hit.collider.GetComponent<Slot>();
            if (slot != null && slot.occupyingPlayer == null)
            {
                float distance = Vector2.Distance(startPosition, hit.point);

                if (distance > farthestDistance)
                {
                    farthestUnoccupied = hit.collider.gameObject.GetComponent<Slot>();
                    farthestDistance = distance;
                }
            }
        }

        return farthestUnoccupied;
    }
}

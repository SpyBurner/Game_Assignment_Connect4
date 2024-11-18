using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviourPunCallbacks, IOnEventCallback 
{
    public bool allowInput = true;

    public GameObject pauseMenuPrefab;

    private PlayerCore playerCore;

    // Start is called before the first frame update
    void Start()
    {
        playerCore = GetComponent<PlayerCore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || !allowInput)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

            Debug.Log("Raycast hit: " + hits.Length);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.tag == "Slot")
                {
                    playerCore.Interact(hit.collider.gameObject.GetComponent<Slot>());

                }

                //Only take 1st hit
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(1, null, raiseEventOptions, SendOptions.SendReliable);

            PhotonNetwork.Instantiate(pauseMenuPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            allowInput = false;
        }
        else if (photonEvent.Code == 2)
        {
            allowInput = true;
        }
    }
}

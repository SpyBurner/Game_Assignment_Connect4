using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    public bool allowInput = true;

    private PhotonView photonView;
    private PlayerCore playerCore;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerCore = GetComponent<PlayerCore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;


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
            }
        }
    }

}

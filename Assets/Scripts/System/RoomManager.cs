using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;
public class RoomManager : PhotonSingleton<RoomManager>
{
    public Text roomNameText;

    public Dropdown difficultyDropdown;
    private AIController.Difficulty botDifficulty = AIController.Difficulty.EASY;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (roomNameText != null && PhotonNetwork.InRoom)
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        }
    }

    public void SetBotDifficulty()
    {
        if (difficultyDropdown == null)
        {
            return;
        }
        switch (difficultyDropdown.value)
        {
            case 0:
                botDifficulty = AIController.Difficulty.EASY;
                break;
            case 1:
                botDifficulty = AIController.Difficulty.MEDIUM;
                break;
            case 2:
                botDifficulty = AIController.Difficulty.HARD;
                break;
            default:
                botDifficulty = AIController.Difficulty.EASY;
                break;
        }
    }

    public void SinglePlayer()
    {
        PhotonNetwork.OfflineMode = true;
        CreateRoom();
    }

    public void CreateRoom()
    {
        if (PhotonNetwork.OfflineMode)
        {
            Debug.Log("Creating offline room");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add("difficulty", botDifficulty);
            PhotonNetwork.CreateRoom(null, roomOptions);
            return;
        }

        if (roomNameText == null)
        {
            return;
        }

        string roomName = roomNameText.text;

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom()
    {
        if (roomNameText == null)
        {
            Debug.Log("Room name text is null");
            return;
        }
        string roomName = roomNameText.text;


        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.Log("Not in room");
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room");
        PhotonNetwork.LoadLevel("Selection");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Join room failed: " + message);
    }
}
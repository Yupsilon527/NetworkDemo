using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HostRoomController : MonoBehaviour
{
    public TMP_InputField RoomName;
    public TMP_InputField PlayerCount;

    public void CreateNewRoom()
    {
        string roomName = RoomName.text;

        RoomOptions options = new RoomOptions { MaxPlayers = 8 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }
}

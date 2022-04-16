using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HostRoomController : MonoBehaviour
{
    public TMP_InputField RoomName;
    public PlayerCountController PlayerCount;

    public void CreateNewRoom()
    {
        string roomName = RoomName.text;

        
        RoomOptions options = new RoomOptions { MaxPlayers = (byte)PlayerCount.GetValue() };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }
}

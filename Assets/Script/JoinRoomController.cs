using Photon.Pun;
using Photon.Realtime;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinRoomController : MonoBehaviour
{
    public Button JoinRoomButton;
    public TMP_InputField RoomName;

    void Start()
    {
        JoinRoomButton.onClick.AddListener(JoinRoomByName);
    }

    public void JoinRoomByName()
    {
        if (!PhotonNetwork.JoinRoom(RoomName.text))
            Debug.Log("Room " + RoomName.text + " does not exist");
    }
}

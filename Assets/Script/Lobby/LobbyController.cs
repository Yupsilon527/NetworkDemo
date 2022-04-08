using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI LobbyName;
    public Button StartButton;
    public override void OnEnable()
    {
        base.OnEnable();
        if (PhotonNetwork.CurrentRoom != null) 
        LobbyName.text = PhotonNetwork.CurrentRoom.Name;
        UpdateHost();
    }
    public void UpdateHost()
    {
        StartButton.interactable = PhotonNetwork.IsMasterClient;
    }
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Werewolf Game Scene");
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateHost();
    }
}

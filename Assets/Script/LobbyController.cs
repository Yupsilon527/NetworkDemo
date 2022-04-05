using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class LobbyController : MonoBehaviour
{
    public TextMeshProUGUI LobbyName;
    private void OnEnable()
    {
        if (PhotonNetwork.CurrentRoom!=null)
        LobbyName.text = PhotonNetwork.CurrentRoom.Name;
    }
}

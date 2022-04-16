using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameController : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    void OnEnable()
    {
        PlayerName.text = PhotonNetwork.LocalPlayer.NickName;
    }
}

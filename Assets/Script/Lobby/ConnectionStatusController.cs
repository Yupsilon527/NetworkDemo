using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ConnectionStatusController : MonoBehaviour
{
    public TextMeshProUGUI ConnectionStatusText;
    public void Update()
    {
        ConnectionStatusText.text = "Status: "+PhotonNetwork.NetworkClientState;
    }

}

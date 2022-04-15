using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectButtonController : MonoBehaviour, IButtonScript
{
    public void OnClicked()
    {
        PhotonNetwork.ConnectUsingSettings() ;
    }


}

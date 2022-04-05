using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizationController : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        InitializePlayer();
    }

    void InitializePlayer()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {"FinishedLoading", false},
                {"CustomPortrait", "Player"},
                {"PlayerClass", 0},
                {"WasKilled", false},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}

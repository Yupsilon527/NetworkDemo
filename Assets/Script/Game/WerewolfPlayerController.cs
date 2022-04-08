using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfPlayerController : MonoBehaviour
{
    public void OnEnable()
    {
        InitializePlayer();
    }

    void InitializePlayer()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {"FinishedLoading", true},
                {"CustomPortrait", "Player"},
                {"PlayerClass", "Spectator"},
                {"VoteTarget", -1},
                {"WasKilled", false},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}

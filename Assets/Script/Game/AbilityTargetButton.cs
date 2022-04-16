using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetButton : MonoBehaviour
{
    public Player myPlayer;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI nTargets;

    public void AssignPlayer(Player p)
    {
        myPlayer = p;
        PlayerName.text = p.NickName;
    }
    public void OnVote()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                {WerewolfGameDefines.PlayerVote, myPlayer.NickName},
            });
    }
}

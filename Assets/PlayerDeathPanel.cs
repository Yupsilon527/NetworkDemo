using TMPro;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathPanel : MonoBehaviour
{
    public enum AliveState
    {
        alive = 0,
        lynched = 1,
        mauled = 2
    }

    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerClass;
    public void AnnouncePlayerDeath(Player p)
    {
        if (p.IsLocal)
        {
            PlayerName.text = "You have been killed!";
            PlayerClass.text = "Cause of death";
        }
        else
        {
            PlayerName.text = p.NickName + " has been!";
            if (p.CustomProperties.TryGetValue("PlayerClass", out var playerclass))                
                PlayerClass.text = "Their role was "+ (string)playerclass;
        }
    }
}

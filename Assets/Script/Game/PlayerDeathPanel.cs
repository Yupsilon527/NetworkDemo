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

    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI CauseLabel;
    public void AnnouncePlayerDeath(Player p, WerewolfGameDefines.PlayerAliveState causeOfDeath)
    {
        if (p.IsLocal)
        {
            NameLabel.text = "You have been killed!";
            CauseLabel.text = "Cause of death: "+ causeOfDeath;
        }
        else
        {
            NameLabel.text = p.NickName + " has been "+ causeOfDeath+"!";
            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerClass, out var playerclass))                
                CauseLabel.text = "Their role was "+ (string)playerclass;
        }
    }
}

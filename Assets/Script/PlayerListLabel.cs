using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListLabel : MonoBehaviour
{
    public Player myPlayer;
    public TextMeshProUGUI PlayerName;

    public void AssignPlayer(Player p)
    {
        myPlayer = p;
        PlayerName.text = p.NickName;
    }
}

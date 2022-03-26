using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreController : MonoBehaviour
{
    public CTFPlayer MonitoredPlayer;
    public TextMeshProUGUI text;
    public void AssignPlayer(CTFPlayer player)
    {
        MonitoredPlayer = player;
        text.text = "Score: " + MonitoredPlayer.PlayerScore.Value;
        MonitoredPlayer.PlayerScore.OnValueChanged += (int previousValue, int newValue) =>
        {
            text.text = "Score: " + newValue;
        };
    }
}

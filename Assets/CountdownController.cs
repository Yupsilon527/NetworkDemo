using Photon.Pun;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    public TextMeshProUGUI TimerDisplay;
    public bool TimeOfDay = false;
    private void Update()
    {
        float RemainingTime = (TimeOfDay ? WerewolfGameController.main.NextNightTime : WerewolfGameController.main.NextGamePhaseTime) - Time.time;
        TimerDisplay.text = RemainingTime > 0 ? (Mathf.Ceil(RemainingTime).ToString()) : "";
    }
}

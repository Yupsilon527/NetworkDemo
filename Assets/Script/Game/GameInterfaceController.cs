using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameInterfaceController : MonoBehaviourPunCallbacks
{
    [Header("Player Class Panel")]
    public GameObject PlayerClassPanel;
    public TextMeshProUGUI PlayerClassLabel;

    [Header("Day/Night Panels")]
    public GameObject DayPanel;
    public GameObject NightPanel;

    public GameObject VotingPanel;
    public GameObject AbilityPanel;
    public GameObject DeadPanel;

    [Header("Victory Panels")]
    public GameObject AntagVictoryPanel;
    public GameObject VillageVictoryPanel;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            return;

        if (changedProps.ContainsKey("PlayerClass"))
        {
            if (WerewolfGameController.main.CurrentPhase < WerewolfGameController.GamePhase.Day)
            {
                if (changedProps.TryGetValue("PlayerClass", out var className))
                {
                    Debug.Log("Class of my player set to " + className);
                    PlayerClassLabel.text = (string)className;
                }
                StartCoroutine(ShowPlayerRole());
            }
        }
        if (changedProps.ContainsKey("WasKilled"))
        {
            if (changedProps.TryGetValue("WasKilled", out var playerDead))
            {
                    DeadPanel.SetActive((bool)playerDead);
                
            }
        }
        }
    IEnumerator ShowPlayerRole()
    {
        PlayerClassPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        PlayerClassPanel.SetActive(false);
    }
    private void Update()
    {
        if (WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Day)
        {
            DayPanel.SetActive(true);
            NightPanel.SetActive(false);
        }
        else if (WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Night)
        {
            DayPanel.SetActive(false);
            NightPanel.SetActive(true);
        }
    }
}

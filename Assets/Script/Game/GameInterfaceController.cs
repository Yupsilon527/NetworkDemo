using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameInterfaceController : MonoBehaviourPunCallbacks
{
    [Header("Player Class Panel")]
    public PlayerClassPanel PlayerClassPanel;

    [Header("Day/Night Panels")]
    public GameObject DayPanel;
    public GameObject NightPanel;

    public GameObject VotingPanel;
    public GameObject AbilityPanel;
    public GameObject DeathOverlay;
    public PlayerDeathPanel DeadPanel;

    [Header("Game Over Panels")]
    public GameOverPanel GameOverPanel;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("WasKilled") && changedProps.TryGetValue("WasKilled", out var playerDead))
            {

                if (targetPlayer.UserId == PhotonNetwork.LocalPlayer.UserId)
                    DeathOverlay.SetActive((bool)playerDead);
                if ((bool)playerDead)
            {
                Debug.Log(targetPlayer.NickName + " was killed UI!");
                DeadPanel.AnnouncePlayerDeath(targetPlayer);
                StartCoroutine(ShowPlayerDeath());
            }
            }
        

        if (targetPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            return;

        if (changedProps.ContainsKey("PlayerClass"))
        {
            if (WerewolfGameController.main.CurrentPhase < WerewolfGameController.GamePhase.Day)
            {
                if (changedProps.TryGetValue("PlayerClass", out var className))
                {
                    Debug.Log("Class of my player set to " + className);
                    PlayerClassPanel.gameObject.SetActive(true);
                    PlayerClassPanel.AnnouncePlayerClass((string)className);
                }
            }
        }
    }
    IEnumerator ShowPlayerDeath()
    {
        DeadPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(9999999999);
        DeadPanel.gameObject.SetActive(false);
    }
    WerewolfGameController.GamePhase currentUI = WerewolfGameController.GamePhase.Loading;
    private void Update()
    {
        if (currentUI!= WerewolfGameController.main.CurrentPhase)
        {
            currentUI = WerewolfGameController.main.CurrentPhase;

            if (DeathOverlay.activeSelf) return;
            Debug.Log("UI changephase to "+ currentUI);
            DayPanel.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Day);
            NightPanel.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Night);
            VotingPanel.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Voting);
            GameOverPanel.gameObject.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.PostGame);

            if (currentUI == WerewolfGameController.GamePhase.Night)
                StartCoroutine(DisplayNightAbilities());
            AbilityPanel.SetActive(false);
        }
    }
    IEnumerator DisplayNightAbilities()
    {
            yield return new WaitForSeconds(3);
            if (!DeathOverlay.activeSelf && WerewolfGameController.main.IsPlayerAntagonist(PhotonNetwork.LocalPlayer))
                AbilityPanel.SetActive(true);
    }
}

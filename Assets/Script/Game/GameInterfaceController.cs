using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameInterfaceController : MonoBehaviourPunCallbacks
{
    [Header("Player Class Panel")]
    bool HasPlayerDied = false;
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
                HasPlayerDied = (bool)playerDead;
                if ((bool)playerDead)
            {
                Debug.Log(targetPlayer.NickName + " was killed UI!");
                DeadPanel.AnnouncePlayerDeath(targetPlayer);
                if (TooltipCoroutine != null)
                    StopCoroutine(TooltipCoroutine);
                TooltipCoroutine = StartCoroutine(ShowPlayerDeath());
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
    Coroutine TooltipCoroutine;
    IEnumerator ShowPlayerDeath()
    {
        DeadPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        DeadPanel.gameObject.SetActive(false);
    }
    WerewolfGameController.GamePhase currentUI = WerewolfGameController.GamePhase.Loading;
    private void Update()
    {
        if (currentUI!= WerewolfGameController.main.CurrentPhase)
        {
            currentUI = WerewolfGameController.main.CurrentPhase;

            GameOverPanel.gameObject.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.PostGame);

            DeathOverlay.SetActive(HasPlayerDied);

            Debug.Log("UI changephase to "+ currentUI);
            DayPanel.SetActive(!HasPlayerDied && WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Day);
            NightPanel.SetActive(!HasPlayerDied && WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Night);
            VotingPanel.SetActive(!HasPlayerDied && WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Voting);

            if (!HasPlayerDied && currentUI == WerewolfGameController.GamePhase.Night)
            {
                if (TooltipCoroutine != null)
                    StopCoroutine(TooltipCoroutine);
                TooltipCoroutine = StartCoroutine(DisplayNightAbilities());
            }
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

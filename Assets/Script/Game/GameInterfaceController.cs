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
        if (changedProps.ContainsKey(WerewolfGameDefines.PlayerState) && changedProps.TryGetValue(WerewolfGameDefines.PlayerState, out var playerDead))
            {

            if (targetPlayer.UserId == PhotonNetwork.LocalPlayer.UserId)
                HasPlayerDied = (int)playerDead > 0;
                if (HasPlayerDied)
            {
                Debug.Log(targetPlayer.NickName + " was killed UI!");
                DeadPanel.AnnouncePlayerDeath(targetPlayer, (WerewolfGameDefines.PlayerAliveState)playerDead);
                if (TooltipCoroutine != null)
                    StopCoroutine(TooltipCoroutine);
                TooltipCoroutine = StartCoroutine(ShowPlayerDeath());
            }
            }
        

        if (targetPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            return;

        if (changedProps.ContainsKey(WerewolfGameDefines.PlayerClass))
        {
            if (WerewolfGameController.main.CurrentPhase < WerewolfGameController.GamePhase.Day)
            {
                if (changedProps.TryGetValue(WerewolfGameDefines.PlayerClass, out var className))
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
        yield return new WaitForSeconds(WerewolfGameDefines.UIDeathPanelTime);
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
                StartCoroutine(DisplayNightAbilities());
            }
            AbilityPanel.SetActive(false);
        }
    }
    IEnumerator DisplayNightAbilities()
    {
            yield return new WaitForSeconds(WerewolfGameDefines.UIAbilityDelayTime);
            if (!DeathOverlay.activeSelf && WerewolfGameController.main.IsPlayerAntagonist(PhotonNetwork.LocalPlayer))
                AbilityPanel.SetActive(true);
    }
}

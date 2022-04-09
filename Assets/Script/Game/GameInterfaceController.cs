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
    public GameObject DeathOverlay;
    public PlayerDeathPanel DeadPanel;

    [Header("Game Over Panels")]
    public GameOverPanel GameOverPanel;

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
                DeathOverlay.SetActive((bool)playerDead);
                if ((bool)playerDead)
                    WerewolfGameController.main.pview.RPC("OnPlayerDeath", RpcTarget.AllViaServer, targetPlayer);
            }
        }
    }
    [PunRPC]
    void OnPlayerDeath(Player deadplayer, PhotonMessageInfo info)
    {
        DeadPanel.AnnouncePlayerDeath(deadplayer);
        StartCoroutine(ShowPlayerDeath());
    }
    IEnumerator ShowPlayerDeath()
    {
        DeadPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        DeadPanel.gameObject.SetActive(false);
    }
    IEnumerator ShowPlayerRole()
    {
        PlayerClassPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        PlayerClassPanel.SetActive(false);
    }
    WerewolfGameController.GamePhase currentUI = WerewolfGameController.GamePhase.Loading;
    private void Update()
    {
        if (currentUI!= WerewolfGameController.main.CurrentPhase)
        {
            currentUI = WerewolfGameController.main.CurrentPhase;

            if (DeathOverlay.activeSelf) return;
            DayPanel.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Day);
            NightPanel.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Night);
            VotingPanel.SetActive(WerewolfGameController.main.CurrentPhase == WerewolfGameController.GamePhase.Voting);

            if (currentUI == WerewolfGameController.GamePhase.Night)
                StartCoroutine(DisplayNightAbilities());
            AbilityPanel.SetActive(false);
        }
    }
    IEnumerator DisplayNightAbilities()
    {
            if (DeathOverlay.activeSelf) return;
        yield return new WaitForSeconds(3);
        if (WerewolfGameController.main.IsPlayerAntagonist(PhotonNetwork.LocalPlayer))
            AbilityPanel.SetActive(true);
    }
}

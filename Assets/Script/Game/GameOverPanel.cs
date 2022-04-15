using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public GameObject AntagGameOver;
    public GameObject VillageGameOver;
    public Button RestartGameButton;

    void OnEnable()
    {
        if (WerewolfGameController.main.GetLivingAntags().Length>0)
        {
            ShowAntagVictory();
        }
        else
        {
            ShowVillageVictory();
        }
    }
    public void ShowAntagVictory()
    {
        AntagGameOver.SetActive(true);
        VillageGameOver.SetActive(false);
        RestartGameButton.gameObject.SetActive(RestartGameButton != null && PhotonNetwork.IsMasterClient);
    }
    public void ShowVillageVictory()
    {
        AntagGameOver.SetActive(false);
        VillageGameOver.SetActive(true);
        RestartGameButton.gameObject.SetActive(RestartGameButton != null && PhotonNetwork.IsMasterClient);
    }
}

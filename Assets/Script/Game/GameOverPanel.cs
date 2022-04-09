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

    public void ShowAntagVictory()
    {
        AntagGameOver.SetActive(true);
        VillageGameOver.SetActive(false);
        gameObject.SetActive(true);
    }
    public void ShowVillageVictory()
    {
        AntagGameOver.SetActive(false);
        VillageGameOver.SetActive(true);
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        RestartGameButton.gameObject.SetActive(RestartGameButton != null && PhotonNetwork.IsMasterClient);

    }
}

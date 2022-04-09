using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingPanelController : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public Button ButtonYes;
    public Button ButtonNo;

    private void OnEnable()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("VotedPlayer", out object newGamePhase))
        {
            playerName.text = (string)newGamePhase;
        }
        StartCoroutine(ToggleVotingButtons());
    }
    IEnumerator ToggleVotingButtons()
    {
        ButtonYes.interactable = false;
        ButtonNo.interactable = false;

        yield return new WaitWhile(() =>
        {
            return WerewolfGameController.main.CurrentPhase != WerewolfGameController.GamePhase.Voting;
        });

        ButtonYes.interactable = true;
        ButtonNo.interactable = true;
    }
    public void OnPlayerVoteYes()
    {
        ButtonYes.interactable = false;
        ButtonNo.interactable = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VotedPlayer", "Y" } });
    }
    public void OnPlayerVoteNo()
    {
        ButtonYes.interactable = true;
        ButtonNo.interactable = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VotedPlayer", "N" } });
    }
}

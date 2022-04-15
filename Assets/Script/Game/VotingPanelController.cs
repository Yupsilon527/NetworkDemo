using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingPanelController : MonoBehaviour
{
    string VotedPlayerName = "";
    public TextMeshProUGUI playerName;
    public Button ButtonYes;
    public Button ButtonNo;

    private void OnEnable()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("VotedPlayer", out object voteName))
        {
            VotedPlayerName = (string)voteName;
        }
        StartCoroutine(ToggleVotingButtons());
    }
    IEnumerator ToggleVotingButtons()
    {
        playerName.text = VotedPlayerName;
        ButtonYes.interactable = false;
        ButtonNo.interactable = false;

        yield return new WaitWhile(() =>
        {
            return WerewolfGameController.main.CurrentPhase != WerewolfGameController.GamePhase.Voting;
        });

        ButtonYes.interactable = VotedPlayerName!=PhotonNetwork.LocalPlayer.NickName;
        ButtonNo.interactable = VotedPlayerName != PhotonNetwork.LocalPlayer.NickName;
    }
    public void OnPlayerVoteYes()
    {
        ButtonYes.interactable = false;
        ButtonNo.interactable = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VoteTarget", "Y" } });
    }
    public void OnPlayerVoteNo()
    {
        ButtonYes.interactable = true;
        ButtonNo.interactable = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VoteTarget", "N" } });
    }
}

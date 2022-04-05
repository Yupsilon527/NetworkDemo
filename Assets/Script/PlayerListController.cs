using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListController : MonoBehaviourPunCallbacks
{
    public override void OnEnable()
    {
        base.OnEnable();
        LoadPlayerList();
    }
    public override void OnJoinedRoom()
    {
        LoadPlayerList();
    }
    void ClearPlayerList()
    {
        players = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    int players = 0;
     void LoadPlayerList()
    {
        ClearPlayerList();
        foreach (Player p in PhotonNetwork.PlayerList)
            RegisterNewPlayer(p);
    }
    void RegisterNewPlayer(Player player)
    {
        if (transform.GetChild(players) != null)
        {
            Transform childTransform = transform.GetChild(players);
            childTransform.gameObject.SetActive(true);
            childTransform.GetComponent<PlayerListLabel>().AssignPlayer(player);
        }
        else
        {
            GameObject nChild = GameObject.Instantiate(transform.GetChild(0).gameObject);
            nChild.SetActive(true);
            nChild.GetComponent<PlayerListLabel>().AssignPlayer(player);
            nChild.transform.SetParent(transform);

            RectTransform rectT = nChild.GetComponent<RectTransform>();
            rectT.anchoredPosition = Vector2.down * rectT.rect.height * (players + .5f);
        }
        players++;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RegisterNewPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (PlayerListLabel label in GetComponentsInChildren<PlayerListLabel>())
        {
            if (label.myPlayer == otherPlayer)
            {
                label.gameObject.SetActive(false);
                label.transform.SetAsLastSibling();
                players--;
            }
        }
    }
}

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
        if (transform.childCount > players)
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
            RectTransform rectP = transform.GetChild(0).GetComponent<RectTransform>();
            
            rectT.anchoredPosition = rectP.anchoredPosition + Vector2.down * players * rectT.sizeDelta.y;
            rectT.anchorMin = rectP.anchorMin;
            rectT.anchorMax = rectP.anchorMax;
            rectT.offsetMin = rectP.offsetMin;
            rectT.offsetMax = rectP.offsetMax;
            rectT.sizeDelta = rectP.sizeDelta;
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

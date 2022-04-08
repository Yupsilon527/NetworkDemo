using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccusePlayerMenu : MonoBehaviour
{
    public void OnEnable()
    {
        LoadPlayerList();
    }
    private void OnDisable()
    {
        ClearPlayerList();
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
        foreach (Player p in WerewolfGameController.main.GetAllLivingPlayers())
        {
            if (!p.IsLocal)
                RegisterNewPlayer(p);
        }
    }
    void RegisterNewPlayer(Player player)
    {
        if (transform.childCount <= players)
        {
            Transform childTransform = transform.GetChild(players);
            childTransform.gameObject.SetActive(true);
            childTransform.GetComponent<AccusePlayerButton>().AssignPlayer(player);
        }
        else
        {
            GameObject nChild = GameObject.Instantiate(transform.GetChild(0).gameObject);
            nChild.SetActive(true);
            nChild.GetComponent<AccusePlayerButton>().AssignPlayer(player);
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
}

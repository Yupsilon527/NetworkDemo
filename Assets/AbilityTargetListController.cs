using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AbilityTargetListController : MonoBehaviour
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
            if (IsValidTargetForAbility(p))
                RegisterNewPlayer(p);
        }
    }
    bool IsValidTargetForAbility(Player target)
    {
        return !target.IsLocal && WerewolfGameController.main.IsPlayerAlive(target) && !WerewolfGameController.main.IsPlayerAntagonist(target);
    }
    void RegisterNewPlayer(Player player)
    {
        if (players < transform.childCount)
        {
            Transform childTransform = transform.GetChild(players);
            childTransform.gameObject.SetActive(true);
            childTransform.GetComponent<AccusePlayerButton>().AssignPlayer(player);
        }
        else
        {
            GameObject nChild = GameObject.Instantiate(transform.GetChild(0).gameObject);
            nChild.SetActive(true);
            nChild.GetComponent<AbilityTargetButton>().AssignPlayer(player);
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

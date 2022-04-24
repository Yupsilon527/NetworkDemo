using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayPanelController : MonoBehaviour
{
    public TextMeshProUGUI FirstDayText;
    public TextMeshProUGUI LynchText;
    public AccusationListController AccusationList;
    private void OnEnable()
    {
        bool firstDay = WerewolfGameController.main.IsFirstDay();
        FirstDayText.gameObject.SetActive(firstDay);
        LynchText.gameObject.SetActive(!firstDay);
        AccusationList.gameObject.SetActive(!firstDay);
    }
}

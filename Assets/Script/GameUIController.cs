using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public static GameUIController main;
    public TextMeshProUGUI scoreLabel;
    private void Awake()
    {
        main = this;
    }
    public void UpdatePlayerScore(int oldval, int newval)
    {
        scoreLabel.text = "Score: " + newval;
    }
}

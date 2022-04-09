using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGameButton : MonoBehaviour, IButtonScript
{

    public void OnClicked()
    {
        WerewolfGameController.main.RestartGame();
    }
}

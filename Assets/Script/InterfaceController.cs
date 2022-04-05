using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Android;

public class InterfaceController : MonoBehaviour
{
    public GameObject ServerUI;
    public GameObject LobbyUI;
    public GameObject GameUI;
    public bool HasConnected()
    {
        return NetworkManager.Singleton.IsListening;
    }
    public bool IsInGame()
    {
        return GameController.main.IsGameInProgress();
    }
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
    private void Update()
    {
        if (HasConnected())
        {
            LobbyUI.SetActive(!IsInGame());
            if (GameUI != null)
                GameUI.SetActive(IsInGame());
            ServerUI.SetActive(false);
        }
        else
        {
            ServerUI.SetActive(true);
            LobbyUI.SetActive(false);
            if (GameUI!=null)
            GameUI.SetActive(false);
        }
    }
}

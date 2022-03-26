using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    public GameObject ServerUI;
    public GameObject LobbyUI;
    public GameObject GameUI;
    public bool HasConnected()
    {
        return NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsListening;
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
            GameUI.SetActive(IsInGame());
            ServerUI.SetActive(false);
        }
        else
        {
            ServerUI.SetActive(true);
            LobbyUI.SetActive(false);
            GameUI.SetActive(false);
        }
    }
}

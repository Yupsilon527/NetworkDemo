
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class LoginMenuController : MonoBehaviour
{
    public TMP_InputField PlayerNameInput;
    public Button loginButton;

    protected void Awake()
    {
        PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
        loginButton.onClick .AddListener( OnLoginButtonClicked);
    }


    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }
}

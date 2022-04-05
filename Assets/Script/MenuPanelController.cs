using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MenuPanelController : MonoBehaviourPunCallbacks
{
    public enum MenuItem
    {
        login=0,
        main=1,
        host=2,
        join=3,
        inside=4
    }
    public GameObject loginMenu;
    public GameObject mainMenu;
    public GameObject hostMenu;
    public GameObject joinMenu;
    public GameObject insideMenu;
    private void Start()
    {
        SetActivePanel(MenuItem.login);
    }
    public void SetActivePanel(int activePanel)
    {
        SetActivePanel((MenuItem)activePanel);
    }
    public void SetActivePanel(MenuItem activePanel)
    {
        loginMenu.SetActive(activePanel == MenuItem.login);
        mainMenu.SetActive(activePanel == MenuItem.main);
        hostMenu.SetActive(activePanel == MenuItem.host);
        joinMenu.SetActive(activePanel == MenuItem.join);
        insideMenu.SetActive(activePanel == MenuItem.inside);
    }

    public override void OnConnectedToMaster()
    {
        SetActivePanel(MenuItem.main);
    }
    public override void OnJoinedLobby()
    {
        SetActivePanel(MenuItem.main);
    }
    public override void OnJoinedRoom()
    {
        SetActivePanel(MenuItem.inside);
    }

}

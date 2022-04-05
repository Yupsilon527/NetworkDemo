using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectInterfaceController : MonoBehaviour
{

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        GameController.main.OnServerStarted();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        GameController.main.OnServerStarted();
    }
    public void JoinServer()
    {
        NetworkManager.Singleton.StartClient();
    }
}

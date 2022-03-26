using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    public GameObject StartGameBtn;
    public GameObject DisconnectBtn;

    private void OnEnable()
    {
        if (NetworkManager.Singleton!=null)
        StartGameBtn.GetComponent<Button>().interactable = NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;
        
    }
}

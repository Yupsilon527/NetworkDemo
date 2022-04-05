using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIController : MonoBehaviour
{
    public TextMeshProUGUI Status;
    public GameObject StartGameBtn;
    public GameObject DisconnectBtn;

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            if (StartGameBtn != null)
                StartGameBtn.GetComponent<Button>().interactable = NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;
            if (Status!=null)
            {
                //if (NetworkManager.Singleton.ServerClientId>0 )
                //{
                    Status.text = "Listening "+ NetworkManager.Singleton.ServerClientId;
                /*}
                else if (NetworkManager.Singleton.IsHost)
                {
                    Status.text = "Host";
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    Status.text = "Client";
                }
                else
                {
                    Status.text = "Finding Server";
                }*/
            }
        }        
    }
}

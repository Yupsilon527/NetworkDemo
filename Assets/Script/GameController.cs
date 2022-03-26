using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameController : MonoBehaviour
{
    public GameObject LevelGeometryPrefab;
    public GameObject PlayerPrefab;
    public GameObject HomePrefab;
    public GameObject ObjectivePrefab;
    public GameObject LobbyPrefab;

    public static GameController main;
    public int AbsoluteMaxClients = 4;
    private void Awake()
    {
        main = this;
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientID) =>
        {
            if (NetworkManager.Singleton.ConnectedClients.Count > AbsoluteMaxClients)
            {
                NetworkManager.Singleton.DisconnectClient(clientID);
            }

        };
    }
    public virtual void InitFunctions()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
            return;
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientID) =>
    {
        if (VariableController.main != null)
        {
            VariableController.main.ChangeColorForPlayer(clientID, Color.red);
        }

    };
        VariableController.main.ChangeColorForPlayer(NetworkManager.Singleton.LocalClientId,Color.red);
    }
    public virtual void OnServerStarted()
    {
        Debug.Log("Server started");
        SpawnPrefab(LobbyPrefab);
        InitFunctions();
    }
    public virtual void StartNewGame()
    {
        Debug.Log("New Game Started");
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
            return;
        if (NetworkManager.Singleton.ConnectedClients.Count == 0)
            return;
        VariableController.main.UpdatePlayers();
        InitLevel();
        SpawnPlayers();
    }
    public bool IsGameInProgress()
    {
        return ObjectiveController.main!=null;
    }
    public virtual void InitLevel()
    {
        SpawnPrefab(LevelGeometryPrefab);
        SpawnPrefab(ObjectivePrefab);
    }
    public virtual void SpawnPrefab(GameObject prefab)
    {
        if (prefab.GetComponent< NetworkObject>( )!=null)
        {
            GameObject newObject = GameObject.Instantiate(prefab);

            if (newObject.TryGetComponent(out NetworkObject nObj))
            {
                nObj.Spawn();
                nObj.transform.position = Vector3.zero;
            }
                }
    }
    public virtual void SpawnPlayers()
    {
        players = new List<CTFPlayer>();
        int nPlayerID = 0;
        foreach (KeyValuePair<ulong, NetworkClient> player in NetworkManager.Singleton.ConnectedClients)
        {
            SpawnPrefabForPlayer(PlayerPrefab, nPlayerID, player.Value.ClientId);
            SpawnPrefabForPlayer(HomePrefab, nPlayerID, player.Value.ClientId);
            nPlayerID++;
        }
    }
    public virtual void SpawnPrefabForPlayer(GameObject prefab,int plID, ulong clientID)
    {
        if (prefab.GetComponent<NetworkObject>() != null)
        {
            GameObject newObject = GameObject.Instantiate(prefab);

            if (newObject.TryGetComponent(out NetworkObject nObj))
            {
                Transform startPos = LevelGeometryPrefab.transform.Find("Player Spawns");
                if (startPos != null && plID < startPos.childCount)
                    nObj.transform.position = startPos.GetChild(plID).position;
                if (nObj.TryGetComponent(out IPlayerController interfaceplayer))
                    interfaceplayer.SpawnWithPlayerID(plID);
                if (nObj.TryGetComponent(out CTFPlayer player))
                    players.Add(player);
                nObj.SpawnWithOwnership(clientID);
            }
        }
    }

    public List<CTFPlayer> players;

}

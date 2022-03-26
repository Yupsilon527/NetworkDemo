using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VariableController : NetworkBehaviour
{
    [SerializeField] Dictionary<ulong, Color> playerColors = new Dictionary<ulong, Color>();

    public static VariableController main;
    private void Awake()
    {
        main = this;
    }

    public void ChangeColorForPlayer(ulong playerId, Color c)
    {
        if (IsServer || IsHost)
            UpdatePlayerColorOnClientRpc(playerId, c);
        else
            SetCustomPlayerColorServerRpc(playerId, c);
    }
    void AssignColorForPlayer(ulong client, Color color)
    {
        if (playerColors.ContainsKey(client))
        {
            playerColors[client] = color;
        }
        else
        {
            playerColors.Add(client, color);
        }
    }

    [ClientRpc]
    void UpdatePlayerColorOnClientRpc(ulong clientID, Color playerColor)
    {
        Debug.Log("AssignColorForPlayer " + clientID + ": " + playerColor);
        AssignColorForPlayer(clientID, playerColor);
    }
    [ServerRpc(RequireOwnership = false)]
    void SetCustomPlayerColorServerRpc(ulong clientID, Color playerColor)
    {
        Debug.Log("SetCustomPlayerColorServerRpc " + clientID + ": " + playerColor);
        UpdatePlayerColorOnClientRpc(clientID, playerColor);
    }
    public Color GetColorForPlayer(ulong clientID)
    {
        if (playerColors.TryGetValue(clientID, out Color val))
            return val;
        return Color.white;
    }
    public void UpdatePlayers()
    {
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
            UpdatePlayerColorOnClientRpc(clientID, playerColors[clientID]);
    }
}

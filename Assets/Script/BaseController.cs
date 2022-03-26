using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class BaseController : NetworkBehaviour, IPlayerController
{
    public int AssignedPlayer;

    public void SpawnWithPlayerID(int playerID)
    {
        AssignedPlayer = playerID;
        name = "Player " + (playerID + 1)+" Home";
    }


    public bool IsPlayerControlled()
    {
        return IsOwner;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.TryGetComponent(out CTFPlayer player))
        {
            if (player.PlayerID.Value == AssignedPlayer)
            {
                player.OnReachBase();
            }
        }
    }
}

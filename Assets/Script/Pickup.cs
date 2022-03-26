using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider))]
public class Pickup : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.TryGetComponent(out CTFPlayer player))
        {
                player.PickupItem(this);
            
        }
    }
    public void OnPlayerPickup()
    {
        GetComponent<Collider>().enabled = false;
    }
    public void OnPlayerReturn()
    {
        ObjectiveController.main.DespawnObject(GetComponent<NetworkObject>()) ;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<Collider>().enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectiveController : NetworkBehaviour
{
    public static ObjectiveController main;
    public float SpawnWait;
    public float SpawnInterval;
    public int PickupsMax;
    public GameObject Pickup;

    List<NetworkObject> activePickups = new List<NetworkObject>();
    NetworkObjectPool myPool;
    private void Awake()
    {
        main = this;
        myPool = GetComponent<NetworkObjectPool>();
        myPool.AddPrefab(Pickup,6);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if ((IsServer || IsHost))
            SpawnCoroutine = StartCoroutine(StartSpawning());
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if ((IsServer || IsHost) && SpawnCoroutine != null)
            StopCoroutine(SpawnCoroutine);
    }
    Coroutine SpawnCoroutine;
    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(SpawnWait);
        spawn:
        if (TrySpawnObject())
        {
            yield return new WaitForSeconds(SpawnInterval);
        }
        else
        {
            yield return new WaitForSeconds(1);
        }
        goto spawn;
    }
    bool TrySpawnObject()
    {
        if (activePickups.Count >= PickupsMax)
        {
            Debug.Log("Pickup limit reached");
            return false;
        }
        Transform point = null;
        foreach (Transform child in transform)
        {
                bool valid = true;
            if (Random.value > .33f)
            { valid = false; }
            else
            {
                foreach (NetworkObject pickup in activePickups)
                {
                    if ((pickup.transform.position - child.transform.position).sqrMagnitude == 0)
                    {
                        valid = false;
                    }
                }
            }
                if (!valid)
                    continue;
            point = child;
            break;
        }
        if (point == null)
        {
            Debug.Log("No spawnpoint found");
            return false;
        }
        NetworkObject newObject = myPool.GetNetworkObject(Pickup, point.transform.position, Quaternion.identity);
        newObject.Spawn();
        activePickups.Add(newObject);
        return true;
    }
    public void DespawnObject(NetworkObject target)
    {
        if (activePickups.Contains(target))
        {
            target.Despawn();
            myPool.ReturnNetworkObject(target,Pickup);
            activePickups.Remove(target);
        }
    }
}

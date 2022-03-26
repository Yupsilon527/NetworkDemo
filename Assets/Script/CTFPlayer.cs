using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class CTFPlayer : NetworkBehaviour, IPlayerController
{
    public NetworkVariable<int> PlayerScore;
    public NetworkVariable<int> PlayerID;
    Rigidbody rbody;
    NetworkObject nObj;
    public Color[] PlayerColors = new Color[]
    {
        Color.cyan,
        Color.magenta,
        Color.yellow,
        Color.white
    };
    public void SpawnWithPlayerID(int playerID)
    {
        PlayerID.Value = playerID;
        PlayerScore.Value = 0;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GetComponent<NetworkRigidbody>().enabled = IsPlayerControlled();

        if (VariableController.main!=null)
        {
        GetComponent<MeshRenderer>().material.color = VariableController.main.GetColorForPlayer(OwnerClientId);
        }
    }
    public bool IsPlayerControlled()
    {
        return NetworkManager.Singleton.LocalClientId == OwnerClientId;
    }
    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        nObj = GetComponent<NetworkObject>();
        PlayerID.OnValueChanged += OnChangeID;
        if (IsPlayerControlled())
        {
            PlayerScore.OnValueChanged += GameUIController.main.UpdatePlayerScore;
        }
    }
    void OnChangeID(int oldID, int newID)
    {
        name = "Player " + (newID + 1);
    }
    public void Update()
    {
        if (IsPlayerControlled())
        {
            HandlePlayerInput();
            UpdateCamera();
        }
    }
    public float Speed = 3;
    public float Rotation = 60;
    float GetForwardSpeed()
    {
        return Speed * (myPickup == null ? 1 : .75f);
    }
    void UpdateCamera()
    {
        Camera.main.transform.position = transform.position + Vector3.up - 10 * transform.forward;
        Camera.main.transform.rotation = Quaternion.Lerp(  Camera.main.transform.rotation, transform.rotation, 99999);
    }
    void HandlePlayerInput()
    {
        rbody.velocity = new Vector3( 0 , rbody.velocity.y, 0) + transform.forward * Input.GetAxis("Vertical") * GetForwardSpeed();
        rbody.angularVelocity = Vector3.up * Input.GetAxis("Horizontal") * Rotation;
    }

    Pickup myPickup;
    public void PickupItem(Pickup myItem)
    {
        if (myPickup != null)
            return;
        myPickup = myItem;
        if (IsServer)
        {
            myPickup.OnPlayerPickup();
            myPickup.transform.SetParent(transform);
            myPickup.transform.localPosition = Vector3.up;
        }
    }
    public void OnReachBase()
    {
        if (myPickup == null)
            return;
        PlayerScore.Value++;
        myPickup.OnPlayerReturn();
        myPickup = null;
    }
}

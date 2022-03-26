using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController
{
    public void SpawnWithPlayerID(int playerID);
    public bool IsPlayerControlled();

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerColorUpdater : MonoBehaviour
{
    public Color myColor;
    public void ChangePlayerColor()
    {
        if (VariableController.main!=null)
            {
            VariableController.main.ChangeColorForPlayer(NetworkManager.Singleton.LocalClientId, myColor);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class IPinputController : MonoBehaviour
{
    public TMP_InputField[] fields = new TMP_InputField[4];
    
    public void OnIPUpdate()
    {
        if (NetworkManager.Singleton.TryGetComponent(out UNetTransport transport))
        {
            transport.ConnectAddress = fields[0].text + "." + fields[1].text + "." + fields[2].text + "." + fields[3].text;
        }
    }
}

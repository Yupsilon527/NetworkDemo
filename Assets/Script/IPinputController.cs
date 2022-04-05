using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class IPinputController : MonoBehaviour
{
    TMP_InputField[] fields = new TMP_InputField[4];

    private void Start()
    {
        fields = GetComponentsInChildren<TMP_InputField>();

        string Address = IPManager.GetIP(ADDRESSFAM.IPv4);

        string number = "";
        int field = 0;
        for (int I = 0; I<Address.Length; I++)
        {
            if (Address[I] == '.')
            {
                fields[field].text = number;
                number = "";
                field++;
            }
            else if ( I == Address.Length - 1)
            {
                fields[field].text = number + Address[I];
            }
            else
            {
                number += Address[I];
            }
        }
        OnIPUpdate();
}

    public void OnIPUpdate()
    {
        if (NetworkManager.Singleton.TryGetComponent(out UNetTransport transport))
        {
            transport.ConnectAddress = fields[0].text + "." + fields[1].text + "." + fields[2].text + "." + fields[3].text;
        }
    }
}

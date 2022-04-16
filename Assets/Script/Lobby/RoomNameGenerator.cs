using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomNameGenerator : MonoBehaviour
{
    public TMP_InputField InputField;

    void Awake()
    {
        InputField.text = "Room " + Random.Range(10, 100);
    }
}

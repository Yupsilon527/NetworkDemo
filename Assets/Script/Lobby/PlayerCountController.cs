using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerCountController : MonoBehaviour
{
    public TMP_InputField InputField;
    public int InitialValue = 8;
    public int MinPlayers = 3;
    public int MaxPlayers = 20;

    void Start()
    {
        InputField.text = InitialValue.ToString();
        OnInputFieldUpdate();
    }

    public void OnInputFieldUpdate()
    {
        int val = MinPlayers;
        if (int.TryParse(InputField.text, out val))
        {
            val = Mathf.Clamp(val, MinPlayers, MaxPlayers);
        }
        InputField.text = val.ToString();
    }
    public int GetValue()
    {
        if (int.TryParse(InputField.text, out int val))
        {
            return val;
        }
        return MinPlayers;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IPtoTextOutput : MonoBehaviour
{
    public TMP_InputField[] fields = new TMP_InputField[4];
    public TextMeshProUGUI outputIP;
    public TextMeshProUGUI outputName;

    private void Awake()
    {
        foreach (TMP_InputField  field in fields)
        {
            field.onValueChanged .AddListener( OnIPUpdate);
        }
    }
    public void OnIPUpdate(string val )
    {
        outputIP .text = fields[0].text + "." + fields[1].text + "." + fields[2].text + "." + fields[3].text;
        outputName.text = (int.Parse(fields[0].text) * int.Parse(fields[1].text) * int.Parse(fields[2].text) * int.Parse(fields[3].text))+"";
    }
}

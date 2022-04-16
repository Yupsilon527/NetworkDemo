using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerClassPanel : MonoBehaviour
{
    public TextMeshProUGUI ClassLabel;
    public void AnnouncePlayerClass(string className)
    {
        ClassLabel.text = className;
        if (DisappearCoroutine != null)
            StopCoroutine(DisappearCoroutine);
        DisappearCoroutine = StartCoroutine(Disappear(3));
    }
    Coroutine DisappearCoroutine;
    IEnumerator Disappear(float dur)
    {
        yield return new WaitForSeconds(dur);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InternationText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string[] localize;

    public void SetLocalize(int index) 
    {
        text.text = localize[index];
    }
}

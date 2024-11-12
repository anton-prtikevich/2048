using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class Language : MonoBehaviour
{
    #if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string GetLang();
    #endif
    public string CurrentLanguage = "en";
    public string[] Languages;

    public TextMeshProUGUI langText;
    
    [SerializeField] private InternationText[] internationTexts;

    [ContextMenu("FindInternationTexts")]
    private void FindInternationTexts()
    {
        internationTexts = FindObjectsOfType<InternationText>(true);
    }

    public void SetLanguage(int indexLang)
    {
        CurrentLanguage = Languages[indexLang];

        langText.text = CurrentLanguage;
        
        foreach (var internationText in internationTexts)
        {
            internationText.SetLocalize(indexLang);
        }
    }

    private void Awake()
    {
        #if UNITY_WEBGL
        CurrentLanguage = GetLang();
        langText.text = CurrentLanguage;
        #endif
    }
}

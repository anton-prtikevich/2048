using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using YG;

public class Language : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdownItem;
    [SerializeField] private YandexGame yandexGame;
    [SerializeField] private string CurrentLanguage = "en";
    [SerializeField] private string[] Languages = new string[]
    {
        "ru",
        "en",
        "tr",
        "az",
        "be",
        "he",
        "hy",
        "ka",
        "et",
        "fr",
        "kk",
        "ky",
        "lt",
        "lv",
        "ro",
        "tg",
        "tk",
        "uk",
        "uz",
        "es",
        "pt",
        "ar",
        "id",
        "ja",
        "it",
        "de",
        "hi",
    };

    
    private void OnEnable() => YandexGame.SwitchLangEvent += SeDropdownLang;

    private void OnDisable() => YandexGame.SwitchLangEvent -= SeDropdownLang;

    public void SetLanguage(int index)
    {
        CurrentLanguage = Languages[index];

        yandexGame.SetLanguage(CurrentLanguage);
    }

    private void SeDropdownLang(string lang)
    {
        CurrentLanguage = lang;
        dropdownItem.value = System.Array.IndexOf(Languages, CurrentLanguage);    
    }
    
    [ContextMenu("InitDropdownItem")]
    public void InitDropdownItem()
    {
        dropdownItem.ClearOptions();
        dropdownItem.AddOptions(new List<string>(Languages));
        dropdownItem.value = System.Array.IndexOf(Languages, CurrentLanguage);    
    }
}

using UnityEngine;
using YG;
using System;

public class YandexPlatformSDK : MonoBehaviour, IPlatformSDK
{
    public void Initialize()
    {
        // YandexSDK уже инициализируется через свой плагин
        Debug.Log("Yandex SDK initialized");
    }

    public void ShowInterstitial()
    {
        YandexGame.FullscreenShow();
    }

    public void ShowRewarded()
    {
        // Используем существующий метод показа рекламы из YandexSDK
    }

    public void SaveProgress(string key, string value)
    {
        // Используем существующий метод сохранения из YandexSDK
        YandexGame.savesData.savegame = value;
        YandexGame.SaveLocal();
    }

    public void LoadProgress(string key, Action<string> onComplete)
    {
        string data = YandexGame.savesData.savegame;
        onComplete?.Invoke(data);
    }
}

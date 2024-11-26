using Unity.VisualScripting;
using UnityEngine;
using System;

public class PlatformSDKManager : MonoBehaviour
{
    [SerializeField] private PlatformType platformType;
    private IPlatformSDK currentSDK;

    public enum PlatformType
    {
        VK,
        Yandex
    }

    public void InitializeSDK()
    {
        if (platformType == PlatformType.VK)
        {
            currentSDK = transform.GetComponentInChildren<VKPlatformSDK>();
        }
        else
        {
            currentSDK = transform.GetComponentInChildren<YandexPlatformSDK>();
        }

        if (currentSDK != null)
        {
            currentSDK.Initialize();
        }
        else
        {
            Debug.LogWarning("No platform SDK initialized!!!!!!!!!!!!");
        }
    }

    public void FullscreenShow()
    {
        if (currentSDK != null)
        {
            currentSDK.ShowInterstitial();
        }
    }

    public void ShowRewarded()
    {
        if (currentSDK != null)
        {
            currentSDK.ShowRewarded();
        }
    }

    public void SaveProgress(string key, string value)
    {
        if (currentSDK != null)
        {
            currentSDK.SaveProgress(key, value);
        }
    }

    public void LoadProgress(string key, Action<string> onComplete)
    {
        if (currentSDK != null)
        {
            currentSDK.LoadProgress(key, onComplete);
        }
        else
        {
            onComplete?.Invoke("");
        }
    }
}

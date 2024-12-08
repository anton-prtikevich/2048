using UnityEngine;
using System.Runtime.InteropServices;
using System;
using TMPro;

// Цепочка вызовов выглядит так: Unity C# (VKPlatformSDK.cs) -> VKPlugins.jslib -> vksdk.js -> vkbridge.js -> VK API

[Serializable]
public class StorageData
{
    public string key;
    public string value;
}

public class VKPlatformSDK : MonoBehaviour, IPlatformSDK
{
    [SerializeField] private TextMeshProUGUI debugtext;
    [SerializeField] private bool useLocalStorage = false;
    private Action<string> currentLoadCallback;

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void showVKAd();

    [DllImport("__Internal")]
    private static extern void showVKRewardedAd();

    [DllImport("__Internal")]
    private static extern void vkSaveData(string key, string value);

    [DllImport("__Internal")]
    private static extern void vkLoadData(string key);

    [DllImport("__Internal")]
    private static extern void setStorageType(bool useLocal);

    #endif

    public void Initialize()
    {
        Debug.Log("Initializing VK SDK");
        Debug.Log($"Using {(useLocalStorage ? "local" : "cloud")} storage");

        #if UNITY_WEBGL && !UNITY_EDITOR
        setStorageType(useLocalStorage);
        #endif
    }

    public void FullscreenShow()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            showVKAd();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error showing interstitial ad: {e.Message}");
        }
        #else
        Debug.Log("VK showing interstitial ad (editor/non-WebGL)");
        #endif
    }

    public void ShowRewarded()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            showVKRewardedAd();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error showing rewarded ad: {e.Message}");
        }
        #else
        Debug.Log("VK showing rewarded ad (editor/non-WebGL)");
        #endif
    }

    public void SaveProgress(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Cannot save with empty key");
            return;
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            vkSaveData(key, value ?? "");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving data: {e.Message}");
        }
        #else
        PlayerPrefs.SetString($"VK_{key}", value ?? "");
        PlayerPrefs.Save();
        Debug.Log($"VK saving progress: {key} = {value} (editor/non-WebGL)");
        #endif
    }

    public void LoadProgress(string key, Action<string> onComplete)
    {
        currentLoadCallback = onComplete;
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Cannot load with empty key");
            onComplete?.Invoke("");
            return;
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            vkLoadData(key);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
            onComplete?.Invoke("");
        }
        #else
        string value = PlayerPrefs.GetString($"VK_{key}", "");
        // Debug.Log($"VK loading progress: {key} = {value} (editor/non-WebGL)");
        currentLoadCallback?.Invoke(value);
        #endif
    }

    public void JSOnLoadComplete(string jsonData)
    {
        try 
        {
            // Парсим структуру данных VK
            StorageData storageData = JsonUtility.FromJson<StorageData>(jsonData);
            
            // Извлекаем actual данные из поля value
            string actualData = storageData?.value ?? jsonData;
            
            Debug.Log($"VK load completed: {actualData}");
            debugtext.text += actualData;
            
            // Возвращаем только actual данные в GameManager
            currentLoadCallback?.Invoke(actualData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing VK storage data: {e.Message}");
            currentLoadCallback?.Invoke(jsonData); // В случае ошибки возвращаем исходные данные
        }
        finally 
        {
            currentLoadCallback = null;
        }
    }

    public void JSOnLoadError(string jsonData)
    {
        Debug.Log($"VK Load Error !!! Не получилось загрузить данные");
        debugtext.text += jsonData;
        currentLoadCallback?.Invoke(jsonData);
        currentLoadCallback = null;
    }

    public void OnSaveComplete(string jsonData)
    {
        Debug.Log($"Сохранения произошло");
    }
}

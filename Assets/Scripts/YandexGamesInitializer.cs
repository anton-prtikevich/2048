using UnityEngine;

public class YandexGamesInitializer : MonoBehaviour
{
    private static YandexGamesInitializer instance;

    #if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void ShowFullscreenAd();
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void ShowRewardedAd(string placement);
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SaveExtern(string data);
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void LoadExtern();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SendMessage(string gameObjectName, string methodName, string parameter);
    #endif

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            LoadExtern();
        #endif
    }

    public void ShowInterstitial()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            ShowFullscreenAd();
        #endif
    }

    public void ShowRewarded(string placement)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            ShowRewardedAd(placement);
        #endif
    }

    public void SaveProgress(string data)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            SaveExtern(data);
        #endif
    }

    // Методы для вызова из JavaScript
    [System.Obsolete]
    public void OnGameLoaded(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            PlayerPrefs.SetString("SavedData", data);
            PlayerPrefs.Save();
            
            GameManager.Instance.LoadProgress();
        }
    }

    [System.Obsolete]
    public void OnRewardedAdClosed(bool rewarded)
    {
        if (rewarded)
        {
            GameManager.Instance.GiveReward();
        }
    }
}

public interface IPlatformSDK
{
    void Initialize();
    void ShowInterstitial();
    void ShowRewarded();
    void SaveProgress(string key, string value);
    void LoadProgress(string key, System.Action<string> onComplete);
}

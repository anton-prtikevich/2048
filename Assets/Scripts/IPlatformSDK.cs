public interface IPlatformSDK
{
    void Initialize();
    void FullscreenShow();
    void ShowRewarded();
    void SaveProgress(string key, string value);
    void LoadProgress(string key, System.Action<string> onComplete);
}

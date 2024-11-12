using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

[Serializable]
public class GameSaveData
{
    public int bestScore;
    public int currentScore;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueWithAdButton;

    [Header("Game Settings")]
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private YandexGamesInitializer yandexGames;
    [SerializeField] private int movesBeforeAd = 30;
    [SerializeField] private float scoreAnimationDuration = 0.5f;

    private int currentScore;
    private int bestScore;
    private bool isGameOver;
    private int movesSinceLastAd;
    private const string BestScoreKey = "BestScore";
    private bool isWaitingForRewardedAd;
    private Sequence scoreAnimationSequence;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Снимаем ограничение FPS
        Application.targetFrameRate = 90; // -1 означает без ограничений
        QualitySettings.vSyncCount = 0; // Отключаем вертикальную синхронизацию
        
        LoadBestScore();
        InitializeUI();
    }

    private void Start()
    {
        if (continueWithAdButton != null)
        {
            continueWithAdButton.onClick.AddListener(ShowRewardedAd);
        }
        
        UpdateScoreUI(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        movesSinceLastAd = 0;
    }

    private void InitializeUI()
    {
        if (scoreText) scoreText.text = "0";
        if (bestScoreText) bestScoreText.text = bestScore.ToString();
    }

    public void AddScore(int points)
    {
        int oldScore = currentScore;
        currentScore += points;
        
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            SaveBestScore();
        }
        
        UpdateScoreUI(true, oldScore);
        
        movesSinceLastAd++;
        if (movesSinceLastAd >= movesBeforeAd)
        {
            ShowInterstitialAd();
            movesSinceLastAd = 0;
        }
    }

    private void UpdateScoreUI(bool animate = false, int oldScore = 0)
    {
        if (scoreAnimationSequence != null)
        {
            scoreAnimationSequence.Kill();
        }

        if (animate && scoreText != null)
        {
            scoreAnimationSequence = DOTween.Sequence();
            
            // Анимация увеличения
            scoreAnimationSequence.Append(scoreText.transform.DOScale(1.2f, scoreAnimationDuration * 0.5f));
            
            // Анимация счета
            float startScore = oldScore;
            scoreAnimationSequence.Join(
                DOTween.To(() => startScore, x => {
                    startScore = x;
                    scoreText.text = Mathf.FloorToInt(x).ToString();
                }, currentScore, scoreAnimationDuration)
                .SetEase(Ease.OutQuad)
            );
            
            // Возвращение к нормальному размеру
            scoreAnimationSequence.Append(scoreText.transform.DOScale(1f, scoreAnimationDuration * 0.5f));
        }
        else
        {
            if (scoreText) scoreText.text = currentScore.ToString();
        }

        if (bestScoreText) bestScoreText.text = bestScore.ToString();
    }

    //вызывается кнопкой
    [ContextMenu("GameOver")]
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.localScale = Vector3.zero;
        gameOverPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        
        SaveProgress();
    }

    //вызывается кнопкой
    public void RestartGame()
    {
        isGameOver = false;
        currentScore = 0;
        UpdateScoreUI(false);
        gameOverPanel.SetActive(false);
        gameBoard.RestartGame();
        movesSinceLastAd = 0;

        ResumeGame();
    }

    //вызывается кнопкой
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        pausePanel.transform.localScale = Vector3.zero;
        pausePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    //вызывается кнопкой
    public void ResumeGame()
    {
        pausePanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => {
                pausePanel.SetActive(false);
                Time.timeScale = 1f;
            });
    }

    private void ShowInterstitialAd()
    {
        #if !UNITY_EDITOR
        if (yandexGames != null)
        {
            yandexGames.ShowInterstitial();
        }
        #endif
    }

    private void ShowRewardedAd()
    {
        #if !UNITY_EDITOR
        if (yandexGames != null)
        {
            isWaitingForRewardedAd = true;
            yandexGames.ShowRewarded("continue");
        }
        #endif
    }

    public void GiveReward()
    {
        if (isWaitingForRewardedAd)
        {
            isWaitingForRewardedAd = false;
            isGameOver = false;
            gameOverPanel.SetActive(false);
        }
    }

    private void LoadBestScore()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    private void SaveBestScore()
    {
        PlayerPrefs.SetInt(BestScoreKey, bestScore);
        PlayerPrefs.Save();
        SaveProgress();
    }

    public void SaveProgress()
    {
        #if !UNITY_EDITOR
        if (yandexGames != null)
        {
            GameSaveData saveData = new GameSaveData
            {
                bestScore = bestScore,
                currentScore = currentScore
            };
            string json = JsonUtility.ToJson(saveData);
            yandexGames.SaveProgress(json);
        }
        #endif
    }

    public void LoadProgress()
    {
        string savedData = PlayerPrefs.GetString("SavedData", "");
        if (!string.IsNullOrEmpty(savedData))
        {
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(savedData);
            bestScore = saveData.bestScore;
            currentScore = saveData.currentScore;
            UpdateScoreUI(false);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveProgress();
        }
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

    private void OnDestroy()
    {
        if (scoreAnimationSequence != null)
        {
            scoreAnimationSequence.Kill();
        }
    }

    public void ExitGame()
    {
        SaveProgress();
        Application.Quit();
    }

    [Tooltip("Как часто обновлять FPS")]
    [SerializeField] [Range(0.1f, 1f)] private float _updateInterval = 0.5f;

    [Tooltip("Текстовое поле для отображения FPS")]
    [SerializeField] private TMP_Text _fpsText;

    [Tooltip("Текстовое поле для отображения среднего FPS")]
    [SerializeField] private TMP_Text _averageFPSText;
    private float _timeleft;
    private float _currentFPS;
    private float _averageFPS;
    private void Update()
    {
        _timeleft -= Time.deltaTime;

        if (_timeleft <= 0.0)
        {
            _currentFPS = 1.0f / Time.smoothDeltaTime;
            _averageFPS = Time.frameCount / Time.time;
            _timeleft = _updateInterval;

            _fpsText.text = $"FPS: {_currentFPS:0.}";
            _averageFPSText.text = $"Средний FPS: {_averageFPS:0.}";
        }
    }
}

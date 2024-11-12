using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI valueText;
    
    private int value;
    public int Value => value;

    private static readonly Color[] tileColors = new Color[]
    {
        new Color(0.93f, 0.89f, 0.85f), // 2
        new Color(0.93f, 0.88f, 0.78f), // 4
        new Color(0.95f, 0.69f, 0.47f), // 8
        new Color(0.96f, 0.58f, 0.39f), // 16
        new Color(0.96f, 0.49f, 0.37f), // 32
        new Color(0.96f, 0.37f, 0.23f), // 64
        new Color(0.93f, 0.81f, 0.45f), // 128
        new Color(0.93f, 0.80f, 0.38f), // 256
        new Color(0.93f, 0.79f, 0.31f), // 512
        new Color(0.93f, 0.78f, 0.24f), // 1024
        new Color(0.93f, 0.77f, 0.17f)  // 2048
    };

    private static readonly Color textColorLow = new Color(0.47f, 0.43f, 0.39f); // Для значений 2 и 4
    private static readonly Color textColorHigh = Color.white; // Для значений 8 и выше

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateTileAppearance();
    }

    private void UpdateTileAppearance()
    {
        valueText.text = value.ToString();
        
        int colorIndex = Mathf.Min(Mathf.FloorToInt(Mathf.Log(value, 2)) - 1, tileColors.Length - 1);
        background.color = tileColors[colorIndex];

        // Меняем цвет текста в зависимости от значения
        valueText.color = value <= 4 ? textColorLow : textColorHigh;

        // Адаптируем размер текста в зависимости от количества цифр
        if (value < 100)
            valueText.fontSize = 72;
        else if (value < 1000)
            valueText.fontSize = 64;
        else
            valueText.fontSize = 52;

        // Анимация изменения значения
        transform.DOScale(1.2f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                transform.DOScale(1f, 0.1f)
                    .SetEase(Ease.InQuad);
            });
    }

    public void MergeTo(Vector2 position, Tile other)
    {
        GetComponent<RectTransform>().DOAnchorPos(position, 0.15f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                other.SetValue(value * 2);
                Destroy(gameObject);
            });
    }

    public void MoveTo(Vector2 position)
    {
        GetComponent<RectTransform>().DOAnchorPos(position, 0.15f)
            .SetEase(Ease.InQuad);
    }
}

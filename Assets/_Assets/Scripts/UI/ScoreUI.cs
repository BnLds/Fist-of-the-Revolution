using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        ScoreManager.Instance.OnNewScore.AddListener(UpdateUI);
        UpdateUI(ScoreManager.Instance.GetCurrentScore());
    }

    private void UpdateUI(int score)
    {
        _scoreText.text = Localizer.Instance.GetMessage(LocalizationKeys.SCORE_KEY) + ": " + score.ToString();
    }
}

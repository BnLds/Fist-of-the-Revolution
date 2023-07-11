using UnityEngine;
using TMPro;

public class HighscoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leaderboardText;
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _nameText;

    private void Awake()
    {
        Hide();
    }

    public void Show(string playerName)
    {
        SetStrings();
        gameObject.SetActive(true);
        Debug.Log(playerName);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetStrings()
    {
        _leaderboardText.text = Localizer.Instance.GetMessage(LocalizationKeys.HIGHSCORES_KEY);
        _rankText.text = Localizer.Instance.GetMessage(LocalizationKeys.RANK_KEY);
        _scoreText.text = Localizer.Instance.GetMessage(LocalizationKeys.SCORE_KEY);
        _nameText.text = Localizer.Instance.GetMessage(LocalizationKeys.NAME_KEY);
    }
}

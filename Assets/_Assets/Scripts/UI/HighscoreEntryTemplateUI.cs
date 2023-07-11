using UnityEngine;
using TMPro;

public class HighscoreEntryTemplateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void SetEntryData(int rank, int score, string name)
    {
        _rankText.text = rank.ToString();
        _scoreText.text = score.ToString();
        _nameText.text = name;
    }
}

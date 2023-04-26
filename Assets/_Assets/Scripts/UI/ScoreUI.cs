using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private BreakablesCollectionManager _breakablesCollectionManager;

    private int _scoreDisplayed;

    private void Awake()
    {
        _breakablesCollectionManager.OnScoreChange.AddListener(UpdateScore);

        _scoreDisplayed = 0;
        UpdateUI();
    }

    private void UpdateScore(int scoreChange)
    {
        _scoreDisplayed += scoreChange;
        UpdateUI();
    }

    private void UpdateUI()
    {
        _scoreText.text = "Score: " + _scoreDisplayed.ToString();
    }
}

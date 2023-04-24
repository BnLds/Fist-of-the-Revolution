using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private BreakablesCollectionManager breakablesCollectionManager;

    private int scoreDisplayed;

    private void Awake()
    {
        breakablesCollectionManager.OnScoreChange.AddListener(UpdateScore);

        scoreDisplayed = 0;
        UpdateUI();
    }

    private void UpdateScore(int scoreChange)
    {
        scoreDisplayed += scoreChange;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + scoreDisplayed.ToString();
    }
}

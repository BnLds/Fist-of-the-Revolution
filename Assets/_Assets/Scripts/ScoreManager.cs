using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [HideInInspector] public UnityEvent<int> OnNewScore;

    private int _currentScore;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _currentScore = 0;
    }

    private void Start()
    {
        BreakablesCollectionManager.Instance.OnScoreChange.AddListener(UpdateScore);
    }

    private void UpdateScore(int scoreChange)
    {
        _currentScore += scoreChange;
        OnNewScore?.Invoke(_currentScore);
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }
}

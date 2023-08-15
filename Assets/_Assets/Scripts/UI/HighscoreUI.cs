using UnityEngine;
using TMPro;

public class HighscoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leaderboardText;
    [SerializeField] private Transform _columnsContainer;
    [SerializeField] private LeaderboardColumnTemplateUI _columnTemplateUI;
    [SerializeField] private HighscoreEntryTemplateUI _highscoreEntryTemplateUI;

    private int _leaderboardEntriesCount;
    private LeaderboardColumnTemplateUI _column2;
    private Localizer _localizer;


    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        _localizer = Localizer.Instance;

        foreach(Transform child in _columnsContainer)
        {
            if(child != _columnTemplateUI.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void AddNewPlayerData(string playerName)
    {
        Leaderboard.Record(playerName, ScoreManager.Instance.GetCurrentScore());
        _leaderboardEntriesCount = Leaderboard.GetEntriesCount();

        if(_leaderboardEntriesCount > 10)
        {
            _column2 = Instantiate(_columnTemplateUI.transform, _columnsContainer).GetComponent<LeaderboardColumnTemplateUI>();
            _column2.transform.SetAsLastSibling();
        }

        Leaderboard.ScoreEntry entry;
        for (int i = 0; i < _leaderboardEntriesCount; i++)
        {
            entry = Leaderboard.GetEntry(i);
            if (i < 10)
            {
                _columnTemplateUI.CreateNewEntry(i+1, entry.Score, entry.Name);
            }
            else
            {
                _column2.CreateNewEntry(i+1, entry.Score, entry.Name);
            }
        }
    }

    public void Show(string playerName)
    {
        LocalizeTexts();
        AddNewPlayerData(playerName);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LocalizeTexts()
    {
        _leaderboardText.text = _localizer.GetMessage(LocalizationKeys.HIGHSCORES_KEY);
    }
}

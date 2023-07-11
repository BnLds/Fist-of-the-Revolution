using UnityEngine;
using TMPro;

public class LeaderboardColumnTemplateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankTitleText;
    [SerializeField] private TextMeshProUGUI _scoreTitleText;
    [SerializeField] private TextMeshProUGUI _nameTitleText;
    [SerializeField] private HighscoreEntryTemplateUI _entryTemplateUI;
    [SerializeField] private Transform _entriesContainer;

    private int _numberOfEntries = 1;

    private void Awake()
    {
        foreach(Transform child in _entriesContainer)
        {
            if(child != _entryTemplateUI.transform)
            {
                Destroy(child.gameObject);
            }
        }

        LocalizeColumnTitles();
    }

    private void LocalizeColumnTitles()
    {
        _rankTitleText.text = Localizer.Instance.GetMessage(LocalizationKeys.RANK_KEY);
        _scoreTitleText.text = Localizer.Instance.GetMessage(LocalizationKeys.SCORE_KEY);
        _nameTitleText.text = Localizer.Instance.GetMessage(LocalizationKeys.NAME_KEY);
    }

    public void CreateNewEntry(int rank, int score, string name)
    {
        if(_numberOfEntries == 1)
        {
            _entryTemplateUI.SetEntryData(rank, score, name);
        }
        else
        {
            var newEntry = Instantiate(_entryTemplateUI.transform, _entriesContainer);
            newEntry.SetAsLastSibling();
            newEntry.GetComponent<HighscoreEntryTemplateUI>().SetEntryData(rank, score, name);
        }

        _numberOfEntries++;
    }
}

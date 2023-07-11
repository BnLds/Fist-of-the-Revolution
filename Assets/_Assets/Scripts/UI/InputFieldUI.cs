using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputFieldUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TextMeshProUGUI _nameInputFieldText;
    [SerializeField] private Button _goButton;
    [SerializeField] private TextMeshProUGUI _goText;
    [SerializeField] private HighscoreUI _highscoreUI;

    private void Start()
    {
        _goButton.interactable = false;
        _nameInputField.onValueChanged.AddListener((string str) =>
        {
            _goButton.interactable = true;
        });
        _nameInputField.onEndEdit.AddListener(DisplayLeaderboard);
        _goButton.onClick.AddListener(() =>
        {
            DisplayLeaderboard(_nameInputField.text);
        });
        Hide();
    }

    private void DisplayLeaderboard(string playerName)
    {
        _highscoreUI.Show(playerName);
        Hide();
    }

    private void SetStrings()
    {
        _nameInputFieldText.text = Localizer.Instance.GetMessage(LocalizationKeys.ENTER_NAME_KEY);
        _goText.text = Localizer.Instance.GetMessage(LocalizationKeys.GO_KEY);
    }

    public void Show()
    {
        SetStrings();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
}

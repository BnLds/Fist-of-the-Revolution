using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GamePauseUI : MonoBehaviour
{
    private const string MAIN_MENU_KEY = "ui_mainMenu";
    private const string RESUME_KEY = "ui_resume";
    private const string PAUSED_KEY = "ui_paused";

    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _resumeText;
    [SerializeField] private TextMeshProUGUI _mainMenuText;
    [SerializeField] private TextMeshProUGUI _pauseText;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(() => {
            _GameManager.Instance.TogglePauseGame();
        });
        _resumeButton.onClick.AddListener(()=> {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        _GameManager.Instance.OnGamePaused.AddListener(Show);
        _GameManager.Instance.OnGameUnpaused.AddListener(Hide);
        Localizer.Instance.LocalizationLoaded.AddListener(SetStrings);

        Hide();
    }

    private void SetStrings()
    {
        _resumeText.text = Localizer.Instance.GetMessage(RESUME_KEY);
        _mainMenuText.text = Localizer.Instance.GetMessage(MAIN_MENU_KEY);
        _pauseText.text = Localizer.Instance.GetMessage(PAUSED_KEY);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}

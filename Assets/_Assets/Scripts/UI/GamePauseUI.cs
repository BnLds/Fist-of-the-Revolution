using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private TextMeshProUGUI _optionsText;
    [SerializeField] private TextMeshProUGUI _resumeText;
    [SerializeField] private TextMeshProUGUI _mainMenuText;
    [SerializeField] private TextMeshProUGUI _pauseText;

    [SerializeField] private OptionsUI _optionsUI;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            _GameManager.Instance.TogglePauseGame();
        });
        _mainMenuButton.onClick.AddListener(()=> 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        _optionsButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            _optionsUI.Show();
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
        _resumeText.text = Localizer.Instance.GetMessage(LocalizationKeys.RESUME_KEY);
        _mainMenuText.text = Localizer.Instance.GetMessage(LocalizationKeys.MAIN_MENU_KEY);
        _pauseText.text = Localizer.Instance.GetMessage(LocalizationKeys.PAUSED_KEY);
        _optionsText.text = Localizer.Instance.GetMessage(LocalizationKeys.OPTIONS_KEY);
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

using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _playText;
    [SerializeField] private Button _quitButton;
    [SerializeField] private TextMeshProUGUI _quitText;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private TextMeshProUGUI _optionsText;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private TextMeshProUGUI _creditsText;
    [SerializeField] private OptionsUI _optionsUI;
    [SerializeField] private CreditsUI _creditsUI;

    private Localizer _localizer;

    private void Awake()
    {
        _playButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Loader.Load(Loader.Scene.GameScene);
        });

        _quitButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Application.Quit();

        });

        _optionsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayButtonClickSound();
            _optionsUI.Show();
        });

        _creditsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayButtonClickSound();
            _creditsUI.Show();
        });

        Time.timeScale = 1f;
    }

    private void Start()
    {
        _localizer = Localizer.Instance;

        _localizer.LocalizationLoaded.AddListener(() => {
            LocalizeStrings();
        });

        _optionsUI.OnReturnToMainMenu.AddListener(() =>
        {
            LocalizeStrings();
        });
    }

    private void OnDisable()
    {
        _optionsUI.OnReturnToMainMenu.RemoveAllListeners();
    }

    private void LocalizeStrings()
    {
        _optionsText.text = _localizer.GetMessage(LocalizationKeys.OPTIONS_KEY);
        _quitText.text = _localizer.GetMessage(LocalizationKeys.QUIT_KEY);
        _playText.text = _localizer.GetMessage(LocalizationKeys.PLAY_KEY);
        _creditsText.text = _localizer.GetMessage(LocalizationKeys.CREDITS_KEY);
    }
}
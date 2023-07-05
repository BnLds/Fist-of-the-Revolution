using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OptionsUI : MonoBehaviour
{
    private const string OPTIONS_KEY = "ui_options";
    private const string SOUND_EFFECTS_KEY = "ui_soundEffects";
    private const string MUSIC_KEY = "ui_music";
    private const string CLOSE_KEY = "ui_close";
    private const string LANGUAGE_KEY = "ui_language";
    private const string ENGLISH_KEY = "ui_english";
    private const string FRENCH_KEY = "ui_french";
    private const string CHINESE_KEY = "ui_chinese";
    private const string GERMAN_KEY = "ui_german";
    private const string JAPANESE_KEY = "ui_japanese";
    private const string KOREAN_KEY = "ui_korean";
    private const string SPANISH_KEY = "ui_spanish";

    [SerializeField] private LocalizationManager _localizationManager;
    [SerializeField] private Button _soundEffectsButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private TextMeshProUGUI _soundEffectsText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _closeText;
    [SerializeField] private Button _languageButton;
    [SerializeField] private TextMeshProUGUI _languageText;
    [SerializeField] private TextMeshProUGUI _musicText;
    [SerializeField] private TextMeshProUGUI _optionsText;

    private Dictionary<int, string> _languagesDict;

    private void Awake()
    {
        _languagesDict = new() {
            {(int)LocalizationManager.Languages.Chinese, CHINESE_KEY},
            {(int)LocalizationManager.Languages.English, ENGLISH_KEY},
            {(int)LocalizationManager.Languages.French, FRENCH_KEY},
            {(int)LocalizationManager.Languages.German, GERMAN_KEY},
            {(int)LocalizationManager.Languages.Japanese, JAPANESE_KEY},
            {(int)LocalizationManager.Languages.Korean, KOREAN_KEY},
            {(int)LocalizationManager.Languages.Spanish, SPANISH_KEY}
        };
        

            _soundEffectsButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.ChangeVolume();
                UpdateVisual();
            });
        _musicButton.onClick.AddListener(()=>{
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        _closeButton.onClick.AddListener(() => {
            Hide();
        });
        _languageButton.onClick.AddListener(() =>
        {
            _localizationManager.SelectNextLanguage();
            UpdateVisual();
        });
    }

    private void Start()
    {
        _GameManager.Instance.OnGameUnpaused.AddListener(GameManager_OnGameUnpaused);

        Localizer.Instance.LocalizationLoaded.AddListener(() => {
            SetStrings();
            UpdateVisual();
        });

        Hide();
    }

    private void GameManager_OnGameUnpaused()
    {
        Hide();
    }

    private void UpdateVisual()
    {
        _soundEffectsText.text = Localizer.Instance.GetMessage(SOUND_EFFECTS_KEY) + ": " + Mathf.Round(SoundManager.Instance.GetVolume()* 10f);
        _musicText.text = Localizer.Instance.GetMessage(MUSIC_KEY) + ": " + Mathf.Round(MusicManager.Instance.GetVolume()* 10f);
        _languageText.text = Localizer.Instance.GetMessage(LANGUAGE_KEY) + ": " + Localizer.Instance.GetMessage(_languagesDict[_localizationManager.GetCurrentLanguageIndex()]);
    }

    private void SetStrings()
    {
        _optionsText.text = Localizer.Instance.GetMessage(OPTIONS_KEY);
        _closeText.text = Localizer.Instance.GetMessage(CLOSE_KEY);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

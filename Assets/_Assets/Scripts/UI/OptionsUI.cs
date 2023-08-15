using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class OptionsUI : MonoBehaviour
{
    private const string PLAYER_PREFS_FOOTSTEPS_SOUND = "FootstepsSound";

    [HideInInspector] public UnityEvent OnToggleFootstepsSound;
    [HideInInspector] public UnityEvent OnReturnToMainMenu;


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
    [SerializeField] private Button _footstepsButton;
    [SerializeField] private TextMeshProUGUI _footstepsText;

    private Dictionary<int, string> _languagesDict;
    private bool _isFootstepsOn = true;
    private Localizer _localizer;

    private void Awake()
    {
        if(PlayerPrefs.HasKey(PLAYER_PREFS_FOOTSTEPS_SOUND))
        {
            _isFootstepsOn = (PlayerPrefs.GetInt(PLAYER_PREFS_FOOTSTEPS_SOUND) != 0);
        }

        _languagesDict = new() {
            {(int)LocalizationManager.Languages.Chinese, LocalizationKeys.CHINESE_KEY},
            {(int)LocalizationManager.Languages.English, LocalizationKeys.ENGLISH_KEY},
            {(int)LocalizationManager.Languages.French, LocalizationKeys.FRENCH_KEY},
            {(int)LocalizationManager.Languages.German, LocalizationKeys.GERMAN_KEY},
            {(int)LocalizationManager.Languages.Japanese, LocalizationKeys.JAPANESE_KEY},
            {(int)LocalizationManager.Languages.Korean, LocalizationKeys.KOREAN_KEY},
            {(int)LocalizationManager.Languages.Spanish, LocalizationKeys.SPANISH_KEY}
        };
        
        _soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            SoundManager.Instance.PlayButtonClickSound();
            UpdateVisual();
        });

        _musicButton.onClick.AddListener(()=>
        {
            MusicManager.Instance.ChangeVolume();
            SoundManager.Instance.PlayButtonClickSound();
            UpdateVisual();
        });

        _closeButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Hide();
        });

        _languageButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayButtonClickSound();
            _localizationManager.SelectNextLanguage();
        });

        _footstepsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayButtonClickSound();
            _isFootstepsOn = !_isFootstepsOn;
            UpdateVisual();
            OnToggleFootstepsSound?.Invoke();
            PlayerPrefs.SetInt(PLAYER_PREFS_FOOTSTEPS_SOUND, _isFootstepsOn ? 1 : 0);
        });
    }

    private void Start()
    {
        _localizer = Localizer.Instance;
        _GameManager.Instance.OnGameUnpaused.AddListener(GameManager_OnGameUnpaused);

        _localizer.LocalizationLoaded.AddListener(() => {
            LocalizeStrings();
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
        _soundEffectsText.text = _localizer.GetMessage(LocalizationKeys.SOUND_EFFECTS_KEY) + ": " + Mathf.Round(SoundManager.Instance.GetVolume()* 10f);
        _musicText.text = _localizer.GetMessage(LocalizationKeys.MUSIC_KEY) + ": " + Mathf.Round(MusicManager.Instance.GetVolume()* 10f);
        _languageText.text = _localizer.GetMessage(LocalizationKeys.LANGUAGE_KEY) + ": " + _localizer.GetMessage(_languagesDict[_localizationManager.GetCurrentLanguageIndex()]);
        
        if(_isFootstepsOn)
        {
            _footstepsText.text = _localizer.GetMessage(LocalizationKeys.FOOTSTEPS_KEY) + " " + _localizer.GetMessage(LocalizationKeys.ON_KEY);
        }
        else
        {
            _footstepsText.text = _localizer.GetMessage(LocalizationKeys.FOOTSTEPS_KEY) + " " + _localizer.GetMessage(LocalizationKeys.OFF_KEY);
        }
    }

    private void LocalizeStrings()
    {
        _optionsText.text = _localizer.GetMessage(LocalizationKeys.OPTIONS_KEY);
        _closeText.text = _localizer.GetMessage(LocalizationKeys.CLOSE_KEY);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        OnReturnToMainMenu?.Invoke();
        gameObject.SetActive(false);
    }

    public bool IsFootstepsOn()
    {
        return _isFootstepsOn;
    }
}

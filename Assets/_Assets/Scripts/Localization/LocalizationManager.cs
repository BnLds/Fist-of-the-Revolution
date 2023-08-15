using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    public enum Languages
    {
        English,
        French,
        Spanish,
        German,
        Chinese,
        Japanese,
        Korean
    }

    private const string LOCALIZATION_TABLE = "MyLocalizationTable";
    private const string PLAYER_PREF_LANGUAGE_KEY = "selectedLanguage";
    [HideInInspector] public UnityEvent OnLocalTableLoaded;
    [HideInInspector] public UnityEvent OnLanguageChanged;

    private int _currentLanguageIndex = 0;
    private LocalizationSettings _localizationSettings;

    private void Start()
    {
        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        yield return LocalizationSettings.InitializationOperation;
        _localizationSettings = LocalizationSettings.Instance;
        if(PlayerPrefs.HasKey(PLAYER_PREF_LANGUAGE_KEY))
        {
            int languageIndex = PlayerPrefs.GetInt(PLAYER_PREF_LANGUAGE_KEY);
            SetPlayerLanguagePref(languageIndex);
        }
        else
        {
            SetPlayerLanguagePref(_currentLanguageIndex);
        }
    }

    private IEnumerator InitLocaleTable()
    {
        yield return LocalizationSettings.InitializationOperation;
        _localizationSettings = LocalizationSettings.Instance;
        OnLanguageChanged.Invoke();
    }

    public string UpdateLocalizedString(string key)
    {
        var op = _localizationSettings.GetStringDatabase().GetLocalizedStringAsync(LOCALIZATION_TABLE, key);
        return op.Result;
    }

    private void SetPlayerLanguagePref(int languageIndex)
    {
        _currentLanguageIndex = languageIndex;

        _localizationSettings.SetSelectedLocale(LocalizationSettings.AvailableLocales.Locales[languageIndex]);
        PlayerPrefs.SetInt(PLAYER_PREF_LANGUAGE_KEY, languageIndex);
        PlayerPrefs.Save();
        StartCoroutine(InitLocaleTable());
    }

    public void SelectNextLanguage()
    {
        int newIndex = (_currentLanguageIndex+1) % LocalizationSettings.AvailableLocales.Locales.Count;
        SetPlayerLanguagePref(newIndex);
    }

    public int GetCurrentLanguageIndex()
    {
        return _currentLanguageIndex;
    }
}

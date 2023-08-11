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

    private void Start()
    {
        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        yield return LocalizationSettings.InitializationOperation;
        if(PlayerPrefs.HasKey(PLAYER_PREF_LANGUAGE_KEY))
        {
            int languageIndex = PlayerPrefs.GetInt(PLAYER_PREF_LANGUAGE_KEY);
            ChangeLanguage(languageIndex);
        }

        OnLocalTableLoaded?.Invoke();
    }

    private IEnumerator InitNewTable()
    {
        yield return LocalizationSettings.InitializationOperation;
        OnLanguageChanged?.Invoke();
    }

    public string InitializeLocalizedString(string key)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LOCALIZATION_TABLE, key);
        return op.Result;
    }

    public string UpdateLocalizedString(string key)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LOCALIZATION_TABLE, key);
        return op.Result;
    }

    private void ChangeLanguage(int languageIndex)
    {
        LoadPlayerLanguagePref(languageIndex);
        _currentLanguageIndex = languageIndex;
    }

    private void LoadPlayerLanguagePref(int languageIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
        PlayerPrefs.SetInt(PLAYER_PREF_LANGUAGE_KEY, languageIndex);
        PlayerPrefs.Save();
        StartCoroutine(InitNewTable());
    }

    public void SelectNextLanguage()
    {
        int newIndex = (_currentLanguageIndex+1) % LocalizationSettings.AvailableLocales.Locales.Count;
        ChangeLanguage(newIndex);
    }

    public int GetCurrentLanguageIndex()
    {
        return _currentLanguageIndex;
    }
}

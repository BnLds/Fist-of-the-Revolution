using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private const string LOCALIZATION_TABLE = "MyLocalizationTable";
    private const string PLAYER_PREF_LANGUAGE_KEY = "selectedLanguage";
    [HideInInspector] public UnityEvent OnLocalTableLoaded;
    [HideInInspector] public UnityEvent OnLanguageChanged;

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

    public string InitializeLocalizedString(string key)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LOCALIZATION_TABLE, key);
        return op.Result;
    }

    public string UpdateLocalizedString(string key)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedString(LOCALIZATION_TABLE, key, LocalizationSettings.SelectedLocale);
        return op;
    }

    public void ChangeLanguage(int languageIndex)
    {
        LoadPlayerLanguagePref(languageIndex);
        OnLanguageChanged?.Invoke();
    }

    private void LoadPlayerLanguagePref(int languageIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
        PlayerPrefs.SetInt(PLAYER_PREF_LANGUAGE_KEY, languageIndex);
        PlayerPrefs.Save();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private const string LOCALIZATION_TABLE = "MyLocalizationTable";
    [HideInInspector] public UnityEvent OnLocalTableLoaded;

    private void Start()
    {
        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        yield return LocalizationSettings.InitializationOperation;
        OnLocalTableLoaded?.Invoke();
    }

    public string GetLocalizedString(string key)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LOCALIZATION_TABLE, key);
        return op.Result;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
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
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("LocalizationTable", key);
        return op.Result;
    }
}

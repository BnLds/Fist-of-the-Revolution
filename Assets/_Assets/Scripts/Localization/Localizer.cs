using UnityEngine;
using UnityEngine.Events;

public class Localizer : MonoBehaviour
{
    public static Localizer Instance {get; private set;}

    [HideInInspector] public UnityEvent LocalizationLoaded;

    [SerializeField] private LocalizationManager _localizationManager;
    [SerializeField] private LocalTablesListSO _localTablesSO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _localizationManager.OnLanguageChanged.AddListener(UpdateMessages);
    }

    private void UpdateMessages()
    {
        for (int i = 0; i < _localTablesSO._localTablesList.Count; i++)
        {
            _localTablesSO._localTablesList[i].Message = _localizationManager.UpdateLocalizedString(_localTablesSO._localTablesList[i].LocalizationKey);
        }

        LocalizationLoaded?.Invoke();
    }

    public string GetMessage(string key)
    {
        for (int i = 0; i < _localTablesSO._localTablesList.Count; i++)
        {
            if(key == _localTablesSO._localTablesList[i].LocalizationKey)
            {
                return _localTablesSO._localTablesList[i].Message;
            }
        }

        Debug.LogWarning("Unable to access localization key");
        return "Woubaloubaloub";
    }
}

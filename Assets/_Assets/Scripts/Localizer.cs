using UnityEngine;

public class Localizer : MonoBehaviour
{
    [SerializeField] private LocalizationManager _localizationManager;
    [SerializeField] private LocalTablesListSO _localTablesSO;

    private void Start()
    {
        _localizationManager.OnLocalTableLoaded.AddListener(InitializeMessages);
        _localizationManager.OnLanguageChanged.AddListener(UpdateMessages);
    }

    private void InitializeMessages()
    {
        for (int i = 0; i < _localTablesSO._localTablesList.Count; i++)
        {
            _localTablesSO._localTablesList[i].Message = _localizationManager.InitializeLocalizedString(_localTablesSO._localTablesList[i].LocalizationKey);
        }
    }

    private void UpdateMessages()
    {
        for (int i = 0; i < _localTablesSO._localTablesList.Count; i++)
        {
            _localTablesSO._localTablesList[i].Message = _localizationManager.UpdateLocalizedString(_localTablesSO._localTablesList[i].LocalizationKey);
        }
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

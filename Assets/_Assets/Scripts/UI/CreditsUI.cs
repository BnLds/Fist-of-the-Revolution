using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _closeText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _musicText;
    [SerializeField] private TextMeshProUGUI _cityText;
    [SerializeField] private TextMeshProUGUI _charactersText;
    [SerializeField] private TextMeshProUGUI _soundsText;

    private void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayButtonClickSound();
            Hide();
        });
    }

    private void Start()
    {
        _GameManager.Instance.OnGameUnpaused.AddListener(GameManager_OnGameUnpaused);
        Hide();
    }

    private void GameManager_OnGameUnpaused()
    {
        Hide();
    }

    public void Show()
    {
        LocalizeStrings();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LocalizeStrings()
    {
        _closeText.text = Localizer.Instance.GetMessage(LocalizationKeys.CLOSE_KEY);
        _titleText.text = Localizer.Instance.GetMessage(LocalizationKeys.CREDITS_KEY);
        _musicText.text = Localizer.Instance.GetMessage(LocalizationKeys.MUSIC_KEY) + ": Kyubz";
        _cityText.text = Localizer.Instance.GetMessage(LocalizationKeys.CITY_KEY) + ": Kenney";
        _charactersText.text = Localizer.Instance.GetMessage(LocalizationKeys.CHARACTERS_KEY) + ": Bit Gamey";
        _soundsText.text = Localizer.Instance.GetMessage(LocalizationKeys.SOUND_EFFECTS_KEY) + ": 3maze/Gamemaster Audio/Glitchedtones/West Wolf";
    }
    
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    private const string OPTIONS_KEY = "ui_options";
    private const string SOUND_EFFECTS_KEY = "ui_soundEffects";
    private const string MUSIC_KEY = "ui_music";
    private const string CLOSE_KEY = "ui_close";


    [SerializeField] private Button _soundEffectsButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private TextMeshProUGUI _soundEffectsText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _closeText;
    [SerializeField] private TextMeshProUGUI _musicText;
    [SerializeField] private TextMeshProUGUI _optionsText;

    private void Awake()
    {
        _soundEffectsButton.onClick.AddListener(()=> {
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

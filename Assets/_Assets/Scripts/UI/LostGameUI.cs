using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LostGameUI : MonoBehaviour
{
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _retryText;
    [SerializeField] private TextMeshProUGUI _mainMenuText;
    [SerializeField] private TextMeshProUGUI _lostText;
    [SerializeField] private InputFieldUI _inputFieldUI;

    private Localizer _localizer;

    private void Awake()
    {
        _retryButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Loader.Load(Loader.Scene.GameScene);
        });
        _mainMenuButton.onClick.AddListener(()=> 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        _localizer = Localizer.Instance;

        PoliceResponseManager.Instance.OnPlayerCaught.AddListener(() =>
        {
            Show();
        });

        Hide();
    }

    private void SetStrings()
    {
        _retryText.text = _localizer.GetMessage(LocalizationKeys.RETRY_KEY);
        _mainMenuText.text = _localizer.GetMessage(LocalizationKeys.MAIN_MENU_KEY);
        _lostText.text = _localizer.GetMessage(LocalizationKeys.LOST_KEY);
    }

    private void Show()
    {
        SetStrings();
        _inputFieldUI.Show();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

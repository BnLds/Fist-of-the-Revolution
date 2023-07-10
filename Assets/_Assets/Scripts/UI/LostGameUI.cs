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
        PoliceResponseManager.Instance.OnPlayerCaught.AddListener(() =>
        {
            Show();
        });
        Localizer.Instance.LocalizationLoaded.AddListener(SetStrings);

        Hide();
    }

    private void SetStrings()
    {
        _retryText.text = Localizer.Instance.GetMessage(LocalizationKeys.RETRY_KEY);
        _mainMenuText.text = Localizer.Instance.GetMessage(LocalizationKeys.MAIN_MENU_KEY);
        _lostText.text = Localizer.Instance.GetMessage(LocalizationKeys.LOST_KEY);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

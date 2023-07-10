using UnityEngine.UI;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private OptionsUI _optionsUI;

    private void Awake()
    {
        _playButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Loader.Load(Loader.Scene.GameScene);
        });

        _quitButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.PlayButtonClickSound();
            Application.Quit();

        });

        _optionsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayButtonClickSound();
            _optionsUI.Show();
        });

        Time.timeScale = 1f;
    }
}
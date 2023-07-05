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
            Loader.Load(Loader.Scene.GameScene);
        });

        _quitButton.onClick.AddListener(() => 
        {
            Application.Quit();

        });

        _optionsButton.onClick.AddListener(() =>
        {
            _optionsUI.Show();
        });

        Time.timeScale = 1f;
    }
}
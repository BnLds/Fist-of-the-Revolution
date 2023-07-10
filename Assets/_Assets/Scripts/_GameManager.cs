using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance { get; private set; }

    [HideInInspector] public UnityEvent OnGamePaused;
    [HideInInspector] public UnityEvent OnGameUnpaused;

    private bool _isGamePaused = false;
    private Scene _currentScene;

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
        GameInput.Instance.OnPauseAction.AddListener(GameInput_OnPauseAction);

        _currentScene = SceneManager.GetActiveScene();

        if(_currentScene.name == "MainMenuScene")
        {
                MusicManager.Instance.PlayMusic(MusicManager.MusicTag.MainMenu);

        }
        else if (_currentScene.name == "GameScene")
        {
                MusicManager.Instance.PlayMusic(MusicManager.MusicTag.NewProtester);

        }
    }

    private void GameInput_OnPauseAction()
    {
        TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        if(_currentScene.name == "MainMenuScene")
        {
            OnGameUnpaused?.Invoke();
        }
        else
        {
            _isGamePaused = !_isGamePaused;

            if(_isGamePaused)
            {
                Time.timeScale = 0f;
                OnGamePaused?.Invoke();
            }
            else
            {
                OnGameUnpaused?.Invoke();
                Time.timeScale = 1f;
            }
        }
    }
}

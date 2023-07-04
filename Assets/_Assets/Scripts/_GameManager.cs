using UnityEngine;
using UnityEngine.Events;

public class _GameManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnGamePaused;
    [HideInInspector] public UnityEvent OnGameUnpaused;

    [SerializeField] private GameInput _gameInput;

    private bool _isGamePaused = false;

    private void Start()
    {
        _gameInput.OnPauseAction.AddListener(GameInput_OnPauseAction);
    }

    private void GameInput_OnPauseAction()
    {
        TogglePauseGame();
    }

    private void TogglePauseGame()
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

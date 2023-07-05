using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    [HideInInspector] public UnityEvent OnInteractBegin;
    [HideInInspector] public UnityEvent OnInteractEnd;
    [HideInInspector] public UnityEvent OnCasseroladeBegin;
    [HideInInspector] public UnityEvent OnCasseroladeEnd;
    [HideInInspector] public UnityEvent OnPauseAction;


    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += OnInteractStarted;
        _playerInputActions.Player.Interact.canceled += OnInteractCanceled;
        _playerInputActions.Player.Casserolade.performed += OnCasseroladeStarted;
        _playerInputActions.Player.Casserolade.canceled += OnCasseroladeCanceled;
        _playerInputActions.Player.Pause.performed += OnPausePerformed;
    }

    private void Start()
    {
        PoliceResponseManager.Instance.OnPlayerCaught.AddListener(() =>
        {
            _playerInputActions.Player.Disable();
        });
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Interact.performed -= OnInteractStarted;
        _playerInputActions.Player.Interact.canceled -= OnInteractCanceled;
        _playerInputActions.Player.Casserolade.performed -= OnCasseroladeStarted;
        _playerInputActions.Player.Casserolade.canceled -= OnCasseroladeCanceled;
        _playerInputActions.Player.Pause.performed -= OnPausePerformed;

        _playerInputActions.Dispose();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke();
    }

    private void OnCasseroladeStarted(InputAction.CallbackContext context)
    {
        OnCasseroladeBegin?.Invoke();
    }

    private void OnCasseroladeCanceled(InputAction.CallbackContext context)
    {
        OnCasseroladeEnd?.Invoke();
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        OnInteractEnd?.Invoke();
    }

    private void OnInteractStarted(InputAction.CallbackContext obj)
    {
        OnInteractBegin?.Invoke();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector.Normalize();

        return inputVector;
    } 
}

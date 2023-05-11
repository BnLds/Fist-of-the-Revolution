using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    [HideInInspector] public UnityEvent OnInteractBegin;
    [HideInInspector] public UnityEvent OnInteractEnd;


    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        if(Instance != null)
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

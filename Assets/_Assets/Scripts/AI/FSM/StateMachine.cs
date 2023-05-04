using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private BaseState _currentState;

    protected virtual void Start()
    {
        _currentState = GetInitialState();

        if(_currentState != null)
        {
            _currentState.Enter();
        }
    }

    protected virtual void Update()
    {
        if(_currentState != null)
        {
            _currentState.UpdateLogic();
        }
    }

    private void LateUpdate()
    {
        if(_currentState != null)
        {
            _currentState.UpdatePhysics();
        }
    }

    public void ChangeState(BaseState newState)
    {
        _currentState.Exit();

        _currentState = newState;
        _currentState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    private void OnGUI()
    {
        string message = _currentState != null ? _currentState.name : "no current state";
        GUILayout.Label($"<color='black'><size=40>{message}</size></color>");
    }
}

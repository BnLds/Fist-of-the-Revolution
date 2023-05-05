using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState CurrentState { get; private set; }

    protected virtual void Start()
    {
        CurrentState = GetInitialState();

        if(CurrentState != null)
        {
            CurrentState.Enter();
        }
    }

    protected virtual void Update()
    {
        if(CurrentState != null)
        {
            CurrentState.UpdateLogic();
        }
    }

    private void LateUpdate()
    {
        if(CurrentState != null)
        {
            CurrentState.UpdatePhysics();
        }
    }

    public void ChangeState(BaseState newState)
    {
        CurrentState.Exit();

        CurrentState = newState;
        CurrentState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    private void OnGUI()
    {
        string message = CurrentState != null ? CurrentState.name : "no current state";
        GUILayout.Label($"<color='white'><size=40>{message}</size></color>");
    }
}

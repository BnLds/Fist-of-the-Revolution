public class BaseState 
{
    public string Name;
    protected StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.Name = name;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() {}
    public virtual void UpdateLogic() {}
    public virtual void UpdatePhysics () {}
    public virtual void Exit() {}
}

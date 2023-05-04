public class Idle : BaseState
{
    private PoliceUnitSM _policeUnitSM;

    public Idle(PoliceUnitSM stateMachine) : base("Idle", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.ChangeState(_policeUnitSM.FollowProtestState);
    }
}

using UnityEngine;

public class FollowProtest : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    public bool IsFollowingProtest { get; private set; }

    public FollowProtest(PoliceUnitSM stateMachine) : base("FollowProtest", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        IsFollowingProtest = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_policeUnitSM.PoliceUnitData.WatchedObjectsInReactionRange.Count != 0)
        {
            Exit();
            _policeUnitSM.ChangeState(_policeUnitSM.WatchObjectState);
        }

        IsFollowingProtest = true;
    }

    public override void Exit()
    {
        base.Exit();
        IsFollowingProtest = false;

    }
}

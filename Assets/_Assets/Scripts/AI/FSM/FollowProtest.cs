using UnityEngine;

public class FollowProtest : BaseState
{
    private PoliceUnitSM _policeUnitSM;


    public FollowProtest(PoliceUnitSM stateMachine) : base("FollowProtest", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        //_policeUnitSM.StartBrokenObjectsDetection();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_policeUnitSM.PoliceUnitData.WatchedObjectsInReactionRange.Count != 0)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WatchObjectState);
        }
    }
}

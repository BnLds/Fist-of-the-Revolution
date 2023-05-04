using UnityEngine;

public class FollowProtest : BaseState
{
    public bool IsFollowingProtest { get; private set; }
    
    private PoliceUnitSM _policeUnitSM;
    private float _countdownToWalkMax = 3f;
    private float _countdownToWalk;
    private float _countdownToPauseMax = 5f;
    private float _countdownToPause;

    public FollowProtest(PoliceUnitSM stateMachine) : base("FollowProtest", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        IsFollowingProtest = false;
        _countdownToPause = _countdownToPauseMax;
        _countdownToWalk = 0f;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _countdownToPause -= Time.deltaTime;
        _countdownToWalk -= Time.deltaTime;


        if(_countdownToWalk <=0)
        {
            IsFollowingProtest = true;
        }

        if(_countdownToPause <= 0)
        {
            IsFollowingProtest = false;
        }

        if(IsFollowingProtest)
        {
            _countdownToWalk = _countdownToWalkMax;
        }  
        else
        {
            _countdownToPause = _countdownToPauseMax;
        } 

        if (_policeUnitSM.PoliceUnitData.ObjectsToProtect.Count != 0)
        {
            Exit();
            _policeUnitSM.ChangeState(_policeUnitSM.WatchObjectState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        IsFollowingProtest = false;

    }
}

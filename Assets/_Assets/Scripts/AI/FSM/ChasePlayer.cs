using UnityEngine;

public class ChasePlayer : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    private float _detectionDelay;
    private float _catchAttemptDelay;
    
    public ChasePlayer(PoliceUnitSM stateMachine) : base("ChasePlayer", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = true;
        _policeUnitSM.PoliceUnitData.CurrentTarget = PlayerController.Instance.transform;

        _detectionDelay = 0f;
        _catchAttemptDelay = _policeUnitSM.DetectionDelay;
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        //follow protest is there is no target
        bool isPlayerIded = PoliceResponseManager.Instance.IsPlayerIdentified();
        if(!isPlayerIded) _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);

        //wander if the player is lost
        if(_policeUnitSM.IsTargetLost)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WanderState);
        }

        //bool targetIsPlayer = _policeUnitSM.PoliceUnitData.CurrentTarget == PlayerController.Instance.transform;
        bool targetIsWithinCatchDistance = Utility.Distance2DBetweenVector3(_policeUnitSM.PoliceUnitData.CurrentTarget.position, _policeUnitSM.transform.position) <= _policeUnitSM.CatchDistance;
        //check if player is within catch distance
        if(targetIsWithinCatchDistance)
        {
            _catchAttemptDelay -= Time.deltaTime;
            if(_catchAttemptDelay <= 0)
            {
                _policeUnitSM.AttemptCatchPlayer();
                _catchAttemptDelay = _policeUnitSM.CatchAttemptDelay;
            }
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _detectionDelay -= Time.deltaTime;
        
        if(_detectionDelay <= 0)
        {
            _detectionDelay = _policeUnitSM.DetectionDelay;
            _policeUnitSM.MoveDirectionInput = _policeUnitSM.GetMoveDirectionInput();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
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

        _detectionDelay = 0f;
        _catchAttemptDelay = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _catchAttemptDelay -= Time.deltaTime;

        //follow protest is there is no target
        bool isPlayerIded = PoliceResponseData.IsPlayerIdentified;
        if(!isPlayerIded) _policeUnitSM.ChangeState(_policeUnitSM.FollowSuspectState);

        //wander if the player is lost
        if(_policeUnitSM.IsTargetLost)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WanderState);
        }

        //bool targetIsPlayer = _policeUnitSM.PoliceUnitData.CurrentTarget == PlayerController.Instance.transform;
        bool targetIsWithinCatchDistance = Utility.Distance2DBetweenVector3(_policeUnitSM.PoliceUnitData.CurrentTarget.position, _policeUnitSM.transform.position) <= _policeUnitSM.CatchDistance;
        //check if player is within catch distance
        if(/*targetIsPlayer &&*/ targetIsWithinCatchDistance)
        {
            if(_catchAttemptDelay <= 0)
            {
                Debug.Log("Attempting to catch the player");
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

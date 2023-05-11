using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FollowSuspect : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    private float _detectionDelay;
    private float _catchAttemptDelay;
    private bool _hasTarget;

    public FollowSuspect(PoliceUnitSM stateMachine) : base("FollowSuspect", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        PlayerController.Instance.OnDamageDone.AddListener(PlayerController_OnDamageDone);
        _policeUnitSM.PoliceUnitData.IsChasingTarget = true;
        _hasTarget = _policeUnitSM.PoliceUnitData.CurrentTarget != null;

        _detectionDelay = 0f;
        _catchAttemptDelay = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = false;
    }

    private void PlayerController_OnDamageDone(Transform attacker)
    {
        bool canDetectTarget = _policeUnitSM.PoliceUnitData.Targets != null && _policeUnitSM.PoliceUnitData.Targets.Contains(_policeUnitSM.PoliceUnitData.CurrentTarget);
        bool suspectIsNotAttacker = attacker != _policeUnitSM.PoliceUnitData.CurrentTarget;
        
        //check if current target in within line of site and is not the attacker
        if(_hasTarget && canDetectTarget && suspectIsNotAttacker)
        {
            //clear the target from suspicion
            Debug.Log("suspect no longer suspected: " + _policeUnitSM.PoliceUnitData.CurrentTarget);
            //remove current tracked suspect from suspects list
            PoliceResponseData.TrackedSuspects.Remove(PoliceResponseData.TrackedSuspects.FirstOrDefault(_ => _.SuspectTransform == _policeUnitSM.PoliceUnitData.CurrentTarget));
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _catchAttemptDelay -= Time.deltaTime;

        //follow protest is there is no target
        if(!_hasTarget) _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);

        //wander if the target is lost
        if(_policeUnitSM.IsTargetLost && _hasTarget)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WanderState);
        }

        bool targetIsPlayer = _policeUnitSM.PoliceUnitData.CurrentTarget == PlayerController.Instance.transform;
        bool targetIsWithinCatchDistance = Utility.Distance2DBetweenVector3(_policeUnitSM.PoliceUnitData.CurrentTarget.position, _policeUnitSM.transform.position) <= _policeUnitSM.CatchDistance;
        //check if player is within catch distance
        if(targetIsPlayer && targetIsWithinCatchDistance)
        {
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

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FollowSuspect : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    private float _detectionDelay;
    private float _catchAttemptDelay;

    public FollowSuspect(PoliceUnitSM stateMachine) : base("FollowSuspect", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        PlayerController.Instance.OnDamageDone.AddListener(PlayerController_OnDamageDone);
        _policeUnitSM.PoliceUnitData.IsChasingTarget = true;

        _detectionDelay = 0f;
        _catchAttemptDelay = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = false;
    }

    private void PlayerController_OnDamageDone(Transform arg0)
    {
        //check if current target in within line of site
        if(_policeUnitSM.PoliceUnitData.CurrentTarget != null && _policeUnitSM.PoliceUnitData.Targets != null && _policeUnitSM.PoliceUnitData.Targets.Contains(_policeUnitSM.PoliceUnitData.CurrentTarget))
        {
            Debug.Log("suspect no longer suspected");
            //remove current tracked suspect from suspects list
            PoliceResponseData.TrackedSuspects.Remove(PoliceResponseData.TrackedSuspects.FirstOrDefault(_ => _.SuspectTransform == _policeUnitSM.PoliceUnitData.CurrentTarget));
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _catchAttemptDelay -= Time.deltaTime;

        if(_policeUnitSM.PoliceUnitData.CurrentTarget == null) _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);

        if(_policeUnitSM.IsTargetLost && _policeUnitSM.PoliceUnitData.CurrentTarget!= null)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WanderState);
        }

        //check if player within catch distance
        if(Utility.Distance2DBetweenVector3(_policeUnitSM.PoliceUnitData.CurrentTarget.position, _policeUnitSM.transform.position)<= _policeUnitSM.CatchDistance)
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

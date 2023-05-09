using System;
using UnityEngine;

public class FollowProtest : BaseState
{
    public bool IsFollowingProtest { get; private set; }
    
    private PoliceUnitSM _policeUnitSM;
    private float _countdownToWalk;
    private float _countdownToPause;
    private float _detectionDelay;

    public FollowProtest(PoliceUnitSM stateMachine) : base("FollowProtest", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        //IsFollowingProtest is used by the PolicemanController to update policeman position
        IsFollowingProtest = false;
        _countdownToPause = _policeUnitSM.CountdownToPauseMax;
        _countdownToWalk = 0f;
        _detectionDelay = 0f;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _detectionDelay -= Time.deltaTime;

        //check if player is identified
        if(PoliceResponseData.IsPlayerIdentified)
        {
            //check if player is within detection range and line of sight
            if (_detectionDelay <= 0)
            {
                _detectionDelay = _policeUnitSM.DetectionDelay;
                if (Utility.Distance2DBetweenVector3(PlayerController.Instance.transform.position, _policeUnitSM.transform.position) <= _policeUnitSM.PlayerDetectionRange && _policeUnitSM.IsPlayerInLineOfSight()) 
                {
                    //assign new target in unit data
                    _policeUnitSM.PoliceUnitData.CurrentTarget = PlayerController.Instance.transform;
                    //follow player
                    _policeUnitSM.ChangeState(_policeUnitSM.FollowSuspectState);
                }
            }
        }
        else
        {
            if (_policeUnitSM.PoliceUnitData.ObjectsToProtect.Count != 0)
            {
                _policeUnitSM.ChangeState(_policeUnitSM.WatchObjectState);
            }

            if(_detectionDelay<= 0)
            {
                _detectionDelay = _policeUnitSM.DetectionDelay;
                for (int i = 0; i < PoliceResponseData.TrackedSuspects.Count; i++)
                {
                    //check if suspect is not already tracked and if it is within detection range 
                    if (PoliceResponseData.TrackedSuspects[i].IsTracked == false && Utility.Distance2DBetweenVector3(PoliceResponseData.TrackedSuspects[i].SuspectTransform.position, _policeUnitSM.transform.position) <= _policeUnitSM.PlayerDetectionRange)
                    {
                        //set it to tracked in police response data
                        (Transform, bool) newTargetData = (PoliceResponseData.TrackedSuspects[i].SuspectTransform, true) ;
                        PoliceResponseData.TrackedSuspects[i] = newTargetData;
                        //assign new target in unit data
                        _policeUnitSM.PoliceUnitData.CurrentTarget = PoliceResponseData.TrackedSuspects[i].SuspectTransform;
                    
                        _policeUnitSM.ChangeState(_policeUnitSM.FollowSuspectState);
                    }
                }
            }
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
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
            _countdownToWalk = _policeUnitSM.CountdownToWalkMax;
        }  
        else
        {
            _countdownToPause = _policeUnitSM.CountdownToPauseMax;
        } 
    }

    public override void Exit()
    {
        base.Exit();
        IsFollowingProtest = false;
    }
}

using System;
using UnityEngine;

public class FollowProtest : BaseState
{
    public bool IsFollowingProtest { get; private set; }
    
    private PoliceUnitSM _policeUnitSM;
    private float _countdownToWalkMax = 3f;
    private float _countdownToWalk;
    private float _countdownToPauseMax = 5f;
    private float _countdownToPause;
    private float _detectionDelay;

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
        _detectionDelay = 0f;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _countdownToPause -= Time.deltaTime;
        _countdownToWalk -= Time.deltaTime;
        _detectionDelay -= Time.deltaTime;

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
                    (Transform, bool) element = (PoliceResponseData.TrackedSuspects[i].SuspectTransform, true) ;
                    PoliceResponseData.TrackedSuspects[i] = element;
                    //assign new target in unit data
                    _policeUnitSM.PoliceUnitData.CurrentTarget = PoliceResponseData.TrackedSuspects[i].SuspectTransform;
                
                    _policeUnitSM.ChangeState(_policeUnitSM.FollowSuspectState);
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        IsFollowingProtest = false;
    }
}

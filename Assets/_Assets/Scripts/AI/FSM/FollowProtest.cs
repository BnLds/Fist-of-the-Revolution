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

        _policeUnitSM.EnterFollowProtestState = true;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _detectionDelay -= Time.deltaTime;

        //check if player is identified
        if(PoliceResponseManager.Instance.IsPlayerIdentified())
        {
            //check if player is within detection range and line of sight
            if (_detectionDelay <= 0)
            {
                _detectionDelay = _policeUnitSM.DetectionDelay;
                if (Utility.Distance2DBetweenVector3(PlayerController.Instance.transform.position, _policeUnitSM.transform.position) <= _policeUnitSM.PlayerDetectionRange && _policeUnitSM.IsPlayerInLineOfSight()) 
                {
                    //follow player
                    _policeUnitSM.ChangeState(_policeUnitSM.ChasePlayerState);
                }
            }
        }
        else
        {
            //player is not IDed
            if (_detectionDelay <= 0)
            {
                _detectionDelay = _policeUnitSM.DetectionDelay;
                for (int i = 0; i < PoliceResponseManager.Instance.GetTrackedList().Count; i++)
                {
                    //check if suspect is not already tracked and if it is within detection range 
                    if (PoliceResponseManager.Instance.GetTrackedList()[i].IsTracked == false && Utility.Distance2DBetweenVector3(PoliceResponseManager.Instance.GetTrackedList()[i].SuspectTransform.position, _policeUnitSM.transform.position) <= _policeUnitSM.PlayerDetectionRange)
                    {
                        //set it to tracked in police response data
                        PoliceResponseManager.Instance.AddFollowedTargetToTrackedList(PoliceResponseManager.Instance.GetTrackedList()[i].SuspectTransform);
                        //assign new target in unit data
                        _policeUnitSM.PoliceUnitData.CurrentTarget = PoliceResponseManager.Instance.GetTrackedList()[i].SuspectTransform;
                        _policeUnitSM.ChangeState(_policeUnitSM.FollowSuspectState);
                        break;
                    }
                }
            }
        }

        if (_policeUnitSM.PoliceUnitData.ObjectsToProtect.Count != 0)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WatchObjectState);
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        if(_policeUnitSM.PoliceUnitData.IsStatic)
        {
            IsFollowingProtest = false;
        }
        else
        {
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
    }

    public override void Exit()
    {
        base.Exit();
        IsFollowingProtest = false;
    }
}

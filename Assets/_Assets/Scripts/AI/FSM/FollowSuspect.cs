using UnityEngine;

public class FollowSuspect : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    private float _detectionDelay;

    public FollowSuspect(PoliceUnitSM stateMachine) : base("FollowSuspect", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        PlayerController.Instance.OnAttackPerformed.AddListener(PlayerController_OnDamageDone);
        _policeUnitSM.PoliceUnitData.IsChasingTarget = true;
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = false;
        PlayerController.Instance.OnAttackPerformed.RemoveListener(PlayerController_OnDamageDone);

        if(PoliceResponseManager.Instance.GetTrackedList().Contains((_policeUnitSM.PoliceUnitData.CurrentTarget, true)))
        {
            PoliceResponseManager.Instance.SetTrackedSuspectToUnfollowed(_policeUnitSM.PoliceUnitData.CurrentTarget);
        }
    }

    private void PlayerController_OnDamageDone(Transform attacker)
    {
        bool hasTarget = _policeUnitSM.PoliceUnitData.CurrentTarget != null;
        bool canDetectTarget = _policeUnitSM.PoliceUnitData.Targets != null && _policeUnitSM.PoliceUnitData.Targets.Contains(_policeUnitSM.PoliceUnitData.CurrentTarget);
        bool suspectIsNotAttacker = attacker != _policeUnitSM.PoliceUnitData.CurrentTarget;
        
        //check if current target in within line of site and is not the attacker
        if(hasTarget && canDetectTarget && suspectIsNotAttacker)
        {
            //clear the target from suspicion
            Debug.Log("suspect no longer suspected: " + _policeUnitSM.PoliceUnitData.CurrentTarget);
            //remove current tracked suspect from suspects list
            PoliceResponseManager.Instance.ClearTrackedSuspect(_policeUnitSM.PoliceUnitData.CurrentTarget);
            _policeUnitSM.PoliceUnitData.CurrentTarget = null;

            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        bool hasTarget = _policeUnitSM.PoliceUnitData.CurrentTarget != null;
        if(!hasTarget)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        } 

        //wander if the player is lost
        if(_policeUnitSM.IsTargetLost)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.WanderState);
        }

        if(PoliceResponseManager.Instance.IsPlayerIdentified() && _policeUnitSM.IsPlayerInLineOfSight() && Utility.Distance2DBetweenVector3(PlayerController.Instance.transform.position, _policeUnitSM.transform.position) <= _policeUnitSM.PlayerDetectionRange)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.ChasePlayerState);
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _detectionDelay -= Time.deltaTime;

        float chaseMinDistance = 3f;
        if(Utility.Distance2DBetweenVector3(_policeUnitSM.transform.position, PlayerController.Instance.transform.position) >= chaseMinDistance)
        {
            if(_detectionDelay <= 0)
            {
                _detectionDelay = _policeUnitSM.DetectionDelay;
                _policeUnitSM.MoveDirectionInput = _policeUnitSM.GetMoveDirectionInput();
            }
        }
        else
        {
            _policeUnitSM.MoveDirectionInput = Vector3.zero;

        }
    }
}

using System.Linq;
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
        PlayerController.Instance.OnDamageDone.AddListener(PlayerController_OnDamageDone);
        _policeUnitSM.PoliceUnitData.IsChasingTarget = true;
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.PoliceUnitData.IsChasingTarget = false;
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
            PoliceResponseData.TrackedSuspects.Remove(PoliceResponseData.TrackedSuspects.FirstOrDefault(_ => _.SuspectTransform == _policeUnitSM.PoliceUnitData.CurrentTarget));
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        bool hasTarget = _policeUnitSM.PoliceUnitData.CurrentTarget != null;
        if(!hasTarget) _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
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

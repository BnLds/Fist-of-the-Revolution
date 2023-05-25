using UnityEngine;

public class Flee : BaseState
{
    PoliceUnitSM _policeUnitSM;
    public Flee(PoliceUnitSM stateMachine) : base("Flee", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        
        PlayerController.Instance.OnStoppedCasserolade.AddListener(PlayerController_OnStoppedCasserolade);

    }

    private void PlayerController_OnStoppedCasserolade()
    {
        _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float policeFleeingDistance = 8f;
        Vector3 direction = new Vector3((_policeUnitSM.transform.position - PlayerController.Instance.transform.position).x, 0, (_policeUnitSM.transform.position - PlayerController.Instance.transform.position).z).normalized; ;
        if(Utility.Distance2DBetweenVector3(_policeUnitSM.transform.position, PlayerController.Instance.transform.position) < policeFleeingDistance)
        {
            _policeUnitSM.MoveDirectionInput = direction;
        }
        else
        {
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
    }

}

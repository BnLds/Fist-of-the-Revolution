using UnityEngine;

public class Wander : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    private Vector3 _wanderPoint;
    private bool _isFirstWanderPoint;
    private float _detectionDelay;
    private float _wanderTime;
    private float _wanderRandomDistanceMax;
    private float _wanderPointReachedRange = 2f;


    public Wander(PoliceUnitSM stateMachine) : base("Wander", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        _wanderTime = _policeUnitSM.WanderDuration;
        _wanderRandomDistanceMax = _policeUnitSM.WanderRandomDistanceMax;
        _isFirstWanderPoint = true;


        CreateNewWanderPoint();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _wanderTime -= Time.deltaTime;
        _detectionDelay -= Time.deltaTime;

        if(_wanderTime <= 0)
        {
            //do not track player anymore
            _policeUnitSM.PoliceUnitData.CurrentTarget = null;
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }

        //check if player is within detection range and line of sight
        if (_detectionDelay <= 0)
        {
            _detectionDelay = _policeUnitSM.DetectionDelay;
            if (Utility.Distance2DBetweenVector3(PlayerController.Instance.transform.position, _policeUnitSM.transform.position) <= _policeUnitSM.PlayerDetectionRange && _policeUnitSM.IsPlayerInLineOfSight()) 
            {
                if(PoliceResponseManager.Instance.IsPlayerIdentified())
                {
                    //assign new target in unit data
                    _policeUnitSM.PoliceUnitData.CurrentTarget = PlayerController.Instance.transform;
                    //chase player
                    _policeUnitSM.ChangeState(_policeUnitSM.ChasePlayerState);
                }
            }
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        //check if the policeman is within distance of the wander point
        if (Utility.Distance2DBetweenVector3(_wanderPoint, _policeUnitSM.transform.position) < _wanderPointReachedRange)
        {
            CreateNewWanderPoint();
        }
        else if (_policeUnitSM.PoliceUnitData.CurrentFlowField != null)
        {
            //go to damaged object following genereated flowfield
            _policeUnitSM.MoveDirectionInput = _policeUnitSM.GetMoveDirectionInput();
        }
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.IsTargetLost = false;
    }

    private void CreateNewWanderPoint()
    {
        if(!_isFirstWanderPoint && _wanderPoint != null)
        {
            _wanderPoint = new Vector3(_wanderPoint.x + Random.Range(0f, _wanderRandomDistanceMax), _wanderPoint.y, _wanderPoint.z + Random.Range(0f, _wanderRandomDistanceMax));
            while(!IsWanderPointAccessible(_wanderPoint))
            {
                _wanderPoint = new Vector3(_wanderPoint.x + Random.Range(0f, _wanderRandomDistanceMax), _wanderPoint.y, _wanderPoint.z + Random.Range(0f, _wanderRandomDistanceMax));
            }
        }
        else
        {
            _wanderPoint = new Vector3(_policeUnitSM.PoliceUnitData.CurrentTarget.position.x + Random.Range(0f, _wanderRandomDistanceMax), _policeUnitSM.PoliceUnitData.CurrentTarget.position.y, _policeUnitSM.PoliceUnitData.CurrentTarget.position.z + Random.Range(0f, _wanderRandomDistanceMax));
            while(!IsWanderPointAccessible(_wanderPoint))
            {
                _wanderPoint = new Vector3(_wanderPoint.x + Random.Range(0f, _wanderRandomDistanceMax), _wanderPoint.y, _wanderPoint.z + Random.Range(0f, _wanderRandomDistanceMax));
            }
        }

        _isFirstWanderPoint = false;

        //Generate flowfield to next wander point
        _policeUnitSM.PoliceUnitData.CurrentFlowField = GridController.Instance.GenerateFlowField(_wanderPoint);
    }

    private bool IsWanderPointAccessible(Vector3 pointPosition)
    {
        float radius = 0.5f;
        int unwalkableLayerValue = 1 << 7;
        return !Physics.CheckSphere(pointPosition, radius, unwalkableLayerValue);
    }
}

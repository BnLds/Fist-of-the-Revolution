using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WatchObject : BaseState
{
    private PoliceUnitSM _policeUnitSM;
    private bool _isSuspectsListUpdated;
    private float _detectionDelay;
    private bool _isObjectDestroyed;

    public WatchObject(PoliceUnitSM stateMachine) : base("WatchObject", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _isSuspectsListUpdated = false;
        _isObjectDestroyed = false;

        _policeUnitSM.OnObjectDestroyed.AddListener(PoliceUnitSM_OnObjectDestroyed);

        //if within reaction range of watched objects, go to protect the closest object
        if (_policeUnitSM.PoliceUnitData.ObjectsToProtect.Count != 0)
        {
            //order the list of objects to protect by distance to policeUnit
            List<Transform> reactionList = _policeUnitSM.PoliceUnitData.ObjectsToProtect.OrderBy(target => Utility.Distance2DBetweenVector3(_policeUnitSM.transform.position, target.position)).ToList();
            //get the closest HighPriority object or null
            Transform reactionPoint = reactionList.FirstOrDefault(target => target.GetComponent<BreakableController>().IsHighPriority);
            if(reactionPoint == null) 
            {
                //Get the closest watched object if there is no HighPriority
                reactionPoint = reactionList.FirstOrDefault();
            }
            
            _policeUnitSM.PoliceUnitData.CurrentWatchedObject = reactionPoint;
            _policeUnitSM.PoliceUnitData.CurrentWatchObjectPosition = new Vector3 (reactionPoint.position.x,reactionPoint.position.y, reactionPoint.position.z);

            //Generate flowfield to reaction point
            _policeUnitSM.PoliceUnitData.CurrentFlowField = GridController.Instance.GenerateFlowField(reactionPoint.position);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _policeUnitSM.OnObjectDestroyed.RemoveListener(PoliceUnitSM_OnObjectDestroyed);

    }

    private void PoliceUnitSM_OnObjectDestroyed(Transform objectDestroyed)
    {
        if(_policeUnitSM.PoliceUnitData.CurrentWatchedObject == objectDestroyed)
        {
            _isObjectDestroyed = true;
        }
        
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
                    _policeUnitSM.ChangeState(_policeUnitSM.ChasePlayerState);
                }
            }
        }

        float identificationRange = 5f;
        if(Utility.Distance2DBetweenVector3(_policeUnitSM.transform.position, _policeUnitSM.PoliceUnitData.CurrentWatchObjectPosition) <= identificationRange && !_isSuspectsListUpdated)
        {
            _isSuspectsListUpdated = true;
            PoliceResponseManager.Instance.UpdateClosestSuspects(_policeUnitSM.transform, _policeUnitSM.PoliceUnitData.CurrentWatchObjectPosition);
        }

        if(_isSuspectsListUpdated && _isObjectDestroyed)
        {
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float reactionPointReachedRange = 2f;
        //check if the policeman is within distance of the object to watch
        if (Utility.Distance2DBetweenVector3(_policeUnitSM.PoliceUnitData.CurrentWatchObjectPosition, _policeUnitSM.transform.position) < reactionPointReachedRange)
        {
            //stop and reset flowfield
            _policeUnitSM.MoveDirectionInput = Vector3.zero;
            _policeUnitSM.PoliceUnitData.CurrentFlowField = null;
        }
        else if (_policeUnitSM.PoliceUnitData.CurrentFlowField != null)
        {
            //go to damaged object following genereated flowfield
            _policeUnitSM.MoveDirectionInput = _policeUnitSM.GetMoveDirectionInput();
        }
    }
}

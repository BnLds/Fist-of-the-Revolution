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
    private bool _isWatchingObject;

    public WatchObject(PoliceUnitSM stateMachine) : base("WatchObject", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _isSuspectsListUpdated = false;
        _isObjectDestroyed = false;
        _isWatchingObject = false;

        //if within reaction range of watched objects, go to protect the closest object
        if (_policeUnitSM.PoliceUnitData.ObjectsToProtect.Count != 0)
        {
            List<Transform> reactionList;

            int retries = 0;
            int maxRetries = 3;
            while(true)
            {
                try
                {
                    //order the list of objects to protect by distance to policeUnit
                    reactionList = _policeUnitSM.PoliceUnitData.ObjectsToProtect.OrderBy(target => Utility.Distance2DBetweenVector3(_policeUnitSM.transform.position, target.position)).ToList();
                    break;
                }
                //catch exception when object was destroyed but it was not raken into account by system yet
                catch (NullReferenceException e)
                {
                    if(retries < maxRetries)
                    {
                        retries++;
                        _policeUnitSM.WaitForEndOfframe();
                    }
                    else
                    {
                        //exit state if there is an error finding an object (likely destroyed)
                        Debug.LogWarning("Changing state to avoid Exception " + e);
                        _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
                    }
                }
            }

            //Look first for a HighPriority match
            List<Transform> reactionListHighPriority = reactionList.Where(_=>_.GetComponent<BreakableController>().IsHighPriority).ToList();
            Transform reactionPoint = GetAvailableReactionPoint(reactionListHighPriority);

            //check if there was no HighPriority match
            if(reactionPoint == null)
            {
                //there was no HighPriority match
                //Get the closest regular watched object with number of Watchers not maxed out
                reactionPoint = GetAvailableReactionPoint(reactionList);
            }

            //check if cop was able to find any object to watch
            if(reactionPoint == null)
            {
                //change state if there is no object to watch
                _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
                return;
            }

            //Generate flowfield to reaction point
            _policeUnitSM.PoliceUnitData.CurrentFlowField = PoliceResponseManager.Instance.GetPoliceForceFlowfield(reactionPoint.position);
            
            _policeUnitSM.PoliceUnitData.CurrentWatchedObject = reactionPoint;
            _policeUnitSM.PoliceUnitData.CurrentWatchObjectPosition = new Vector3 (reactionPoint.position.x,reactionPoint.position.y, reactionPoint.position.z);
        }
    }

    private Transform GetAvailableReactionPoint(List<Transform> reactionList)
    {
        Transform reactionPoint = reactionList.FirstOrDefault();
        while (reactionPoint != null)
        {
            //check if the max number of watchers allowed has already been reached
            if (_policeUnitSM.CanWatchObject(reactionPoint))
            {
                //if not, exit the loop and generate flowfield
                _policeUnitSM.AddWatcher(reactionPoint);
                _isWatchingObject = true;
                return reactionPoint;
            }
            else
            {
                //if the max number of watchers has been reached, try to find another object to watch
                //loop through the list of objects to watch
                reactionList.Remove(reactionPoint);
                _policeUnitSM.RemoveObjectToProtect(reactionPoint);
                reactionPoint = reactionList.FirstOrDefault();
            }
        }

        return reactionPoint;
    }

    public override void Exit()
    {
        base.Exit();

        if(_isWatchingObject)
        {
            _policeUnitSM.RemoveWatcher(_policeUnitSM.PoliceUnitData.CurrentWatchedObject);
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        _isObjectDestroyed = _policeUnitSM.PoliceUnitData.CurrentWatchedObject.GetComponent<BreakableController>().WasDestroyed;

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
            PoliceResponseManager.Instance.RemoveObjectOnDestruction(_policeUnitSM.PoliceUnitData.CurrentWatchedObject);
            _policeUnitSM.PoliceUnitData.ObjectsToProtect.Remove(_policeUnitSM.PoliceUnitData.CurrentWatchedObject);
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

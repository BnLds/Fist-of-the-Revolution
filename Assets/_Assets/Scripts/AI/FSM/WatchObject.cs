using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WatchObject : BaseState
{
    private PoliceUnitSM _policeUnitSM;

    public WatchObject(PoliceUnitSM stateMachine) : base("WatchObject", stateMachine)
    {
        _policeUnitSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

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
            _policeUnitSM.PoliceUnitData.CurrentWatchObjectPosition = reactionPoint.position;
            //Generate flowfield to reaction point
            //policemanData.currentFlowField = PoliceFlowfieldsGenerator.Instance.CreateNewFlowField(reactionPoint);
            _policeUnitSM.PoliceUnitData.CurrentFlowField = GridController.Instance.GenerateFlowField(reactionPoint);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void PoliceUnitSM_OnObjectDestroyed(Transform objectDestroyed)
    {
        if(_policeUnitSM.PoliceUnitData.CurrentWatchedObject == objectDestroyed)
        {
            _policeUnitSM.PoliceUnitData.CurrentWatchedObject = null;
            Exit();
            _policeUnitSM.ChangeState(_policeUnitSM.FollowProtestState);
        }
        
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
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

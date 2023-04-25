using System.Collections.Generic;
using UnityEngine;

public class PolicemanData : AIData
{
    public List<Transform> watchedObjectsInReactionRange;
    public FlowField currentFlowField = null;
    public Transform currentWatchObject;
}

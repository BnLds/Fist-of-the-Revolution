using System.Collections.Generic;
using UnityEngine;

public class PolicemanData : AIData
{
    public List<Transform> WatchedObjectsInReactionRange;
    public FlowField CurrentFlowField = null;
    public Vector3 CurrentWatchObjectPosition;
}

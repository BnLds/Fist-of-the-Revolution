using System.Collections.Generic;
using UnityEngine;

public class PolicemanData : AIData
{
    public List<Transform> ObjectsToProtect;
    public FlowField CurrentFlowField = null;
    public Vector3 CurrentWatchObjectPosition;
    public Transform CurrentWatchedObject;
}

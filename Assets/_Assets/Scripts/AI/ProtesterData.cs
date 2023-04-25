using System.Collections.Generic;
using UnityEngine;

public class ProtesterData : AIData
{
    //flowfields data
    public List<ProtestFlowFieldData> flowFieldsProtest;
    public int currentFlowFieldIndex;
    public Transform endOfProtest;
    public bool reachedEndOfProtest = false;
}

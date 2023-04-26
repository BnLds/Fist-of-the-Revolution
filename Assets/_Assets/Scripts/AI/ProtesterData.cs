using System.Collections.Generic;
using UnityEngine;

public class ProtesterData : AIData
{
    //flowfields data
    public List<ProtestFlowFieldData> FlowFieldsProtest;
    public int CurrentFlowFieldIndex;
    public Transform EndOfProtest;
    public bool ReachedEndOfProtest = false;
}

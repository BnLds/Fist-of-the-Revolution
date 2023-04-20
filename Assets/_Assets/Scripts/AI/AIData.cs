using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    //target seeking behaviour data 
    public List<Transform> targets = null;
    public Collider[] obstacles = null;
    public Transform currentTarget;

    //flowfields data
    public List<FlowFieldData> flowFieldsProtest;
    public int currentFlowFieldIndex;
    public Transform endOfProtest;
    public bool reachedEndOfProtest = false;

    public int GetTargetsCount() => targets == null ? 0 : targets.Count;
}

using System.Collections.Generic;
using UnityEngine;

public class PoliceResponseData 
{
    public Dictionary<Transform, (int WatchersLimit, int numberOfWatchers)> WatchPoints;
    //List of NPN and player close to damaged area
    public List<Transform> Suspects;
    public List<(Transform SuspectTransform, bool IsTracked)> TrackedSuspects;
    public bool IsPlayerIdentified;
    public bool IsPlayerTracked;
    public (FlowField flowfield, Vector3 target) HighPriorityFlowfield;
}

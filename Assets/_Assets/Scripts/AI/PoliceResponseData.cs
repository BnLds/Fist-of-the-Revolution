using System.Collections.Generic;
using UnityEngine;

public class PoliceResponseData 
{
    public List<Transform> WatchPoints;
    //List of NPN and player close to damaged area
    public List<Transform> Suspects;
    public List<(Transform SuspectTransform, bool IsTracked)> TrackedSuspects;
    public bool IsPlayerIdentified;

}

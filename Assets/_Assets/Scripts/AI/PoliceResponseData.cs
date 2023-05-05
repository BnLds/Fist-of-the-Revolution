using System.Collections.Generic;
using UnityEngine;

public static class PoliceResponseData 
{
    public static void ResetStaticData()
    {
        WatchPoints = null;
        Suspects = null;
        TrackedSuspects = null;
    }

    public static List<Transform> WatchPoints;
    //List of NPN and player close to damaged area
    public static List<Transform> Suspects;
    public static List<Transform> TrackedSuspects;
    public static bool IsPlayerIdentified;

}

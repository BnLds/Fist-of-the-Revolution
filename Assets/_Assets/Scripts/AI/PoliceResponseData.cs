using System.Collections.Generic;
using UnityEngine;

public static class PoliceResponseData 
{
    public static void ResetStaticData()
    {
        WatchPoints = null;
    }

    public static List<Transform> WatchPoints;
    public static List<Transform> Suspects;
    public static Transform Culprit;

}

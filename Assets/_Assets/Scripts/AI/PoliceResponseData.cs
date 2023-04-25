using System.Collections.Generic;
using UnityEngine;

public static class PoliceResponseData 
{
    public static void ResetStaticData()
    {
        watchPoints = null;
    }

    public static List<Transform> watchPoints;

}

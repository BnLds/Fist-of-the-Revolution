using UnityEngine;

public static class Utility
{
    public static Vector2 ConvertVector3ToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static float Distance2DBetweenVector3(Vector3 vect1, Vector3 vect2)
    {
        return Vector2.Distance(Utility.ConvertVector3ToVector2(vect1), Utility.ConvertVector3ToVector2(vect2));
    }
}

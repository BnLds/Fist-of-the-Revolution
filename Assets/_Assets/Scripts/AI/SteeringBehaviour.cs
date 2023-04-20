using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    public abstract (float[] danger, float[] interest) GetSteeringToTargets(float[] danger, float[] interest, AIData aiData);
}

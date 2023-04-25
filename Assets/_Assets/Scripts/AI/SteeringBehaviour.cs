using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    public abstract (float[] danger, float[] interest) GetSteeringToTargets(float[] danger, float[] interest, AIData aiData);
    public abstract (float[] danger, float[] interest) GetSteeringFlowFields(float[] danger, float[] interest, ProtesterData protesterData);
    public abstract (float[] danger, float[] interest) GetSteeringPoliceReaction(float[] danger, float[] interest, PolicemanData policemanData);
}

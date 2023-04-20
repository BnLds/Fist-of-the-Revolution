using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;

    //gizmo parameters
    private float[] interestGizmo;
    private Vector3 resultDirection = Vector3.zero;
    private float rayLength = 1f;

    private void Awake()
    {
        interestGizmo = new float[8];
    }

    public Vector3 GetChaseDirection(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        //loop through each behaviour
        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteeringToTargets(danger, interest, aiData);
        }

        //substract danger values from interest array
        for (int i = 0; i < interest.Length; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        interestGizmo = interest;

        //get the average direction
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < interest.Length; i++)
        {
            outputDirection += GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]) * interest[i];
        }
        outputDirection.Normalize();

        resultDirection = new Vector3(outputDirection.x, 0, outputDirection.y);

        return resultDirection;
    }

    public Vector3 GetProtestDirection(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        //loop through each behaviour
        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteeringFlowFields(danger, interest, aiData);
        }

        //substract danger values from interest array
        for (int i = 0; i < interest.Length; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        interestGizmo = interest;

        //get the average direction
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < interest.Length; i++)
        {
            outputDirection += GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]) * interest[i];
        }
        outputDirection.Normalize();

        resultDirection = new Vector3(outputDirection.x, 0, outputDirection.y);

        return resultDirection;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLength);
        }
    }
}

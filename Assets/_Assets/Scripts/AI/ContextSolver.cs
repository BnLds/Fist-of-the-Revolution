using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField] private bool _showGizmos = true;

    //gizmo parameters
    private float[] _interestGizmo;
    private Vector3 _resultDirection = Vector3.zero;
    private float _rayLength = 1f;

    private void Awake()
    {
        _interestGizmo = new float[8];
    }

    public Vector3 GetContextDirection(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        if(aiData is ProtesterData || (aiData is PolicemanData policemanData && policemanData.IsChasingTarget == false))
        {
            //loop through each behaviour
            foreach (SteeringBehaviour behaviour in behaviours)
            {
                (danger, interest) = behaviour.GetSteeringFlowFields(danger, interest, aiData);
            }
        }
        else
        {
            //loop through each behaviour
            foreach (SteeringBehaviour behaviour in behaviours)
            {
                (danger, interest) = behaviour.GetSteeringToTargets(danger, interest, aiData);
            }
        }

        //substract danger values from interest array
        for (int i = 0; i < interest.Length; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        _interestGizmo = interest;

        //get the average direction
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < interest.Length; i++)
        {
            outputDirection += GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]) * interest[i];
        }
        outputDirection.Normalize();

        _resultDirection = new Vector3(outputDirection.x, 0, outputDirection.y);

        return _resultDirection;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && _showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, _resultDirection * _rayLength);
        }
    }
}

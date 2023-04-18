using UnityEngine;
using System.Linq;


public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField] private float targetReachedThreshold = 0.7f;
    [SerializeField] private bool showGizmos = true;

    private bool reachedLastTarget = true;

    //gizmo parameters
    private Vector3 targetPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        //if we don't have a target stop seeking
        //else set a new target
        if(reachedLastTarget)
        {
            if(aiData.targets == null || aiData.targets.Count <= 0)
            {
                aiData.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                reachedLastTarget = false;
                aiData.currentTarget = aiData.targets.OrderBy(target => Vector2.Distance(transform.position, target.position)).FirstOrDefault();
            }
        }

        //cache the last position only if we still see the target (if the targets collection is not empty)
        if(aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
        {
            targetPositionCached = aiData.currentTarget.position;
        }

        //first check if we have reached the target
        if(Vector2.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        //if we havent reached the target, do the main logic of finding the interest directions
        Vector2 directionToTarget = new Vector2((targetPositionCached - transform.position).x, (targetPositionCached - transform.position).z);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]));

            //accept only directions at less tahn 90degrees to the target direction
            if(result>0)
            {
                float interestValue = result;
                if(interestValue > interest[i])
                {
                    interest[i] = interestValue;
                }
            }
        }
        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if(!showGizmos) return;
        Gizmos.DrawSphere(targetPositionCached, .2f);

        if(Application.isPlaying && interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]) * interestsTemp[i]);
            }
            if (reachedLastTarget == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPositionCached, .1f);
            }
        }
    }
}


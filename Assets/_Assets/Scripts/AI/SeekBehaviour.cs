using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField] private float targetReachedThreshold = 0.7f;
    [SerializeField] private bool showGizmos = true;

    public UnityEvent OnTargetReached;

    private bool reachedLastTarget = true;

    //gizmo parameters
    private Vector3 targetPositionCached;
    private float[] interestsTemp;
    private Vector3 moveDirectionFlowFieldTemp;

    public override (float[] danger, float[] interest) GetSteeringToTargets(float[] danger, float[] interest, AIData aiData)
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
                aiData.currentTarget = aiData.targets.OrderBy(target => Vector3.Distance(transform.position, target.position)).FirstOrDefault();
            }
        }

        //cache the last position only if we still see the target (if the targets collection is not empty)
        if(aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
        {
            targetPositionCached = aiData.currentTarget.position;
        }

        //first check if we have reached the target
        if(Vector3.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            Debug.Log("Target reached");
            reachedLastTarget = true;
            aiData.currentTarget = null;
            OnTargetReached?.Invoke();
            return (danger, interest);
        }

        //if we havent reached the target, do the main logic of finding the interest directions
        Vector2 directionToTarget = new Vector2((targetPositionCached - transform.position).x, (targetPositionCached - transform.position).z);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]));

            //accept only directions at less than 90 degrees to the target direction
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


    public override (float[] danger, float[] interest) GetSteeringFlowFields(float[] danger, float[] interest, AIData aiData)
    {
        Node nodeBelow;
        //using declaration and type pattern to check the type of AIData
        if(aiData is ProtesterData protesterData)
        {
            //if we don't have a target stop seeking
            if(protesterData.reachedEndOfProtest || protesterData.flowFieldsProtest.Count == 0)
            {
                Debug.Log("Stopped seeking");
                return (danger, interest);
            }

            //executes the main logic
            //get NPC position on grid
            nodeBelow = protesterData.flowFieldsProtest[protesterData.currentFlowFieldIndex].flowField.GetNodeFromWorldPoint(transform.position);
        }
        else if(aiData is PolicemanData policemanData)
        {
            //if we don't have a target stop seeking
            if(policemanData.currentFlowField == null)
            {
                return (danger, interest);
            }

            //executes the main logic
            //get NPC position on grid
            nodeBelow = policemanData.currentFlowField.GetNodeFromWorldPoint(transform.position);    
        }
        else
        {
            nodeBelow = null;
            Debug.LogWarning("GetSteeringFlowFields method failed due to invalid AIData subtype used");
        }
        
        //Update the move direction of the player based on its position on the grid
        Vector3 moveDirectionFlowField = new Vector3(nodeBelow.bestDirection.Vector.x, 0, nodeBelow.bestDirection.Vector.y);
        moveDirectionFlowFieldTemp = moveDirectionFlowField;
        //if we havent reached the target, do the main logic of finding the interest directions
        Vector2 directionToTarget = new Vector2(moveDirectionFlowField.x, moveDirectionFlowField.z);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]));

            //accept only directions at less than 90 degrees to the target direction
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
                Vector3 direction = new Vector3(GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).x, 0, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).y);
                Gizmos.DrawRay(transform.position, direction * interestsTemp[i]);
            }
            if (reachedLastTarget == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPositionCached, .1f);
            }
        }
    }
}



using UnityEngine;
using UnityEngine.Events;

public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField] private float _targetReachedThreshold = 0f;
    [SerializeField] private bool _showGizmos = false;

    public UnityEvent OnTargetLost;

    private bool _reachedLastTarget = true;

    //gizmo parameters
    private Vector3 _targetPosition;
    private float[] _interestsTemp;
    private Vector3 _moveDirectionFlowFieldTemp;

    public override (float[] danger, float[] interest) GetSteeringToTargets(float[] danger, float[] interest, AIData aiData)
    {
        //if we don't have a target stop seeking
        //else set a new target
        if(_reachedLastTarget)
        {
            if(aiData.Targets == null || aiData.Targets.Count <= 0)
            {
                //aiData.CurrentTarget = null;
                OnTargetLost?.Invoke();
                return (danger, interest);
            }
            else
            {
                _reachedLastTarget = false;
                //aiData.CurrentTarget = aiData.Targets.OrderBy(target => Vector3.Distance(transform.position, target.position)).FirstOrDefault();
            }
        }

        //cache the last position only if we still see the target (if the targets collection is not empty)
        if(aiData.CurrentTarget != null && aiData.Targets != null && aiData.Targets.Contains(aiData.CurrentTarget))
        {
            _targetPosition = aiData.CurrentTarget.position;
        }

        //first check if we have reached the target
        if(Utility.Distance2DBetweenVector3(transform.position, _targetPosition) <= _targetReachedThreshold)
        {
            _reachedLastTarget = true;
            return (danger, interest);
        }
        
        //if we havent reached the target, do the main logic of finding the interest directions
         UpdateInterestDirections((_targetPosition - transform.position), ref interest);

        _interestsTemp = interest;
        return (danger, interest);
    }


    public override (float[] danger, float[] interest) GetSteeringFlowFields(float[] danger, float[] interest, AIData aiData)
    {
        Node nodeBelow;
        //using declaration and type pattern to check the type of AIData
        if(aiData is ProtesterData protesterData)
        {
            //if we don't have a target stop seeking
            if(protesterData.FlowFieldsProtest.Count == 0)
            {
                return (danger, interest);
            }

            //executes the main logic
            //get NPC position on grid
            nodeBelow = protesterData.FlowFieldsProtest[protesterData.CurrentFlowFieldIndex].FlowField.GetNodeFromWorldPoint(transform.position);
        }
        else if(aiData is PolicemanData policemanData)
        {
            //if we don't have a target stop seeking
            if(policemanData.CurrentFlowField == null)
            {
                return (danger, interest);
            }

            //executes the main logic
            //get NPC position on grid
            nodeBelow = policemanData.CurrentFlowField.GetNodeFromWorldPoint(transform.position);
        }
        else
        {
            nodeBelow = null;
            Debug.LogWarning("GetSteeringFlowFields method failed due to invalid AIData subtype used");
        }
        
        //Update the move direction of the player based on its position on the grid
        Vector3 moveDirectionFlowField = new Vector3(nodeBelow.BestDirection.Vector.x, 0, nodeBelow.BestDirection.Vector.y);
        _moveDirectionFlowFieldTemp = moveDirectionFlowField;
        //if we havent reached the target, do the main logic of finding the interest directions
        UpdateInterestDirections(moveDirectionFlowField, ref interest);

        _interestsTemp = interest;
        return (danger, interest);
    }

    private void UpdateInterestDirections(Vector3 moveDirection, ref float[] interest)
    {
        Vector2 directionToTarget = new Vector2(moveDirection.x, moveDirection.z);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]));

            // accept only directions at less than 90 degrees to the target direction
            if (result > 0)
            {
                float interestValue = result;
                if (interestValue > interest[i])
                {
                    interest[i] = interestValue;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(!_showGizmos) return;
        Gizmos.DrawSphere(_targetPosition, .2f);

        if(Application.isPlaying && _interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _interestsTemp.Length; i++)
            {
                Vector3 direction = new Vector3(GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).x, 0, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).y);
                Gizmos.DrawRay(transform.position, direction * _interestsTemp[i]);
            }
            if (_reachedLastTarget == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_targetPosition, .1f);
            }
        }
    }
}



using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField] private float _radius = 2f;
    [SerializeField] private float _agentColliderSize = .7f;
    [SerializeField] private bool _showGizmo = true;

    //gizmo parameters
    private float[] _dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteeringToTargets(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider obstacleCollider in aiData.Obstacles)
        {
            if(obstacleCollider == null) return (danger, interest);

            Vector3 vector3ToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            Vector2 vector2ToObstacle = new Vector2(vector3ToObstacle.x, vector3ToObstacle.z);
            float distanceToObstacle = vector2ToObstacle.magnitude;

            //calculate weight based on the distance NPC <-> Obstacle
            float weight = distanceToObstacle <= _agentColliderSize ? 1 : (_radius - distanceToObstacle) / _radius;

            Vector2 directionToObstacle2D = vector2ToObstacle.normalized;

            //add obstacle parameters to the danger array
            for (int i = 0; i < GridDirection.CardinalAndIntercardinalDirections.Count; i++)
            {
                float result = Vector2.Dot(directionToObstacle2D, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]));

                float dangerValue = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                if(dangerValue > danger[i])
                {
                    danger[i] = dangerValue;
                }
            }
        }
        _dangersResultTemp = danger;
        return (danger, interest);
    }

    public override (float[] danger, float[] interest) GetSteeringFlowFields(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider obstacleCollider in aiData.Obstacles)
        {
            if(obstacleCollider == null) return (danger, interest);
            
            Vector3 vector3ToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            Vector2 vector2ToObstacle = new Vector2(vector3ToObstacle.x, vector3ToObstacle.z);
            float distanceToObstacle = vector2ToObstacle.magnitude;

            //calculate weight based on the distance NPC <-> Obstacle
            float weight = distanceToObstacle <= _agentColliderSize ? 1 : (_radius - distanceToObstacle) / _radius;

            Vector2 directionToObstacle2D = vector2ToObstacle.normalized;

            //add obstacle parameters to the danger array
            for (int i = 0; i < GridDirection.CardinalAndIntercardinalDirections.Count; i++)
            {
                float result = Vector2.Dot(directionToObstacle2D, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]));

                float dangerValue = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                if(dangerValue > danger[i])
                {
                    danger[i] = dangerValue;
                }
            }
        }
        _dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if(!_showGizmo) return;

        if(Application.isPlaying && _dangersResultTemp != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _dangersResultTemp.Length; i++)
            {
                Vector3 direction = new Vector3(GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).x, 0, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).y);
                Gizmos.DrawRay(transform.position,  direction * _dangersResultTemp[i]);
            }
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }

}

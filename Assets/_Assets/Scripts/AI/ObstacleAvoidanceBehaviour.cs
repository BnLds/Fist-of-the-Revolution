using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float agentColliderSize = .7f;
    [SerializeField] private bool showGizmo = true;

    //gizmo parameters
    private float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteeringToTargets(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider obstacleCollider in aiData.obstacles)
        {
            Vector3 vector3ToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            Vector2 vector2ToObstacle = new Vector2(vector3ToObstacle.x, vector3ToObstacle.z);
            float distanceToObstacle = vector2ToObstacle.magnitude;

            //calculate weight based on the distance NPC <-> Obstacle
            float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;

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
        dangersResultTemp = danger;
        return (danger, interest);
    }

    public override (float[] danger, float[] interest) GetSteeringFlowFields(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider obstacleCollider in aiData.obstacles)
        {
            Vector3 vector3ToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            Vector2 vector2ToObstacle = new Vector2(vector3ToObstacle.x, vector3ToObstacle.z);
            float distanceToObstacle = vector2ToObstacle.magnitude;

            //calculate weight based on the distance NPC <-> Obstacle
            float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;

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
        dangersResultTemp = danger;
        return (danger, interest);
    }



    private void OnDrawGizmos()
    {
        if(!showGizmo) return;

        if(Application.isPlaying && dangersResultTemp != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < dangersResultTemp.Length; i++)
            {
                Vector3 direction = new Vector3(GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).x, 0, GridDirection.GetNormalizedDirectionVector(GridDirection.CardinalAndIntercardinalDirections[i]).y);
                Gizmos.DrawRay(transform.position,  direction * dangersResultTemp[i]);
            }
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

}

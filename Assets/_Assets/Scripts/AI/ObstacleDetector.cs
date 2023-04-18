using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool showGizmos = true;

    private Collider[] colliders;

    public override void Detect(AIData aiData)
    {
        colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);
        aiData.obstacles = colliders;
    }

    private void OnDrawGizmos()
    {
        if(!showGizmos) return;
        if(Application.isPlaying && colliders != null)
        {
            foreach(Collider obstacleCollider in colliders)
            {
                Gizmos.DrawIcon(obstacleCollider.transform.position + Vector3.up*2, "33");
            }
        }
    }
}

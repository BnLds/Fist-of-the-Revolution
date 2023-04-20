using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool showGizmos = true;

    private List<Collider> colliders;

    public override void Detect(AIData aiData)
    {
        colliders = new List<Collider>();
        foreach(Collider collider in Physics.OverlapSphere(transform.position, detectionRadius, layerMask))
        {
            //make sure the instance doesn't detect itself as collider
            if (collider.transform.parent != aiData.transform)
            {
                colliders.Add(collider);
            }
        }
        aiData.obstacles = colliders.ToArray();
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

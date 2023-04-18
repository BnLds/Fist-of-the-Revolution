using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float targetDetectionRange = 5f;
    [SerializeField] private LayerMask obstaclesLayerMask, targetsLayerMask;
    [SerializeField] private bool showGizmos = false;

    private List<Transform> colliders;

    public override void Detect(AIData aiData)
    {
        //find out if targets are near
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, targetDetectionRange, targetsLayerMask);

        if(targetColliders.Length != 0)
        {
            //check if it can see the first target
            Vector3 direction = (targetColliders[0].transform.position - transform.position).normalized;

            //targets are also on the obstaclesLayerMask
            bool canSeePlayer = Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, targetDetectionRange, obstaclesLayerMask);

            //Make sure the collider we see is on the targetLayer.
            if(canSeePlayer && 1<<hitInfo.collider.gameObject.layer == targetsLayerMask.value)
            {

                //Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.blue);
                colliders = new List<Transform>() { targetColliders[0].transform };
            }
            else
            {
                colliders = null;
            }
        }
        else
        {
            colliders = null;
        }

        aiData.targets = colliders;
    }

    private void OnDrawGizmos()
    {
        if(!showGizmos) return;

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if(colliders == null) return;

        foreach(Transform collider in colliders)
        {
            Gizmos.DrawIcon(collider.position + Vector3.up*2, "32");
        }
    }
}

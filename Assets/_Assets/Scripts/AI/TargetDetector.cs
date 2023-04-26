using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float _targetDetectionRange = 5f;
    [SerializeField] private LayerMask _obstaclesLayerMask, _targetsLayerMask;
    [SerializeField] private bool _showGizmos = false;

    private List<Transform> _colliders;

    public override void Detect(AIData aiData)
    {
        //find out if targets are near
        Collider[] targetColliders = Physics.OverlapSphere(transform.position, _targetDetectionRange, _targetsLayerMask);

        if(targetColliders.Length != 0)
        {
            //check if it can see the first target
            Vector3 direction = (targetColliders[0].transform.position - transform.position).normalized;

            //targets are also on the obstaclesLayerMask
            bool canSeePlayer = Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, _targetDetectionRange, _obstaclesLayerMask);

            //Make sure the collider we see is on the targetLayer.
            if(canSeePlayer && 1<<hitInfo.collider.gameObject.layer == _targetsLayerMask.value)
            {

                //Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.blue);
                _colliders = new List<Transform>() { targetColliders[0].transform };
            }
            else
            {
                _colliders = null;
            }
        }
        else
        {
            _colliders = null;
        }

        aiData.Targets = _colliders;
    }

    private void OnDrawGizmos()
    {
        if(!_showGizmos) return;

        Gizmos.DrawWireSphere(transform.position, _targetDetectionRange);

        if(_colliders == null) return;

        foreach(Transform collider in _colliders)
        {
            Gizmos.DrawIcon(collider.position + Vector3.up*2, "32");
        }
    }
}

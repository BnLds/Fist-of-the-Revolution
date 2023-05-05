using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float _targetDetectionRange = 20f;
    [SerializeField] private LayerMask _obstaclesLayerMask, _targetsLayerMask;
    [SerializeField] private bool _showGizmos = false;

    private List<Transform> _colliders;

    public override void Detect(AIData aiData)
    {
        Collider[] targetColliders;
        if(aiData is PolicemanData policemandData)
        {
            //targetColliders = PoliceResponseData.TrackedSuspects.Select(_ => _.SuspectTransform.GetComponent<Collider>()).ToArray();
            aiData.Targets = PoliceResponseData.TrackedSuspects.Select(_ => _.SuspectTransform).ToList();
        }
        else
        {
            //find out if targets are near
            targetColliders = Physics.OverlapSphere(transform.position, _targetDetectionRange, _targetsLayerMask);
            if (targetColliders.Length != 0)
            {
                _colliders = new List<Transform>();
                foreach (Collider target in targetColliders)
                {
                    //check if it can see the first target
                    Vector3 direction = (target.transform.position - transform.position).normalized;

                    //targets are also on the obstaclesLayerMask
                    bool canSeePlayer = Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, _targetDetectionRange, _obstaclesLayerMask);

                    //Make sure the collider we see is on the targetLayer.
                    if (canSeePlayer && 1 << hitInfo.collider.gameObject.layer == _targetsLayerMask.value)
                    {

                        //Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.blue);
                        _colliders.Add(target.transform);
                    }
                }
            }
            aiData.Targets = _colliders;
        }


        
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

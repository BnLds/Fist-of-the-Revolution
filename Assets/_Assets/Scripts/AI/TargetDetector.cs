using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float _targetDetectionRange = 20f;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask[] _targetsLayerMask;
    //[SerializeField] private bool _showGizmos = false;

    private Vector3 _directionGizmo;

    public override void Detect(AIData aiData)
    {
        if(aiData is PolicemanData policemandData)
        {
            aiData.Targets.Clear();

            var targetColliders = PoliceResponseManager.Instance.GetTrackedList().Select(_ => _.SuspectTransform.GetComponent<Collider>());
            
            if (targetColliders!=null && targetColliders.Count() != 0)
            {
                foreach (Collider target in targetColliders)
                {
                    //check if it can see the first target
                    Vector3 direction = (target.transform.position - transform.position).normalized;

                    //targets are also on the obstaclesLayerMask
                    bool isPlayerInDetectionRange = Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, _targetDetectionRange, _obstaclesLayerMask);

                    //Make sure the collider we see is on the targetLayer.
                    if (isPlayerInDetectionRange && _targetsLayerMask.FirstOrDefault(_ => _.value == 1 << hitInfo.collider.gameObject.layer) != 0)
                    {
                        //Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.blue);
                        aiData.Targets.Add(target.transform);
                    }
                }
            }
        }
    }

/*
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
*/
}

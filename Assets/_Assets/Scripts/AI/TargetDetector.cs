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
    private List<Transform> _targets = new();

    public override void Detect(AIData aiData)
    {
        if(aiData is PolicemanData policemandData)
        {
            aiData.Targets.Clear();
            _targets.Clear();
            _targets = PoliceResponseManager.Instance.GetTrackedList().Select(_ => _.SuspectTransform).ToList();
            
            if (_targets!=null && _targets.Count() != 0)
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    //check if it can see the first target
                    Vector3 direction = (_targets[i].position - transform.position).normalized;

                    //targets are also on the obstaclesLayerMask
                    bool isPlayerInDetectionRange = Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, _targetDetectionRange, _obstaclesLayerMask);

                    //Make sure the collider we see is on the targetLayer.
                    if (isPlayerInDetectionRange && _targetsLayerMask.FirstOrDefault(_ => _.value == 1 << hitInfo.collider.gameObject.layer) != 0)
                    {
                        //Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.blue);
                        aiData.Targets.Add(_targets[i]);
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

using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField] private float _detectionRadius = 2f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private bool _showGizmos = false;

    private List<Collider> _colliders;

    public override void Detect(AIData aiData)
    {
        _colliders = new List<Collider>();
        foreach(Collider collider in Physics.OverlapSphere(transform.position, _detectionRadius, _layerMask))
        {
            //make sure the instance doesn't detect itself as collider
            if (collider.transform.parent != aiData.transform)
            {
                _colliders.Add(collider);
            }
        }
        aiData.Obstacles = _colliders.ToArray();
    }

    private void OnDrawGizmos()
    {
        if(!_showGizmos) return;
        if(Application.isPlaying && _colliders != null)
        {
            foreach(Collider obstacleCollider in _colliders)
            {
                Gizmos.DrawIcon(obstacleCollider.transform.position + Vector3.up*2, "33");
            }
        }
    }
}

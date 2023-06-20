using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField] private float _detectionRadius = 2f;
    [SerializeField] private LayerMask _layerMask;
    //[SerializeField] private bool _showGizmos = false;

    private static int _maxColliders = 100;
    private Collider[] _hitColliders = new Collider[_maxColliders];

    public override void Detect(AIData aiData)
    {
        aiData.Obstacles.Clear();

        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _hitColliders, _layerMask);

        for (int i = 0; i < numColliders; i++)
        {
            //make sure the instance doesn't detect itself as collider
            if (_hitColliders[i].transform.parent != aiData.transform)
            {
                aiData.Obstacles.Add(_hitColliders[i]);
            }
        }
    }

    /*
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
    */
}

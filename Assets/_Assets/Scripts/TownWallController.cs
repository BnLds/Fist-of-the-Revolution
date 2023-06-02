using UnityEngine;

public class TownWallController : MonoBehaviour
{
    [SerializeField] private LayerMask _avoidedLayers;

    private Collider _wallCollider;

    private void Awake()
    {
        _wallCollider = GetComponent<MeshCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == _avoidedLayers.value)
        {
            Physics.IgnoreCollision(collision.collider, _wallCollider);
        }
    }
}

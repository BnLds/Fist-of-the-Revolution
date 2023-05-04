using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private LayerMask _breakableMask;
    [SerializeField] private LayerMask _avoidCollisionMask;

    [HideInInspector] public UnityEvent OnDamageBreakable;

    private Rigidbody _playerRigidbody;
    private Collider _playerCollider;
    [SerializeField] private int _playerDamage = 1;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        GameInput.Instance.OnInteract.AddListener(PerformInteractAction);
    }

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(GameInput.Instance.GetMovementVectorNormalized().x, 0,GameInput.Instance.GetMovementVectorNormalized().y);

        _playerRigidbody.velocity = moveInput * _moveSpeed;
    }

    private void PerformInteractAction()
    {
        float detectionRadius = 2f;
        Collider[] breakableColliders = Physics.OverlapSphere(transform.position, detectionRadius, _breakableMask);
        foreach(Collider collider in breakableColliders)
        {
            collider.GetComponent<BreakableController>().Damage(_playerDamage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(1 << collision.collider.gameObject.layer == _avoidCollisionMask.value)
        {
            Physics.IgnoreCollision(collision.collider, _playerCollider);
        }
    }
}
